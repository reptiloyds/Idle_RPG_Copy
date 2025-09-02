// #define UNITY_WEBGL
// #undef UNITY_EDITOR
// #undef UNITY_ANDROID

using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;
using NutakuUnitySdk;
using PleasantlyGames.RPG.NutakuRuntime.Nutaku;
using PleasantlyGames.RPG.Runtime.SystemFeature.Contract;
using R3;
using UnityEngine;
using UnityEngine.Scripting;
using Logger = PleasantlyGames.RPG.Runtime.DebugUtilities.Logger;

namespace _Game.Scripts.Systems.Nutaku
{
    public class NutakuSystem : MonoBehaviour, ISystemDataProvider
    {
        public event Action ProfileInitiateFinished;
        
        private Action<bool> _purchaseCallBack;

        private string _playerNutakuId;
        private string _platformNutakuId;
        private string _userNumericId;
        private string _accessToken;
        private readonly ReactiveProperty<NutakuState> _state = new(NutakuState.NotInitialized);

        public string NumericId => _userNumericId;
        public string AccessToken => _accessToken;
        public string UserId => _playerNutakuId;
        public string PlatformId => _platformNutakuId;
        public ReadOnlyReactiveProperty<NutakuState> State => _state;

        public void Init()
        {
#if !UNITY_WEBGL || UNITY_EDITOR
            NutakuConfigHandler.OnLoginAction += BrowserAuthCallBack;
            NutakuSdk.Initialize(this);
#endif
#if UNITY_WEBGL && !UNITY_EDITOR
            Logger.Log("NutakuSystem: GetProfileData");
            NutakuJsBridge.GetProfileData();
            Logger.Log("NutakuSystem: WebGameHandshake");
            NutakuJsBridge.WebGameHandshake();
            Logger.Log("NutakuSystem: Init End");
#endif
        }

#if UNITY_ANDROID || UNITY_EDITOR
        private void BrowserAuthCallBack(bool wasSuccess)
        {
            Logger.Log("NutakuSystem: initialization result" + wasSuccess);
            if (wasSuccess)
            {
                _playerNutakuId = NutakuCurrentUser.GetUserNickname();
                _platformNutakuId = NutakuCurrentUser.GetUserId().ToString();
                NutakuApi.GameHandshake(this, OnFinalAuthStepFinished);
            }
            else
            {
#if UNITY_EDITOR
                // nothing to do in this situation for Unity Editor mode. The NutakuUnityEditorLogin is automatically started by the SDK.
#else
                // Logger.Log("NutakuSystem: Login in browser");
                // NutakuSdk.OpenApkLoginPageInBrowser();
#endif
            }
        }
#endif
        
        [Preserve]
        public void RequestNickNameCallback(string name)
        {
            _playerNutakuId = name;
            Logger.Log($"Profile get, _playerNutakuId is: {_playerNutakuId}");
        }
        
        [Preserve]
        public void RequestPlatformIdCallback(string id)
        {
            _platformNutakuId = id;
            Logger.Log("RequestPlatformIdCallback: " + _platformNutakuId);
        }

        #region GET_FIELDS

        [Preserve]
        public string GetUserId()
        {
            return _playerNutakuId;
        }

        [Preserve]
        public string GetUserNumericId()
        {
            return _userNumericId;
        }

        [Preserve]
        public string GetAccessToken()
        {
            return _accessToken;
        }
        
        #endregion
        
        [Preserve]
        public void PostPayment(NutakuInApp inAppsConfig, Action<bool> callBack)
        {
            _purchaseCallBack = callBack;

#if UNITY_EDITOR || UNITY_ANDROID
            PostPaymentAndroid(inAppsConfig);
#elif UNITY_WEBGL
            PostPaymentWeb(inAppsConfig);
#endif
        }
        
#if UNITY_WEBGL && !UNITY_EDITOR

        [Preserve]
        public void WebHandShakeCallBack(string callBackData)
        {
            try
            {
                var result = JsonConvert.DeserializeObject<NutakuGameHandshakeResponse>(callBackData);
                if (result.game_rc is > 199 and < 300)
                {
                    var data = JsonConvert.DeserializeObject<IDictionary<string, string>>(result.message);
                    _accessToken = data["access_token"];
                    _userNumericId = data["user_numeric_id"]; // our server id
                    if (string.IsNullOrEmpty(_accessToken) || string.IsNullOrEmpty(_userNumericId))
                    {
                        Logger.LogError($"Failed to get player profile, some data is null: {result.message}");
                        _state.Value = NutakuState.InitializationError;
                        return;
                    }

                    Logger.Log($"Profile get, nutaku id is: {_playerNutakuId}");
                    ProfileInitiateFinished?.Invoke();
                    _state.Value = NutakuState.Initialized;
                }
                else
                {
                    Logger.LogError($"Failed to get player profile: {result.message}");
                    _state.Value = NutakuState.InitializationError;
                }
            }
            catch (Exception e)
            {
                Logger.LogError($"WebHandShakeCallBack Error: {e.Message}");
                _state.Value = NutakuState.InitializationError;
            }
        }

        [Preserve]
        public void PostPaymentWeb(NutakuInApp inAppsConfig)
        {
            NutakuJsBridge.CreatePayment(inAppsConfig.Id,
                inAppsConfig.FormattedName,
                inAppsConfig.ImageUrl,
                inAppsConfig.Price,
                1,
                inAppsConfig.FormattedDescription);
        }

        [Preserve]
        public void PostPaymentWebCallBack(string resultStr)
        {
            Logger.Log($"PaymentWebStatus STRING is {resultStr}");
            var isSuccess = bool.Parse(resultStr);
            Logger.Log($"PaymentWebStatus BOOL is {isSuccess}");
            _purchaseCallBack?.Invoke(isSuccess);
        }
        
#elif UNITY_EDITOR || UNITY_ANDROID
        [Preserve]
        private void OnFinalAuthStepFinished(NutakuApiRawResult rawResult)
        {
            try
            {
                if (rawResult.responseCode is > 199 and < 300)
                {
                    var result = NutakuApi.Parse_GameHandshake(rawResult);
                    if (result.game_rc is > 199 and < 300)
                    {
                        var data = JsonConvert.DeserializeObject<IDictionary<string, string>>(result.message);
                        _accessToken = data["access_token"];
                        _userNumericId = data["user_numeric_id"]; // our server id
                        if (string.IsNullOrEmpty(_accessToken) || string.IsNullOrEmpty(_userNumericId))
                        {
                            Logger.LogError($"Failed to get player profile, some data is null: {result.message}");
                            _state.Value = NutakuState.InitializationError;
                            return;
                        }

                        Logger.Log($"Profile get, nutaku id is: {_playerNutakuId}");
                        ProfileInitiateFinished?.Invoke();
                        _state.Value = NutakuState.Initialized;
                    }
                    else
                    {
                        Logger.LogError($"Failed to get player profile: {result.message}");
                        _state.Value = NutakuState.InitializationError;
                    }
                }
                else
                {
                    Logger.LogError($"Failed to get player profile: {rawResult.body}");
                    _state.Value = NutakuState.InitializationError;
                }
            }
            catch (Exception e)
            {
                Logger.LogError($"Failed to get player profile: [{e.Message}]");
                _state.Value = NutakuState.InitializationError;
            }
        }
        
        [Preserve]
        private void PostPaymentAndroid(NutakuInApp inAppsConfig)
        {
            try
            {
                NutakuPayment payment = NutakuPayment.PaymentCreationInfo(
                    inAppsConfig.Id,
                    inAppsConfig.FormattedName,
                    inAppsConfig.Price,
                    inAppsConfig.ImageUrl,
                    inAppsConfig.FormattedDescription);

                Logger.Log("PostPayment Start");

                NutakuApi.CreatePayment(payment, this, PostPaymentCallback);
            }
            catch (Exception ex)
            {
                Logger.Log("PostPayment Failure");
                Logger.Log(ex.Message);

                _purchaseCallBack?.Invoke(false);
            }
        }

        [Preserve]
        public void PostPaymentCallback(NutakuApiRawResult rawResult)
        {
            try
            {
                if (rawResult.responseCode is > 199 and < 300)
                {
                    var result = NutakuApi.Parse_CreatePayment(rawResult);

                    Logger.Log($"PostPayment Success {result.paymentId} {result.name}");
                    Logger.Log($"PostPayment is PUT: {result.next}");

                    // _payment = result;
                    NutakuApi.PutPayment(result.paymentId, this, OpenPaymentCallBack);
                }
                else
                {
                    _purchaseCallBack?.Invoke(false);
                    Logger.Log("PostPayment Failure");
                    Logger.Log("Http Status Code: " + rawResult.responseCode);
                    Logger.Log("Http Status Message: " + rawResult.body);
                }
            }
            catch (Exception ex)
            {
                Logger.Log("PostPayment Failure");
                Logger.Log(ex.Message);

                _purchaseCallBack?.Invoke(false);
            }
        }

        [Preserve]
        public void OpenPaymentCallBack(NutakuApiRawResult resultDelegate)
        {
            try
            {
                if (resultDelegate.responseCode is > 199 and < 300)
                {
                    Logger.Log("OpenPaymentSuccess");
                    _purchaseCallBack?.Invoke(true);
                }
                else
                {
                    Logger.Log("OpenPaymentFailure");
                    _purchaseCallBack?.Invoke(false);
                }
            }
            catch (Exception ex)
            {
                Logger.Log("OpenPayment Failure");
                Logger.Log(ex.Message);

                _purchaseCallBack?.Invoke(false);
            }
        }
#endif
        [Preserve]
        public void LogOut()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            NutakuSdk.LogoutAndExit();
#endif
        }
    }
}