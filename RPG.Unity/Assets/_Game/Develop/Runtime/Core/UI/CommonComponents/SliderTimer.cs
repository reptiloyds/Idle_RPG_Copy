using UnityEngine;
using UnityEngine.UI;

namespace PleasantlyGames.RPG.Runtime.Core.UI.CommonComponents
{
    public class SliderTimer : UITimer
    {
        [SerializeField] private Slider _slider;
        [SerializeField] private bool _fromZero = false;
        
        public override void UpdateValue(float leftSeconds)
        {
            if (_fromZero)
                _slider.value = 1 - leftSeconds / StartValue;
            else
                _slider.value = leftSeconds / StartValue;
        }
    }
}