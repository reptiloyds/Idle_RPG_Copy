using System.Collections.Generic;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Analytics.ErrorsAnalytics
{
    public class ErrorAnalyticsLogger
    {
        private class ErrorInfo
        {
            public int Count;
            public float LastSentTime;
        }

        private Dictionary<string, ErrorInfo> _errorCache = new Dictionary<string, ErrorInfo>();
        private float _minIntervalBetweenSameError = 10f;
        private int _maxSendsPerError = 5;
        private bool _fullErrorStackTrace = true;
        private float _cacheCleanupInterval = 60f;
        private float _errorExpiryTime = 300f;
        private float _lastCacheCleanupTime = 0f;

        public string GetMessage(string condition, string stacktrace, LogType type)
        {
            if (Time.time - _lastCacheCleanupTime > _cacheCleanupInterval)
            {
                CleanupCache();
                _lastCacheCleanupTime = Time.time;
            }

            if (type != LogType.Error && type != LogType.Exception)
                return null;

            string resultStackTrace;

            if (_fullErrorStackTrace)
            {
                resultStackTrace = stacktrace;
            }
            else
            {
                int startIndex = stacktrace.IndexOf("(at");
                if (startIndex == -1)
                {
                    resultStackTrace = stacktrace;
                }
                else
                {
                    startIndex += 4;
                    int endIndex = stacktrace.IndexOf(")", startIndex);
                    if (endIndex == -1)
                        endIndex = stacktrace.Length;

                    resultStackTrace = stacktrace.Substring(startIndex, endIndex - startIndex);
                }
            }

            var message = $"{condition}:{resultStackTrace}";

            if (_errorCache.TryGetValue(message, out var errorInfo))
            {
                if (errorInfo.Count >= _maxSendsPerError)
                    return null;

                if (Time.time - errorInfo.LastSentTime < _minIntervalBetweenSameError)
                    return null;

                errorInfo.Count++;
                errorInfo.LastSentTime = Time.time;
            }
            else
            {
                _errorCache[message] = new ErrorInfo { Count = 1, LastSentTime = Time.time };
            }

            return message;
        }

        private void CleanupCache()
        {
            float currentTime = Time.time;
            var keysToRemove = new List<string>();

            foreach (var kvp in _errorCache)
            {
                if (currentTime - kvp.Value.LastSentTime > _errorExpiryTime)
                {
                    keysToRemove.Add(kvp.Key);
                }
            }

            foreach (var key in keysToRemove)
            {
                _errorCache.Remove(key);
            }
        }
    }
}