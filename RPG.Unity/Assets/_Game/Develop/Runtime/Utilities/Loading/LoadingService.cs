using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Cysharp.Threading.Tasks;
using PleasantlyGames.RPG.Runtime.Utilities.Logging;
using UnityEngine.Scripting;

namespace PleasantlyGames.RPG.Runtime.Utilities.Loading
{
    public interface IDisposableLoadUnit : ILoadUnit, IDisposable { }

    public interface ILoadUnit
    {
        string DescriptionToken { get; }
        
        UniTask LoadAsync(CancellationToken token, IProgress<float> progress = null);
    }

    public interface ILoadUnit<in T>
    {
        UniTask LoadAsync(T param, IProgress<float> progress = null);
    }

    public interface IDisposableLoadUnit<in T> : ILoadUnit<T>, IDisposable { }

    public sealed class LoadingService
    {
        private readonly Stopwatch _watch =
#if !COMPANYNAME_PROD
            new();
#else
            null;
#endif

        public readonly List<IDisposable> Disposables = new();
        
        [Preserve]
        public LoadingService()
        {
        }

        private void OnLoadingBegin(object unit)
        {
            _watch?.Restart();
            Log.Loading.D($"{unit.GetType().Name} loading is started");
        }

        private async UniTask OnLoadingFinish(object unit, bool isError)
        {
            _watch?.Stop();
            Log.Loading.D($"{unit.GetType().Name} is {(isError ? "NOT " : "")}loaded with time {_watch?.ElapsedMilliseconds}ms");

            int currentThreadId = Thread.CurrentThread.ManagedThreadId;
            int mainThreadId = PlayerLoopHelper.MainThreadId;

            if (mainThreadId != currentThreadId) {
                _watch?.Restart();
                Log.Loading.D("THREADING",$"start switching from '{currentThreadId}' thread to main thread '{mainThreadId}'");
                await UniTask.SwitchToMainThread();
                _watch?.Stop();
                Log.Loading.D("THREADING",$"switch finished with time {_watch?.ElapsedMilliseconds}");
            }
        }

        // public async UniTask BeginLoading(ILoadUnit loadUnit, bool skipExceptionThrow = false)
        // {
        //     var isError = true;
        //
        //     try {
        //         OnLoadingBegin(loadUnit);
        //         await loadUnit.Load();
        //         isError = false;
        //     }
        //     catch (Exception e) {
        //         Log.Loading.E(e);
        //
        //         if (!skipExceptionThrow)
        //             throw;
        //     }
        //     finally {
        //         await OnLoadingFinish(loadUnit, isError);
        //     }
        // }

        // public async UniTask BeginLoading(IDisposableLoadUnit unit, bool skipExceptionThrow = false)
        // {
        //     Disposables.Add(unit);
        //     await BeginLoading((ILoadUnit)unit, skipExceptionThrow);
        // }

        public async UniTask BeginLoading<T>(ILoadUnit<T> loadUnit, T param, bool skipExceptionThrow = false)
        {
            var isError = true;

            try {
                OnLoadingBegin(loadUnit);
                await loadUnit.LoadAsync(param);
                isError = false;
            }
            catch (Exception e) {
                Log.Loading.E(e);

                if (!skipExceptionThrow)
                    throw;
            }
            finally {
                await OnLoadingFinish(loadUnit, isError);
            }
        }

        public async UniTask BeginLoading<T>(IDisposableLoadUnit<T> unit, T param, bool skipExceptionThrow = false)
        {
            Disposables.Add(unit);
            await BeginLoading((ILoadUnit<T>)unit, param, skipExceptionThrow);
        }

        // public async UniTask BeginLoading(bool skipExceptionThrow = false, params ILoadUnit[] units)
        // {
        //     foreach (ILoadUnit loadUnit in units)
        //         await BeginLoading(loadUnit, skipExceptionThrow);
        // }

        // public async UniTask BeginLoadingParallel(string logName, bool skipExceptionThrow = false, params ILoadUnit[] units)
        // {
        //     var isError = true;
        //
        //     try {
        //         OnLoadingBegin(logName);
        //         var t = UniTask.WhenAll(units.Select(e => e.Load()));
        //         await t;
        //         isError = false;
        //     }
        //     catch (Exception e) {
        //         Log.Loading.E(e);
        //
        //         if (!skipExceptionThrow)
        //             throw;
        //     }
        //     finally {
        //         await OnLoadingFinish(logName, isError);
        //     }
        // }
    }
}