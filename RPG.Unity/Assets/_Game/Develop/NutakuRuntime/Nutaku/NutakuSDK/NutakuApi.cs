#if UNITY_ANDROID || UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;

namespace NutakuUnitySdk
{
    /// <summary>
    /// Class used for making Nutaku API calls.
    /// </summary>
    public static class NutakuApi
    {
        private const string APP_TYPE = "/android_app/";


        /// <summary>
        /// Allows checking if there is a newer Published version of this APK on the Nutaku Platform
        /// </summary>
        public static void CheckForNewerPublishedApk(
            MonoBehaviour myMonoBehaviour,
            NutakuUnityWebRequest.callbackFunctionDelegate callbackFunctionDelegate)
        {
            if (NutakuSdk._lastVersionCodeCheckEpoch + 3600 < DateTimeOffset.Now.ToUnixTimeSeconds())
            {
                NutakuSdk._lastVersionCodeCheckEpoch = DateTimeOffset.Now.ToUnixTimeSeconds();
                RawRequest(
                    "GET",
                    NutakuSdk.GI_Domain() + "/apk-info/" + NutakuSdkConfig.TitleId,
                    null,
                    null,
                    null,
                    myMonoBehaviour,
                    callbackFunctionDelegate,
                    null);
            }
            else
            {
                callbackFunctionDelegate(new NutakuApiRawResult() { responseCode = 200 });
            }
        }


        /// <summary>
        /// Parser function you must use in the callback for CheckForNewerPublishedApk(), if rawResult.statusCode is 2XX
        /// </summary>
        public static NutakuApkInfoResult Parse_CheckForNewerPublishedApk(NutakuApiRawResult apiResult)
        {
            if (apiResult.body == null)
                return new NutakuApkInfoResult { url = NutakuSdk._latestPublishedUpdateUrl, version = NutakuSdk._latestPublishedVersionCode, newerVersionAvailable = NutakuSdk._latestPublishedVersionCode > NutakuSdkConfig.RuntimeVersionCodeFromManifest };
            else
            {
                var response = ParseApiResponse<NutakuApkInfoResult>(apiResult);
                response.newerVersionAvailable = response.version > NutakuSdkConfig.RuntimeVersionCodeFromManifest;
                NutakuSdk._latestPublishedUpdateUrl = response.url;
                NutakuSdk._latestPublishedVersionCode = response.version;
                return response;
            }
        }


        /// <summary>
        /// If you are participating in the Nutaku Gold Subscription Partnership program, you can call this to find out if the user has an active Nutaku Gold Subscription
        /// </summary>
        public static void GetUserGoldSubscriptionStatus(
            MonoBehaviour myMonoBehaviour,
            NutakuUnityWebRequest.callbackFunctionDelegate callbackFunctionDelegate)
        {
            RawRequest(
                "GET",
                NutakuSdk.GI_Domain() + "/profile/" + NutakuSdkConfig.TitleId,
                new Dictionary<string, string>() { { "fields", "activeSub" } },
                new Dictionary<string, string>() { { "GI-NUGT", NutakuCurrentUser.GetNUGT() } },
                null,
                myMonoBehaviour,
                callbackFunctionDelegate,
                null);
        }


        /// <summary>
        /// Parser function you must use in the callback for GetUserGoldSubscriptionStatus(), if rawResult.statusCode is 2XX
        /// </summary>
        public static bool Parse_GetUserGoldSubscriptionStatus(NutakuApiRawResult apiResult)
        {
            return ParseApiResponse<NutakuUserProfile>(apiResult).activeSub == "1";
        }


        /// <summary>
        /// Used for the Game Handshake feature
        /// </summary>
        public static void GameHandshake(
            MonoBehaviour myMonoBehaviour,
            NutakuUnityWebRequest.callbackFunctionDelegate callbackFunctionDelegate)
        {
            RawRequest(
                "POST",
                NutakuSdk.GI_Domain() + "/game-handshake/" + NutakuSdkConfig.TitleId + APP_TYPE + NutakuCurrentUser.GetUserId(),
                null,
                new Dictionary<string, string>() { { "GI-NUGT", NutakuCurrentUser.GetNUGT() } },
                null,
                myMonoBehaviour,
                callbackFunctionDelegate,
                null);
        }


        /// <summary>
        /// Parser function you must use in the callback for GameHandshake(), if rawResult.statusCode is 2XX
        /// </summary>
        public static NutakuGameHandshakeResponse Parse_GameHandshake(NutakuApiRawResult apiResult)
        {
            return ParseApiResponse<NutakuGameHandshakeResponse>(apiResult);
        }


        /// <summary>
        /// The method used to initiate new in-game payments. The optional parameter forceAllowPutOnSandbox has no effect outside of sandbox
        /// </summary>
        public static void CreatePayment(
            NutakuPayment payment,
            MonoBehaviour myMonoBehaviour,
            NutakuUnityWebRequest.callbackFunctionDelegate callbackFunctionDelegate,
            bool forceAllowPutOnSandbox = false)
        {
            var queryParams = new Dictionary<string, string>();
            if (NutakuSdk.IsN2I())
                queryParams.Add("n2i", "1");
            if (forceAllowPutOnSandbox && NutakuSdkConfig.Environment != "PRODUCTION")
                queryParams.Add("allowPut", "1");

            RawRequest(
                "POST",
                NutakuSdk.GI_Domain() + "/payment/" + NutakuSdkConfig.TitleId + APP_TYPE + NutakuCurrentUser.GetUserId(),
                queryParams,
                new Dictionary<string, string>() { { "GI-NUGT", NutakuCurrentUser.GetNUGT() } },
                JsonUtility.ToJson(payment),
                myMonoBehaviour,
                callbackFunctionDelegate,
                null);
        }


        /// <summary>
        /// Parser function you must use in the callback for CreatePayment(), if rawResult.statusCode is 2XX
        /// </summary>
        public static NutakuPayment Parse_CreatePayment(NutakuApiRawResult apiResult)
        {
            return ParseApiResponse<NutakuPayment>(apiResult);
        }


        /// <summary>
        /// Used to complete payments entirely via API.
        /// Usable only on payments which have received status=put at the time of creation. Otherwise, this API call will error out.
        /// </summary>
        public static void PutPayment(
            string paymentId,
            MonoBehaviour myMonoBehaviour,
            NutakuUnityWebRequest.callbackFunctionDelegate callbackFunctionDelegate)
        {
            RawRequest(
                "PUT",
                NutakuSdk.GI_Domain() + "/payment/" + NutakuSdkConfig.TitleId + APP_TYPE + NutakuCurrentUser.GetUserId() + "/" + paymentId,
                (NutakuSdk.IsN2I() ? new Dictionary<string, string>() { { "n2i", "1" } } : null),
                new Dictionary<string, string>() { { "GI-NUGT", NutakuCurrentUser.GetNUGT() } },
                "",
                myMonoBehaviour,
                callbackFunctionDelegate,
                paymentId);
        }


#if UNITY_EDITOR
        internal static void ManualLoginForUnityEditor(
            string email,
            string password,
            MonoBehaviour myMonoBehaviour,
            NutakuUnityWebRequest.callbackFunctionDelegate callbackFunctionDelegate)
        {
            RawRequest(
                "POST",
                 NutakuSdk.GI_Domain() + "/login/" + NutakuSdkConfig.TitleId + APP_TYPE,
                null,
                null,
                "{\"e\":\"" + email + "\",\"p\":\"" + password + "\"}",
                myMonoBehaviour,
                callbackFunctionDelegate,
                null);
        }
#endif


        internal static void AutoLogin(
            int userId,
            string nugt,
            MonoBehaviour myMonoBehaviour,
            NutakuUnityWebRequest.callbackFunctionDelegate callbackFunctionDelegate)
        {
            RawRequest(
                "POST",
                 NutakuSdk.GI_Domain() + "/login/" + NutakuSdkConfig.TitleId + APP_TYPE + userId,
                (NutakuSdk.IsN2I() ? new Dictionary<string, string>() { { "n2i", "1" } } : null),
                new Dictionary<string, string>(){ { "GI-NUGT", nugt } },
                null,
                myMonoBehaviour,
                callbackFunctionDelegate,
                null);
        }


        internal static TResult ParseApiResponse<TResult>(NutakuApiRawResult apiResult)
        {
            try
            {
                return JsonUtility.FromJson<TResult>(apiResult.body);
            }
            catch (Exception ex)
            {
                NutakuSdk.Log("ParseApiResponse() Error: " + ex.StackTrace);
                throw ex;
            }
        }


        private static void RawRequest(
            string method,
            string url,
            Dictionary<string, string> queryParams,
            Dictionary<string, string> headers,
            string body,
            MonoBehaviour myMonoBehaviour,
            NutakuUnityWebRequest.callbackFunctionDelegate callbackFunctionDelegate,
            string correlationId)
        {
            method = method.ToUpperInvariant();
            var queryParamStringList = new List<string>();

            if (queryParams != null)
                foreach (var kvp in queryParams)
                    if (kvp.Key.Length != 0)
                        queryParamStringList.Add(Uri.EscapeDataString(kvp.Key) + "=" + Uri.EscapeDataString(kvp.Value ?? ""));

            var builder = new UriBuilder(url) { Query = string.Join("&", queryParamStringList.ToArray()) };
            var uri = builder.Uri;

            if (headers == null)
                headers = new Dictionary<string, string>();
            headers.Add("Content-Type", "application/json");
            headers.Add("User-Agent", NutakuSdkConfig.TitleId + " Nutaku Unity SDK " + NutakuSdk.SdkVersion);

            LogApiRequest(method, uri, body);
            
            new NutakuUnityWebRequest().StartSendingRawRequest(method, myMonoBehaviour, headers, uri.ToString(), body ?? "", callbackFunctionDelegate, correlationId);
        }


        //[Conditional("DEBUG")]
        private static void LogApiRequest(string method, Uri uri, string body)
        {
            NutakuSdk.Log(string.Format("Nutaku API Request: {0} {1}\n{2}", method, uri.ToString(), body ?? "No Request Body"));
        }


        //[Conditional("DEBUG")]
        private static void LogApiResponse(HttpWebResponse res)
        {
            if (res.ContentLength != 0)
            {
                string content;
                using (var reader = new StreamReader(res.GetResponseStream()))
                {
                    content = reader.ReadToEnd();
                }

                NutakuSdk.Log("## Nutaku API Response: Http Code: " + (int)res.StatusCode + ". Body:\n" + content);
            }
            else
                NutakuSdk.Log("## Nutaku API Response: Http Code: " + (int)res.StatusCode + ". No response body.");
        }
    }
}
#endif