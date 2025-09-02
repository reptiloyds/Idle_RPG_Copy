#if UNITY_ANDROID || UNITY_EDITOR
namespace NutakuUnitySdk
{
    /// <summary>
	/// Structure that stores the result of NutakuApi.CheckForNewerPublishedApk()
    /// </summary>
    public struct NutakuApkInfoResult
    {
        /// <summary> If this is true, it means a newer published version is available on the Nutaku website </summary>
        public bool newerVersionAvailable;

        /// <summary> The URL where the user can grab the latest published APK for your game </summary>
        public string url;

        /// <summary> The version of the newest published apk </summary>
        public int version;
    }
}
#endif