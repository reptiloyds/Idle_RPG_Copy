using System;
using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.Resource.View.FlyResource.Model;
using PleasantlyGames.RPG.Runtime.Core.Resource.View.FlyResource.Type;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.Resource.View.FlyResource.Contracts
{
    public interface IPopupResourceFactory
    {
        void Setup(List<PopupResourceSettings> contexts, UnityEngine.Camera camera);

        void SpawnFromWorldPosition(Vector3 worldPosition, Sprite sprite, BigDouble.Runtime.BigDouble trueAmount, int viewAmount,
            Transform target, ResourceView resourceView = null, bool decreaseTotalValue = false, PopupIconEffect effect = PopupIconEffect.None,
            PopupIconContext context = PopupIconContext.Gameplay, Action onComplete = null);

        public void SpawnFromUI(Vector3 popupPosition, Sprite sprite, BigDouble.Runtime.BigDouble trueAmount, int viewAmount,
            Transform target, ResourceView resourceView = null, bool decreaseTotalValue = false, PopupIconEffect effect = PopupIconEffect.None,
            PopupIconContext context = PopupIconContext.Gameplay, Action onComplete = null);
    }   
}
