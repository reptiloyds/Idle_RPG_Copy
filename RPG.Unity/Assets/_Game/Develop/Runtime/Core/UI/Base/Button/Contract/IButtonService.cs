using System;
using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Button.Model.Base;

namespace PleasantlyGames.RPG.Runtime.Core.UI.Base.Button.Contract
{
    public interface IButtonService
    {
        event Action<BaseButton> OnButtonClick;
        event Action<string> OnButtonIdClick;
        event Action<BaseButton> OnButtonRegistered; 
        
        bool IsButtonInputBlocked { get; }
        
        void RegisterButton(BaseButton button);
        void UnregisterButton(BaseButton button);
        BaseButton GetButton(string id);
        void TriggerButtonClick(BaseButton button);
        void AppendAllowedButtonId(string id);
        void AppendAllowedButtonIds(List<string> ids);
        void RemoveAllowedButtonId(string id);
        void RemoveAllowedButtonIds(List<string> ids);
        bool IsAllowedButton(string id);
        bool IsButtonRegistered(string id);
    }
}