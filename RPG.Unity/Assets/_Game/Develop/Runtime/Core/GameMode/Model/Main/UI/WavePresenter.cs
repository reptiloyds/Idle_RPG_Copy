using System.Collections.Generic;
using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.GameMode.Model.Main.UI
{
    [DisallowMultipleComponent, HideMonoScript]
    public class WavePresenter : MonoBehaviour
    {
        [SerializeField, Required] private Transform _parent;
        [SerializeField, Required] private WaveFragment _waveFragmentTemplate;
        [SerializeField, Required] private GameObject _bossFragment;
        [SerializeField, Required] private Transform _pointer;
        [SerializeField] private Ease _fillEase = Ease.Linear;

        private readonly List<WaveFragment> _waveFragments = new();

        private Tween _pointerTween;
        private int _resetPointerOnNextFrame;

        public void Enable(int waveAmount, bool withBoss)
        {
            gameObject.SetActive(true);

            if (waveAmount > _waveFragments.Count) 
                SpawnFragments(waveAmount - _waveFragments.Count);

            for (var i = 0; i < _waveFragments.Count; i++)
            {
                var waveFragment = _waveFragments[i];
                waveFragment.gameObject.SetActive(i < waveAmount);
                waveFragment.Clear();

                if (i == 0)
                    waveFragment.EnableFullOutline();
                else
                    waveFragment.DisableFullOutline();

                if (i % 2 == 1)
                    waveFragment.EnableSmallStick();
                else
                    waveFragment.EnableLargeStick();
            }
            
            _waveFragments[0].FillStick(0, _fillEase);
            _resetPointerOnNextFrame = 5;
            
            _bossFragment.transform.SetSiblingIndex(_waveFragments.Count);
            _bossFragment.gameObject.SetActive(withBoss);
        }

        private void Update()
        {
            if(_resetPointerOnNextFrame <= 0) return;
            _resetPointerOnNextFrame--;
            if(_resetPointerOnNextFrame == 0)
                MovePointer(_waveFragments[0].StickPosition, 0);
        }
        

        public void Disable()
        {
            gameObject.SetActive(false);
            foreach (var waveFragment in _waveFragments) 
                waveFragment.Clear();
        }

        public void Fill(int wave, float time)
        {
            if(_waveFragments.Count - 1 < wave) return;
            
            _waveFragments[wave].Fill(time, _fillEase);
            
            var nextWave = wave + 1;
            if(_waveFragments.Count < nextWave) return;
            if (nextWave >= _waveFragments.Count)
            {
                MovePointer(_bossFragment.transform.position, time);
                return;
            }

            MovePointer(_waveFragments[nextWave].StickPosition, time);
            
            _waveFragments[wave].FillTween.OnComplete(() => _waveFragments[nextWave].FillStick(time, _fillEase));
        }

        private void MovePointer(Vector3 position, float time)
        {
            _pointerTween.Stop();
            if (time == 0)
                _pointer.transform.position = position;
            else
                _pointerTween = Tween.Position(_pointer, position, time, _fillEase);
        }

        private void SpawnFragments(int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                var waveFragment = Instantiate(_waveFragmentTemplate, _parent);
                _waveFragments.Add(waveFragment);
            }
        }
    }
}