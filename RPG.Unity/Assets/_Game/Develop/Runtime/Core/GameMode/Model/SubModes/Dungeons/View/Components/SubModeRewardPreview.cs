using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.Model;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace PleasantlyGames.RPG.Runtime.Core.GameMode.Model.SubModes.Dungeons.View.Components
{
    [DisallowMultipleComponent, HideMonoScript]
    public class SubModeRewardPreview : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private List<Graphic> _graphics;

        public void Redraw(SubMode subMode)
        {
            _image.sprite = subMode.RewardImage;
            foreach (var graphic in _graphics) 
                graphic.color = subMode.MainColor;
        }
    }
}