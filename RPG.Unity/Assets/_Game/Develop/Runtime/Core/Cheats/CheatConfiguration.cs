using System;
using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.ContentControl.Type;

namespace PleasantlyGames.RPG.Runtime.Core.Cheats
{
    [Serializable]
    public class CheatConfiguration
    {
        public List<ContentType> UnlockContentTypes = new()
        {
            ContentType.StuffSlot,
            ContentType.CompanionSlot,
            ContentType.SkillSlot,
            ContentType.Lootbox,
            ContentType.UIElement,
            ContentType.SubMode,
            ContentType.PopupWindow,
            ContentType.Product
        };
    }
}