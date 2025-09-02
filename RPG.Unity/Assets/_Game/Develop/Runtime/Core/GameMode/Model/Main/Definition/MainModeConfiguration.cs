using System;
using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Model.Type;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.GameMode.Model.Main.Definition
{
    [Serializable]
    public class MainModeConfiguration
    {
        public List<SquadActionType> ActionAfterAllyWin;
        [MinValue(0)] public float LocationSpeed = 5;
        [MinValue(0)] public float BonusUnitSpeed = 5;
        [MinValue(0)] public float SwitchWaveDuration = 0.75f;
        [MinValue(0)] public float SwitchWaveDurationUI = 0.5f;
        public Vector2 BossHealthBarSize = new Vector2(200, 55);
    }
}