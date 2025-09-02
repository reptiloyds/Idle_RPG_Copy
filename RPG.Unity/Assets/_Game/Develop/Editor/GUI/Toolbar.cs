using System.Collections.Generic;
using System.Linq;
using PleasantlyGames.RPG.Runtime.Save.Models;
using PleasantlyGames.RPG.Runtime.UnityExtension.Extensions;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityToolbarExtender;

namespace Editor.GUI
{
	[InitializeOnLoad]
    public class Toolbar
    {
	    private const string ProdDefine = "RPG_PROD";
	    private const string DevDefine = "RPG_DEV";
	    
	    private const string GooglePlatform = "P_GOOGLE";
	    private const string NutakuPlatform = "P_NUTAKU";
	    private const string YandexPlatform = "P_YANDEX";
	    
	    private const string DisableSaveDefine = "SAVE_DISABLED";
	    private const string DisableLogDefine = "LOG_DISABLED";
	    private const string DisableTutorialDefine = "TUTORIAL_DISABLED";
	    private const string DisableCheatsDefine = "CHEATS_DISABLED";
	    private const string DisableStoryDefine = "STORY_DISABLED";
	    
	    private const string DefinitionPath = "Assets/_Game/Definitions/";
	    private static string ApplicationPath => DefinitionPath + "Application/";
	    private static string BalancePath => DefinitionPath + "Balance/";
	    private const string ParserFile = "EditorBalanceParser.asset";
	    private const string GameConfigFile = "GameConfig.asset";
	    private const string SnapshotConfigFile = "SnapshotConfig.asset";
	    private const string LocalizationPath = "Assets/Resources/I2Languages.asset";
	    private const string DefaultBackgroundColor = "3C3C3C";

	    static Toolbar ()
        {
            ToolbarExtender.LeftToolbarGUI.Add(OnToolbarGUILeft);
            ToolbarExtender.RightToolbarGUI.Add(OnToolbarGUIRight);
        }
        
        static void OnToolbarGUILeft ()
		{
			GUILayout.FlexibleSpace();
			
			if (GUILayout.Button(new GUIContent("Parser", "Select Parser"), ToolbarStyles.commandButtonStyle))
				SelectParser();
			
			if (GUILayout.Button(new GUIContent("Config", "Select GameConfig"), ToolbarStyles.commandButtonStyle))
				SelectGameConfig();
			
			if (GUILayout.Button(new GUIContent("Snapshot", "Select SnapshotConfig"), ToolbarStyles.longCommandButtonStyle))
				SelectSnapshotConfig();
			
			if (GUILayout.Button(new GUIContent("Localization", "Select Localization"), ToolbarStyles.veryLongCommandButtonStyle))
				SelectLocalization();
			
			if (GUILayout.Button(new GUIContent("Update", "Update this scene"), ToolbarStyles.commandButtonStyle))
				UpdateThisScene();
		}
        
		static void OnToolbarGUIRight ()
		{
			if (Application.isPlaying)
			{
				UnityEngine.GUI.enabled = false;
			}

			var defaultBackgroundColor = UnityEngine.GUI.backgroundColor;

			HandleClearData();
			
			HandleMainDefine();
			if (HasDefine(DevDefine))
			{
				HandleCheats();
				HandleSaves();
				HandleTutorial();
				HandleStory();
				HandleLog();	
			}
			
			UnityEngine.GUI.backgroundColor = Color.white;
			if (HasDefine(GooglePlatform) &&
			    GUILayout.Button(new GUIContent(GooglePlatform, "Google Play platform"),
				    ToolbarStyles.veryLongCommandButtonStyle)) ;
			if (HasDefine(NutakuPlatform) &&
			    GUILayout.Button(new GUIContent(NutakuPlatform, "Nutaku platform"),
				    ToolbarStyles.veryLongCommandButtonStyle)) ;
			if (HasDefine(YandexPlatform) &&
			    GUILayout.Button(new GUIContent(YandexPlatform, "Yandex platform"),
				    ToolbarStyles.veryLongCommandButtonStyle)) ;
			
			UnityEngine.GUI.backgroundColor = defaultBackgroundColor;
			GUILayout.FlexibleSpace();
			UnityEngine.GUI.enabled = true;
		}

		private static void HandleClearData()
		{
			UnityEngine.GUI.backgroundColor = new Color(1f, 0.5f, 0f);
			if (GUILayout.Button(new GUIContent("Clear Data", "Clear Game Data"), ToolbarStyles.longCommandButtonStyle) && EditorUtility.DisplayDialog("ClearData", "Clear Game Data?", "Yes", "No"))
				ClearData();
		}

		private static void HandleMainDefine()
		{
			if (!HasDefine(ProdDefine) && !HasDefine(DevDefine)) 
				Replace(ProdDefine, DevDefine);
			else if (HasDefine(ProdDefine))
			{
				UnityEngine.GUI.backgroundColor = Color.cyan;
				if(GUILayout.Button(new GUIContent("PROD", "Switch to DEV"), ToolbarStyles.commandButtonStyle))
					Replace(ProdDefine, DevDefine);
			}
			else if (HasDefine(DevDefine))
			{
				UnityEngine.GUI.backgroundColor = Color.cyan;
				if(GUILayout.Button(new GUIContent("DEV", "Switch to PROD"), ToolbarStyles.commandButtonStyle))
					Replace(DevDefine, ProdDefine);
			}
		}

		private static void HandleCheats()
		{
			var isCheatsDisabled = HasDefine(DisableCheatsDefine);
			UnityEngine.GUI.backgroundColor = isCheatsDisabled ? Color.red : Color.green;
			if(GUILayout.Button(new GUIContent("CHEATS", "Switch cheat functionality"), ToolbarStyles.commandButtonStyle))
				ToggleDefine(DisableCheatsDefine);
		}

		private static void HandleSaves()
		{
			var isSaveDisabled = HasDefine(DisableSaveDefine);
			UnityEngine.GUI.backgroundColor = isSaveDisabled ? Color.red : Color.green;
			if(GUILayout.Button(new GUIContent("SAVES", "Switch save functionality"), ToolbarStyles.commandButtonStyle))
				ToggleDefine(DisableSaveDefine);
		}

		private static void HandleTutorial()
		{
			var isTutorialDisabled = HasDefine(DisableTutorialDefine);
			UnityEngine.GUI.backgroundColor = isTutorialDisabled ? Color.red : Color.green;
			if(GUILayout.Button(new GUIContent("TUTOR", "Switch tutorial functionality"), ToolbarStyles.commandButtonStyle))
				ToggleDefine(DisableTutorialDefine);
		}

		private static void HandleStory()
		{
			var isStoryDisabled = HasDefine(DisableStoryDefine);
			UnityEngine.GUI.backgroundColor = isStoryDisabled ? Color.red : Color.green;
			if(GUILayout.Button(new GUIContent("STORY", "Switch story functionality"), ToolbarStyles.commandButtonStyle))
				ToggleDefine(DisableStoryDefine);
		}
		
		private static void HandleLog()
		{
			var isDebugDisabled = HasDefine(DisableLogDefine);
			UnityEngine.GUI.backgroundColor = isDebugDisabled ? Color.red : Color.green;
			if(GUILayout.Button(new GUIContent("LOG", "Switch log functionality"), ToolbarStyles.commandButtonStyle))
				ToggleDefine(DisableLogDefine);
		}
		
		private static Color HexToColor(string hex)
		{
			byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
			byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
			byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);

			return new Color32(r, g, b, 255);
		}

		private static bool HasDefine(string define)
		{
			var buildTargetGroup = BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget);
			var currentSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);

			return currentSymbols.Contains(define);
		}

		private static void ToggleDefine(string define)
		{
			BuildTargetGroup buildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
			List<string> currentSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup).Split(';').ToList();
			if (currentSymbols.Contains(define))
				currentSymbols.Remove(define);
			else
				currentSymbols.Add(define);
			
			PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, string.Join(";", currentSymbols));
		}

		private static void Replace(string oldDefine, string newDefine)
		{
			BuildTargetGroup buildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
			List<string> currentSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup).Split(';').ToList();
			if (currentSymbols.Contains(oldDefine))
				currentSymbols.Remove(oldDefine);
			if(!currentSymbols.Contains(newDefine))
				currentSymbols.Add(newDefine);
			
			PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, string.Join(";", currentSymbols));
		}
		
		private static void SelectParser()
		{
			Object asset = AssetDatabase.LoadAssetAtPath<Object>(BalancePath + ParserFile);
			Selection.activeObject = asset;
			EditorGUIUtility.PingObject(Selection.activeObject);
		}

		private static void SelectGameConfig()
		{
			Object asset = AssetDatabase.LoadAssetAtPath<Object>(ApplicationPath + GameConfigFile);
			Selection.activeObject = asset;
			EditorGUIUtility.PingObject(Selection.activeObject);
		}
		
		private static void SelectSnapshotConfig()
		{
			Object asset = AssetDatabase.LoadAssetAtPath<Object>(ApplicationPath + SnapshotConfigFile);
			Selection.activeObject = asset;
			EditorGUIUtility.PingObject(Selection.activeObject);
		}

		private static void SelectLocalization()
		{
			Object asset = AssetDatabase.LoadAssetAtPath<Object>(LocalizationPath);
			Selection.activeObject = asset;
			EditorGUIUtility.PingObject(Selection.activeObject);
		}
		
		private static void ClearData()
		{
			PlayerPrefs.DeleteKey(BaseDataRepository.PlayerProfileKey);
			PlayerPrefs.SetInt(BaseDataRepository.ClearDataKey, 1);
			PlayerPrefs.Save();
		}

		private static void UpdateThisScene()
		{
			var scene = SceneManager.GetActiveScene();
			scene.UpdateAllSceneHasher();
		}
    }
    
    static class ToolbarStyles
    {
	    public static readonly GUIStyle commandButtonStyle;
	    public static readonly GUIStyle longCommandButtonStyle;
	    public static readonly GUIStyle veryLongCommandButtonStyle;

	    static ToolbarStyles ()
	    {
		    commandButtonStyle = new GUIStyle("ToolbarButton")
		    {
			    fontSize = 12,
			    alignment = TextAnchor.MiddleCenter,
			    imagePosition = ImagePosition.ImageAbove,
			    fixedWidth = 60,
			    fixedHeight = 18,
		    };

		    longCommandButtonStyle = new GUIStyle("ToolbarButton")
		    {
			    fontSize = 12,
			    alignment = TextAnchor.MiddleCenter,
			    imagePosition = ImagePosition.ImageAbove,
			    fixedWidth = 70,
			    fixedHeight = 18,
		    };

		    veryLongCommandButtonStyle = new GUIStyle("ToolbarButton")
		    {
			    fontSize = 12,
			    alignment = TextAnchor.MiddleCenter,
			    imagePosition = ImagePosition.ImageAbove,
			    fixedWidth = 90,
			    fixedHeight = 18,
		    };
	    }
    }
}
