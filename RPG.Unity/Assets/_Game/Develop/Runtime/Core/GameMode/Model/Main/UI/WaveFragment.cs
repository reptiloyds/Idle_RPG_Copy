using System.Collections.Generic;
using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace PleasantlyGames.RPG.Runtime.Core.GameMode.Model.Main.UI
{
    [DisallowMultipleComponent, HideMonoScript]
    public class WaveFragment : MonoBehaviour
    {
        [SerializeField, Required] private Image _image;
        [SerializeField, Required] private Image _smallStick;
        [SerializeField, Required] private Image _largeStick;
        [SerializeField, MinValue(0), MaxValue(1)] private float _stickPercent = 0.1f;
        
        [SerializeField, Required] private GameObject _fullOutline;
        [SerializeField, Required] private List<GameObject> _cutOutlines;
        [SerializeField, Required] private List<GameObject> _smallStickObjects;
        [SerializeField, Required] private List<GameObject> _largeStickObjects;
        
        public Tween FillTween { get; private set; }
        public Tween FillStickTween { get; private set; }
        
        private Image _stick;

        public Vector3 StickPosition => _stick.transform.position;
        
        public void Fill(float time, Ease ease)
        {
            FillTween.Stop();
            FillTween = Tween.UIFillAmount(_image, 1, time * (1 - _stickPercent), ease);
        }

        public void FillStick(float time, Ease ease)
        {
            FillStickTween.Stop();
            if (time == 0)
                _stick.fillAmount = 1;
            else
                FillStickTween = Tween.UIFillAmount(_stick, 1, time * _stickPercent, ease);
        }

        public void Clear()
        {
            FillTween.Stop();
            FillStickTween.Stop();
            _image.fillAmount = 0;
            _smallStick.fillAmount = 0;
            _largeStick.fillAmount = 0;
        }

        public void EnableFullOutline()
        {
            _fullOutline.SetActive(true);
            foreach (var cutOutline in _cutOutlines) 
                cutOutline.SetActive(false);
        }

        public void DisableFullOutline() => 
            _fullOutline.SetActive(false);

        public void EnableLargeStick()
        {
            _stick = _largeStick;
            
            foreach (var smallStick in _smallStickObjects) 
                smallStick.SetActive(false);

            foreach (var largeStick in _largeStickObjects) 
                largeStick.SetActive(true);
        }

        public void EnableSmallStick()
        {
            _stick = _smallStick;
            
            foreach (var smallStick in _smallStickObjects) 
                smallStick.SetActive(true);

            foreach (var largeStick in _largeStickObjects) 
                largeStick.SetActive(false);
        }
    }
}