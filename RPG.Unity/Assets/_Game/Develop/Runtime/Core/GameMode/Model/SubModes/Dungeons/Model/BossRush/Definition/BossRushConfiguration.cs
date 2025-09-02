using System;
using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Model.Type;
using PleasantlyGames.RPG.Runtime.Core.Resource.Type;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.Dungeons.Model.BossRush.Definition
{
    [Serializable]
    public class BossRushConfiguration
    {
        public List<SquadActionType> ActionAfterAllyWin;
        [MinValue(0)] public float LocationSpeed = 5;
        [MinValue(0)] public float BonusUnitSpeed = 5;
        [MinValue(0)] public float SwitchWaveDuration = 0.75f;
        public Vector2 BossHealthBarSize = new Vector2(220, 45);
        [MinValue(0)] public float AutoLoseTimeInSeconds = 45f;
        public ResourceType RewardType = ResourceType.Hard;
        public Color RewardColor;
        public SubModeSetup SubModeSetup;
    }
}