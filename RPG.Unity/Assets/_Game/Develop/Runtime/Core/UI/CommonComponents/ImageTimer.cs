using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace PleasantlyGames.RPG.Runtime.Core.UI.CommonComponents
{
    [DisallowMultipleComponent, HideMonoScript]
    public class ImageTimer : UITimer
    {
        [SerializeField] private Image _image;
        [SerializeField] private bool _fromZero = false;

        public override void UpdateValue(float leftSeconds)
        {
            if (_fromZero)
                _image.fillAmount = 1 - leftSeconds / StartValue;
            else
                _image.fillAmount = leftSeconds / StartValue;
        }
    }
}