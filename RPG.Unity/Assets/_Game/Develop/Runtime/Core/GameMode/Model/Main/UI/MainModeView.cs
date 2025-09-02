using System;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Button.Model.Base;
using PleasantlyGames.RPG.Runtime.Core.UI.CommonComponents;
using Sirenix.OdinInspector;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace PleasantlyGames.RPG.Runtime.Core.GameMode.Model.Main.UI
{
    public class MainModeView : MonoBehaviour, IInitializable, IDisposable
    {
        [SerializeField, Required] private BaseButton _bossButton;
        [SerializeField, Required] private WavePresenter _wavePresenter;
        [SerializeField, Required] private UITimer _timer;
        [SerializeField, Required] private MainModeLevelPresenter _levelPresenter;

        [Inject] private MainMode _mainMode;

        void IInitializable.Initialize()
        {
            _bossButton.OnClick += OnBossButtonClick;
            _mainMode.OnLaunched += OnLaunched;
            _mainMode.OnRestarted += Redraw;
            _mainMode.OnDisposed += OnDisposed;
            _mainMode.OnBossLaunched += OnBossLaunched;
            _mainMode.OnWaveIncremented += OnWaveIncremented;
            _mainMode.OnBossDefeated += OnBossDefeated;
            
            if(_mainMode.IsLaunched)
                Enable();
            else
                Disable();
        }

        void IDisposable.Dispose()
        {
            _bossButton.OnClick -= OnBossButtonClick;
            _mainMode.OnLaunched -= OnLaunched;
            _mainMode.OnRestarted -= Redraw;
            _mainMode.OnDisposed -= OnDisposed;
            _mainMode.OnBossLaunched -= OnBossLaunched;
            _mainMode.OnWaveIncremented -= OnWaveIncremented;
            _mainMode.OnBossDefeated -= OnBossDefeated; 
        }

        private void OnLaunched(IGameMode gameMode) => Enable();

        private void OnDisposed(IGameMode gameMode) => Disable();

        private void Enable()
        {
            gameObject.SetActive(true);
            Redraw();
        }

        private void Redraw()
        {
            if(_timer.gameObject.activeSelf)
                DisableBossTimer();

            if (_mainMode.IsBossFight)
            {
                EnableBossTimer();
                DisableBossButton();
                DisableWavePresenter();
            }
            else if (_mainMode.WaveIsCleared)
            {
                EnableBossButton();
                DisableBossTimer();
                DisableWavePresenter();
            }
            else
            {
                DisableBossTimer();
                DisableBossButton();
                EnableWavePresenter(_mainMode.WaveAmount, _mainMode.HasBossOnLevel);
            }
            
            _levelPresenter.Present(_mainMode.GetFormattedFullName());
        }

        private void Disable() => 
            gameObject.SetActive(false);

        private void OnBossLaunched()
        {
            DisableBossButton();
            DisableWavePresenter();
            EnableBossTimer();
        }

        private void OnWaveIncremented() => FillWave(_mainMode.WaveIndex - 1, _mainMode.SwitchWaveDurationUI);
        private void OnBossDefeated() => DisableBossTimer();

        private void EnableBossTimer()
        {
            _timer.Listen(_mainMode.LoseDelay);
            _timer.gameObject.SetActive(true);
        }

        private void DisableBossTimer()
        {
            _timer.Stop();
            _timer.gameObject.SetActive(false);
        }

        private void FillWave(int waveNumber, float time) => _wavePresenter.Fill(waveNumber, time);
        private void EnableBossButton() => _bossButton.gameObject.SetActive(true);
        private void DisableBossButton() => _bossButton.gameObject.SetActive(false);
        private void EnableWavePresenter(int waveAmount, bool withBoss = true) => _wavePresenter.Enable(waveAmount, withBoss);
        private void DisableWavePresenter() => _wavePresenter.Disable();
        private void OnBossButtonClick()
        {
            DisableBossButton();
            _mainMode.TriggerBoss();
        }
    }
}