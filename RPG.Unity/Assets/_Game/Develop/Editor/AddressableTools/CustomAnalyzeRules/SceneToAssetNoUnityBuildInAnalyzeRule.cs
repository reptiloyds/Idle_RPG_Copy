using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.AddressableAssets.Build;
using UnityEditor.AddressableAssets.Build.AnalyzeRules;
using UnityEditor.AddressableAssets.Settings;

namespace PleasantlyGames.IdleRPG.Editor.AddressableTools.CustomAnalyzeRules
{
    public class SceneToAssetNoUnityBuildInAnalyzeRule : CheckSceneDupeDependencies
    {
        public override string ruleName => "Check Scene to Addressable No Unity BuildIn";

        public override List<AnalyzeResult> RefreshAnalysis(AddressableAssetSettings settings)
        {
            List<AnalyzeResult> results = base.RefreshAnalysis(settings)
                .Where(x => !x.resultName.Contains("unity_buitin_extra"))
                .ToList();

            return results;
        }
        
        [InitializeOnLoad]
        class RegisterSceneToAssetNoUnityBuildInAnalyzeRule
        {
            static RegisterSceneToAssetNoUnityBuildInAnalyzeRule() => 
                AnalyzeSystem.RegisterNewRule<SceneToAssetNoUnityBuildInAnalyzeRule>();
        }
    }
}