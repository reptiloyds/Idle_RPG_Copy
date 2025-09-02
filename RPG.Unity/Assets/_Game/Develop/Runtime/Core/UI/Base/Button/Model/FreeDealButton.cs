using PleasantlyGames.RPG.Runtime.Core.UI.Base.Button.Model.Base;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.UI.Base.Button.Model
{
    [DisallowMultipleComponent]
    public class FreeDealButton : BaseDealButton
    {
        protected override void Click()
        {
            base.Click();
        
            CompleteDeal();
        }
    }
}
