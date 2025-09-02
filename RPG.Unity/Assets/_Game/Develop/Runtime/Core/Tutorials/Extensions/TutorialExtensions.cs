using System;
using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.Balance.Json;
using PleasantlyGames.RPG.Runtime.Core.Resource.Type;
using PleasantlyGames.RPG.Runtime.Core.Tutorials.Model.Steps.CompleteConditions.Quest;
using PleasantlyGames.RPG.Runtime.Core.Tutorials.Model.Steps.CompleteConditions.Timer;
using PleasantlyGames.RPG.Runtime.Core.Tutorials.Model.Steps.Variants.Accent;
using PleasantlyGames.RPG.Runtime.Core.Tutorials.Model.Steps.Variants.AppendResource;
using PleasantlyGames.RPG.Runtime.Core.Tutorials.Model.Steps.Variants.CloseWindow;
using PleasantlyGames.RPG.Runtime.Core.Tutorials.Model.Steps.Variants.Monologue;
using PleasantlyGames.RPG.Runtime.Core.Tutorials.Model.Steps.Variants.OpenWindow;
using PleasantlyGames.RPG.Runtime.Core.Tutorials.Model.Steps.Variants.PlayAnimation;
using PleasantlyGames.RPG.Runtime.Core.Tutorials.Model.Steps.Variants.Pointer;
using PleasantlyGames.RPG.Runtime.Core.Tutorials.Model.Steps.Variants.SwitchBranch;
using PleasantlyGames.RPG.Runtime.Core.Tutorials.Model.Triggers.Button;
using PleasantlyGames.RPG.Runtime.Core.Tutorials.Model.Triggers.Content;
using PleasantlyGames.RPG.Runtime.Core.Tutorials.Model.Triggers.MainMode;
using PleasantlyGames.RPG.Runtime.Core.Tutorials.Model.Triggers.Quest;
using PleasantlyGames.RPG.Runtime.Core.Tutorials.Type;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.Tutorials.Extensions
{
    public static class TutorialExtensions
    {
        public static void TryDeserialize(this TutorialTriggerType type, string json)
        {
            switch (type)
            {
                case TutorialTriggerType.QuestStart:
                case TutorialTriggerType.QuestComplete:
                case TutorialTriggerType.QuestCollect:
                    var questTriggerData = JsonConvertLog.DeserializeObject<QuestTutorialData>(json);
                    if (questTriggerData.QuestId < 0)
                        Debug.LogError("Invalid quest id");
                    break;
                case TutorialTriggerType.MainModeEnter:
                    var locationTriggerData = JsonConvertLog.DeserializeObject<MainModeEnterData>(json);
                    if (locationTriggerData.Id < 0 || locationTriggerData.Level < 1)
                        Debug.LogError("Invalid location id or level");
                    break;
                case TutorialTriggerType.Button:
                    var buttonTriggerData = JsonConvertLog.DeserializeObject<ButtonTutorialData>(json);
                    if (string.IsNullOrWhiteSpace(buttonTriggerData.ButtonId))
                        Debug.LogError("Invalid button id");
                    break;
                case TutorialTriggerType.MainModeBoseLose:
                    break;
                case TutorialTriggerType.ContentUnlock:
                case TutorialTriggerType.ContentReadyManualUnlock:
                    var contentData = JsonConvertLog.DeserializeObject<ContentTutorialData>(json);
                    if (string.IsNullOrWhiteSpace(contentData.Id))
                        Debug.LogError("Invalid content id");
                    break;
            }
        }

        public static void TryDeserialize(this TutorialStepType type, string json)
        {
            switch (type)
            {
                case TutorialStepType.Monologue:
                    var monologueStepData = JsonConvertLog.DeserializeObject<MonologueTutorialData>(json);
                    if (string.IsNullOrWhiteSpace(monologueStepData.MessageToken))
                        Debug.LogError("Invalid MessageToken");
                    if (string.IsNullOrWhiteSpace(monologueStepData.Position))
                        Debug.LogWarning("Invalid message Position");
                    break;
                case TutorialStepType.Pointer:
                    var pointerStepData = JsonConvertLog.DeserializeObject<PointerTutorialData>(json);
                    if (string.IsNullOrWhiteSpace(pointerStepData.ButtonId))
                        Debug.LogError("Invalid button id");
                    break;
                case TutorialStepType.Accent:
                    var accentStepData = JsonConvertLog.DeserializeObject<AccentTutorialData>(json);
                    if (string.IsNullOrWhiteSpace(accentStepData.ButtonId))
                        Debug.LogError("Invalid button id");
                    break;
                case TutorialStepType.AppendResource:
                    var appendResourceData = JsonConvertLog.DeserializeObject<AppendResourceData>(json);
                    if (appendResourceData.ResourceType == ResourceType.None)
                        Debug.LogError($"{typeof(ResourceType)} == None");
                    break;
                case TutorialStepType.PlayAnimation:
                    var playAnimationData = JsonConvertLog.DeserializeObject<PlayAnimationData>(json);
                    if (string.IsNullOrWhiteSpace(playAnimationData.ButtonId))
                        Debug.LogError("Invalid button id");
                    break;
                case TutorialStepType.OpenWindow:
                    var openWindowData = JsonConvertLog.DeserializeObject<OpenWindowData>(json);
                    if (string.IsNullOrWhiteSpace(openWindowData.WindowId))
                        Debug.LogError("Invalid window id");
                    break;
                case TutorialStepType.CloseWindow:
                    var closeWindowData = JsonConvertLog.DeserializeObject<CloseWindowData>(json);
                    if (string.IsNullOrWhiteSpace(closeWindowData.WindowId))
                        Debug.LogError("Invalid window id");
                    break;
                case TutorialStepType.SwitchBranch:
                    var switchBranchData = JsonConvertLog.DeserializeObject<SwitchBranchTutorialData>(json);
                    if (string.IsNullOrWhiteSpace(switchBranchData.BranchId))
                        Debug.LogError("Invalid branch id");
                    break;
            }
        }

        public static void TryDeserialize(this TutorialCompleteCondition type, string json)
        {
            switch (type)
            {
                case TutorialCompleteCondition.Button:
                    var buttonCompleteConditionData = JsonConvertLog.DeserializeObject<HashSet<string>>(json);
                    if (buttonCompleteConditionData.Count == 0)
                        Debug.LogError("ButtonId`s count must be > 0");
                    break;
                case TutorialCompleteCondition.QuestComplete:
                    var questCompleteConditionData = JsonConvertLog.DeserializeObject<QuestCollectConditionData>(json);
                    if (questCompleteConditionData.QuestId < 0)
                        Debug.LogError("Invalid quest id");
                    break;
                case TutorialCompleteCondition.Timer:
                    var timerCompleteConditionData = JsonConvertLog.DeserializeObject<TimerConditionData>(json);
                    if (timerCompleteConditionData.Duration < 0)
                        Debug.LogError("Invalid timer time");
                    break;
                case TutorialCompleteCondition.ChatComplete:
                    if (String.IsNullOrEmpty(json))
                        Debug.LogError("Invalid chat message");
                    break;
                case TutorialCompleteCondition.ShowChatAnswers:
                    if (String.IsNullOrEmpty(json))
                        Debug.LogError("Invalid chat message");
                    break;
            }
        }
    }
}