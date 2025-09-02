using System;
using PleasantlyGames.RPG.Runtime.UnityExtension;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.UI.RouletteWheel 
{
   [Serializable]
   public class WheelPieceData : IWeightedRandom
   {
      public Sprite Sprite;
      public int Amount;
      public int Weight = 1;

      [HideInInspector] public int Index;
      public int RandomWeight => Weight;
   }
}
