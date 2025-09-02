﻿#if UNITY_ANDROID || UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using Logger = PleasantlyGames.RPG.Runtime.DebugUtilities.Logger;

namespace NutakuUnitySdk
{
    /// <summary>
    /// The core of the Nutaku SDK
    /// </summary>
    public static class NutakuSdk
    {
        public const string SdkVersion = "4.0.3-unity";

        internal static long _lastVersionCodeCheckEpoch = 0L;
        internal static int _latestPublishedVersionCode = 0;
        internal static string _latestPublishedUpdateUrl = null;

        private static bool _oneTimeInitialized = false;
        private static bool _fullyInitialized = false;
        private static bool _initInProgress = false;
        private static string _previousParsedIntentUri = null;
        private static bool _successfulBrowserLoginIntent = false;
        private static bool _isN2i = false;


        /// <summary>
        /// You must call this as early as possible in your application, such as inside Awake(), before calling anything else from the SDK.
        /// </summary>
        public static void Initialize(MonoBehaviour mono)
        {
            if (_fullyInitialized)
            {
                Log("Initialize() SDK is already initialized. Skipping.");
                return;
            }
            if (_initInProgress)
            {
                Log("Initialize() SDK is already processing an initialization. Skipping.");
                return;
            }

            _initInProgress = true;
            _successfulBrowserLoginIntent = false;

            try
            {
                Log("Starting to initialize Nutaku SDK");
                if (string.IsNullOrEmpty(NutakuSdkConfig.AndroidPackageName))
                    throw new Exception("NutakuSdkConfig.AndroidPackageName cannot be empty");

                if (NutakuSdkConfig.AndroidPackageName.Contains('_'))
                    throw new Exception("NutakuSdkConfig.AndroidPackageName cannot contain underscores due to a limitation with Android OS and capturing intents from browsers");

                if (!NutakuSdkConfig.AndroidPackageName.Equals(NutakuSdkConfig.AndroidPackageName.ToLowerInvariant()))
                    throw new Exception("NutakuSdkConfig.AndroidPackageName cannot contain uppercase characters due to a limitation with Android OS and capturing intents from browsers");

                if (NutakuSdkConfig.Environment != "TEST" && NutakuSdkConfig.Environment != "SANDBOX" && NutakuSdkConfig.Environment != "STAGE" && NutakuSdkConfig.Environment != "PRODUCTION")
                    throw new Exception("Unrecognized value for NutakuSdkConfig.Environment");

                Logger.Log("Nutaku:Initialize:_oneTimeInitialized = " + _oneTimeInitialized);
                if (!_oneTimeInitialized)
                {
                    Application.deepLinkActivated += ParseDeeplink;
                    if (!string.IsNullOrEmpty(Application.absoluteURL))
                    {
                        Log("Initialize() cold start intent detected: " + Application.absoluteURL);
                        ParseDeeplink(Application.absoluteURL);
                    }
                    if (!_isN2i)
                        _isN2i = PlayerPrefs.GetInt("sdk_n2i") > 0;
                    
                    _oneTimeInitialized = true;
                }

                if (!_successfulBrowserLoginIntent)
                {
                    if (NutakuCurrentUser.GetUserId() != 0 && !string.IsNullOrEmpty(NutakuCurrentUser.GetNUGT()))
                    {
                        if (NutakuCurrentUser.IsFresh())
                        {
                            Log("Initialize() sending successful login result to game");
                            NutakuSdkConfig.loginResultToGameCallbackDelegate.Invoke(true);
                        }
                        else
                        {
                            NutakuApi.AutoLogin(NutakuCurrentUser.GetUserId(), NutakuCurrentUser.GetNUGT(), mono, AutoLoginResultCallback);
                        }
                    }
                    else
                    {
                        Log("Initialize() User was not already logged in, starting login flow");
                        NutakuSdkConfig.loginResultToGameCallbackDelegate.Invoke(false);
#if UNITY_EDITOR
                        NutakuUnityEditorLogin.Show();
#else
                        OpenApkLoginPageInBrowser();
#endif
                    }
                }

            }
            catch (Exception ex)
            {
                _initInProgress = false;
                throw ex;
            }

            _initInProgress = false;
            _fullyInitialized = true;
        }


        /// <summary>
        /// Opens the device default browser to allow the user to log in on the platform.
        /// The SDK triggers this wen needed, but your game should also call this via a button given to the user while waiting for the browser login result in case the user lost/closed the page opened by the SDK.
        /// </summary>
        public static void OpenApkLoginPageInBrowser()
        {
#if !UNITY_EDITOR
            Application.OpenURL(FE_Domain() + "/apk-browser-login/" + "?titleId=" + NutakuSdkConfig.TitleId + "&sdkVersion=" + SdkVersion + "&androidPackageName=" + NutakuSdkConfig.AndroidPackageName + (_isN2i ? "&n2i=1" : ""));
#endif
        }


        /// <summary>
        /// Call this with the payment.transactionUrl as parameter whenever a payment creation response arrives with next=flyout, or if PutPayment did not result in a 200 response.
        /// </summary>
        public static void OpenTransactionUrlInBrowser(string paymentTransactionUrl)
        {
#if UNITY_EDITOR
            throw new Exception("OpenTransactionUrlInBrowser() is not supported in Unity Editor mode!");
#else
            Application.OpenURL(paymentTransactionUrl + "&apn=" + NutakuSdkConfig.AndroidPackageName + (_isN2i ? "&n2i=1" : ""));
#endif
        }


        /// <summary>
        /// Optional feature where you can allow the user to log out and potentially switch accounts.
        /// </summary>
        public static void LogoutAndExit()
        {
            NutakuCurrentUser.ClearUserData();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.OpenURL(FE_Domain() + "/logout/" + "?titleId=" + NutakuSdkConfig.TitleId + "&sdkVersion=" + SdkVersion + "&androidPackageName=" + NutakuSdkConfig.AndroidPackageName + (_isN2i ? "&n2i=1" : ""));
            Application.Quit();
#endif
        }

        /// <summary> Returns whether the apk is currently running on the N2I streaming ecosystem.
        /// Only call this AFTER the user is logged in.
        /// </summary>
        public static bool IsN2I()
        {
            return _isN2i;
        }


        private static void AutoLoginResultCallback(NutakuApiRawResult rawResult)
        {
            bool success = false;
            if (rawResult.responseCode == 200)
            {
                try
                {
                    var result = NutakuApi.ParseApiResponse<NutakuUserProfile>(rawResult);
                    NutakuCurrentUser.SaveUserInfo(result.nugt, result.id, result.nickname, result.test, result.grade, result.language);
                    success = true;
                    Log("AutoLoginResultCallback() sending successful login result to game");
                    NutakuSdkConfig.loginResultToGameCallbackDelegate.Invoke(true);
                }
                catch (Exception e)
                {
                    Log("AutoLoginResultCallback() failed to parse login info: " + e.Message);
                }
                
            }

            if (!success)
            {
                NutakuCurrentUser.ClearUserData();
                Log("AutoLoginResultCallback() autologin not successful");
                NutakuSdkConfig.loginResultToGameCallbackDelegate.Invoke(false);

#if UNITY_EDITOR
                NutakuUnityEditorLogin.Show();
#else
                OpenApkLoginPageInBrowser();
#endif
            }
        }


        private static void ParseDeeplink(string uri)
        {
            Logger.Log("ParseDeeplink");
            if (string.IsNullOrEmpty(uri))
                return;
            if (_previousParsedIntentUri == uri)
            {
                Log("ParseDeeplink() ignoring uri because it was already processed: " + uri);
                return;
            }
            _previousParsedIntentUri = uri;
            Log("ParseDeeplink() uri: " + uri);
            if (!uri.Contains("callback/param?"))
            {
                Log("ParseDeeplink() ignoring uri because it is not for NutakuSDK: " + uri);
                return;
            }
            string deepLinkQueryString = uri.Split("callback/param?", 2, StringSplitOptions.None)[1];
            var queryParams = new Dictionary<string, string>();
            foreach (var joinedParams in deepLinkQueryString.Split('&'))
            {
                string[] paramSplit = joinedParams.Split('=', 2);
                if (!string.IsNullOrEmpty(paramSplit[0]))
                    queryParams.Add(paramSplit[0], (paramSplit.Length == 1 ? "" : paramSplit[1]));
            }

            if (queryParams.ContainsKey("payment_tentative_status") && queryParams.ContainsKey("paymentId"))
            {
                Log("ParseDeeplink() received payment event from browser for id " + queryParams["paymentId"] + " with status " + queryParams["payment_tentative_status"]);
                NutakuSdkConfig.paymentBrowserResultToGameCallbackDelegate.Invoke(queryParams["paymentId"], queryParams["payment_tentative_status"]);
            }
            else if (queryParams.ContainsKey("result"))
            {
                Log("ParseDeeplink() login details uri: " + uri);
                Logger.LogWarning(queryParams["result"]);
                //
                // string encoded = queryParams["result"];
                // string urlDecoded = WebUtility.UrlDecode(encoded);
                // byte[] bytes = Convert.FromBase64String(urlDecoded);
                // string decodedString = System.Text.Encoding.UTF8.GetString(bytes);
                
                string decodedString = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(queryParams["result"]));
                Log("ParseDeeplink() login details: " + decodedString);
                ParseLoginInfoFromBrowserIntent(decodedString);
            }
            else
                Log("ParseDeeplink() received a nutaku deeplink that is not recognized as login or payment related: " + uri);
        }


        private static void ParseLoginInfoFromBrowserIntent(string payload)
        {
            try
            {
                var result = JsonUtility.FromJson<NutakuUserProfile>(payload);
                if (result.id > 0 && !string.IsNullOrEmpty(result.nugt))
                {
                    NutakuCurrentUser.SaveUserInfo(result.nugt, result.id, result.nickname, result.test, result.grade,
                        result.language);
                    if (!_isN2i && result.n2i > 0)
                    {
                        _isN2i = true;
                        PlayerPrefs.SetInt("sdk_n2i", 1);
                    }

                    Log("ParseLoginInfoFromBrowserIntent() sending successful login result to game");
                    _successfulBrowserLoginIntent = true;
                    NutakuSdkConfig.loginResultToGameCallbackDelegate.Invoke(true);
                }
            }
            catch (Exception e)
            {
                Log("ParseLoginInfoFromBrowserIntent() failed to parse login info: " + e.Message);
            }
        }


        internal static string GI_Domain()
        {
            switch (NutakuSdkConfig.Environment)
            {
                case "TEST": return "https://test-sbox-gi.nutaku.com";
                case "SANDBOX": return "https://sbox-gi.nutaku.com";
                case "STAGE": return "https://stage-gi.nutaku.com";
                default: return "https://gi.nutaku.com";
            }
        }


        internal static string FE_Domain()
        {
            switch (NutakuSdkConfig.Environment)
            {
                case "TEST": return "https://test-sbox-www.nutaku.net";
                case "SANDBOX": return "https://sbox-www.nutaku.net";
                case "STAGE": return "https://stage-newtaku.nutaku.net";
                default: return "https://www.nutaku.net";
            }
        }


        internal static void Log(string message)
        {
            Debug.LogFormat(LogType.Log, LogOption.NoStacktrace, null, "[Nutaku SDK] {0}", message);
            //Debug.Log("[Nutaku SDK] " + message); // this one includes stack trace every time
        }
    }
}
#endif