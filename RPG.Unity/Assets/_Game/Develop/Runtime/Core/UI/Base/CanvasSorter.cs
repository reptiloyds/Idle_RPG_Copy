using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.UI.Base
{
    [HideMonoScript, DisallowMultipleComponent]
    public class CanvasSorter : MonoBehaviour
    {
        [SerializeField, ReadOnly] private List<Canvas> _canvasList = new();

        private void OnValidate() => 
            UpdateCanvasOrders();

        [Button]
        private void UpdateCanvasOrders()
        {
            _canvasList = GetComponentsInChildren<Canvas>().ToList();

            for (var i = 0; i < _canvasList.Count; i++) 
                _canvasList[i].sortingOrder = i;   
        }
    }
}