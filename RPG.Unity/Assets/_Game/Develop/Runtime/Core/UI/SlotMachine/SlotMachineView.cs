using System;
using System.Collections.Generic;
using AssetKits.ParticleImage;
using PleasantlyGames.RPG.Runtime.Audio.Contract;
using PleasantlyGames.RPG.Runtime.Audio.Model;
using PleasantlyGames.RPG.Runtime.Core.AudioTypes;
using PleasantlyGames.RPG.Runtime.UnityExtension;
using PleasantlyGames.RPG.Runtime.UnityExtension.Extensions;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Pool;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.UI.SlotMachine
{
    public class SlotMachineView : MonoBehaviour
    {
        [SerializeField] private SlotView _slotView;
        [SerializeField] private RectTransform _slotParent;
        [SerializeField] private RectTransform _slotSpawnPoint;
        [SerializeField] private RectTransform _slotEndPoint;
        [SerializeField] private RectTransform _slotResultPoint;
        [SerializeField] private List<SlotData> _slotData;
        [SerializeField, MinValue(1)] private float _spinDuration = 3f;
        [SerializeField, Range(0, 1)] private float _slowdownTrigger;
        [SerializeField, MinValue(1)] private float _maxSpeed = 1500f;
        [SerializeField, MinValue(1)] private float _minSpeed = 750f;
        [SerializeField] private bool _initializeOnAwake;
        [SerializeField] private float _offset = 15f;

        [Space] [Header("FX :")] 
        [SerializeField] private List<ParticleImage> _particleImages;

        [ShowInInspector, ReadOnly] private SlotData _result;

        private float _speed;
        private ObjectPool<SlotView> _pool;
        private readonly List<SlotView> _slots = new();
        private bool _isSpinning;
        private bool _isResultSpawned;
        private float _spinTime;
        private float _pixelDelta;
        private float _spinTriggerDelta;
        private SlotView _resultView;

        [Inject] private IAudioService _audioService;
        private AudioEmitter _audioEmitter;

        public event Action<SlotData, SlotView> OnStop; 
        public bool IsSpinning => _isSpinning;
        public SlotData Result => _result;

        private void Awake()
        {
            if (_initializeOnAwake) 
                Initialize();
        }

        public void Initialize(List<SlotData> slotData = null)
        {
            if (slotData != null) 
                _slotData = slotData;

            var trueSizeK = _slotParent.anchorMax.y - _slotParent.anchorMin.y;
            _pixelDelta = _slotParent.rect.height * trueSizeK + _offset;
            
            _pool = new ObjectPool<SlotView>(CreateSlotView, GetSlotView, ReleaseSlotView);
            SpawnNewSlot(_slotData.GetRandomElement(), new Vector3(0, _slotResultPoint.anchoredPosition.y - _pixelDelta, 0));
            SpawnNewSlot(_slotData.GetRandomElement(), _slotResultPoint.anchoredPosition);
            SpawnNewSlot(_slotData.GetRandomElement(), new Vector3(0, _slotResultPoint.anchoredPosition.y + _pixelDelta, 0));

            _audioEmitter = _audioService.CreateLocalSound(UI_Effect.UI_RouletteTick).DontRelease().Build();
        }

        private SlotView CreateSlotView() => 
            Instantiate(_slotView, _slotParent);

        private void GetSlotView(SlotView slot)
        {
            slot.gameObject.SetActive(true);
            _slots.Add(slot);
        }

        private void ReleaseSlotView(SlotView slot)
        {
            slot.gameObject.SetActive(false);
            _slots.Remove(slot);
        }

        [Button]
        public void Spin()
        {
            _result = _slotData.WeightedRandomElement();
            _spinTime = 0;
            _isSpinning = true;
            _speed = _maxSpeed;
            _spinTriggerDelta = _spinDuration * _slowdownTrigger;
            _resultView = null;
            _isResultSpawned = false;
        }

        private void FixedUpdate()
        {
            if(!_isSpinning) return;

            _spinTime += Time.fixedDeltaTime;
            
            if (_spinTime <= _spinDuration)
            {
                UpdateSpeed();
                SpinDown();
                TryRemoveLast();

                if (IsNextSlotReady()) 
                    SpawnNewSlot(_slotData.GetRandomElement(), _slotSpawnPoint.anchoredPosition);
            } 
            else
            {
                SpinDown();
                TryRemoveLast();

                if (IsNextSlotReady())
                {
                    SlotData nextSlot;
                    if (_isResultSpawned)
                    {
                        nextSlot = _slotData.GetRandomElement();
                        SpawnNewSlot(nextSlot, _slotSpawnPoint.anchoredPosition);
                    }
                    else
                    {
                        nextSlot = _result;
                        _isResultSpawned = true;
                        _resultView = SpawnNewSlot(nextSlot, _slotSpawnPoint.anchoredPosition);
                    }
                }
            }
        }

        private bool IsNextSlotReady() => 
            GetDistance(_slots[^1].RectTransform, _slotSpawnPoint) >= _pixelDelta;

        private void TryRemoveLast()
        {
            if (_slots[0].RectTransform.anchoredPosition.y <= _slotEndPoint.anchoredPosition.y)
                RemoveLastSlot();
        }

        private void UpdateSpeed()
        {
            var progress = _spinTime / _spinDuration;
            if (progress < _slowdownTrigger) return;
            _speed = Mathf.SmoothStep(_maxSpeed, _minSpeed, _spinTime - _spinTriggerDelta / _spinDuration - _spinTriggerDelta);
        }

        private void SpinDown()
        {
            foreach (var slot in _slots)
            {
                var direction = _slotEndPoint.position - slot.RectTransform.position;
                slot.RectTransform.position += direction.normalized * _speed * Time.deltaTime;
            }

            if (!_slots[1].IsDetected && _slots[1].RectTransform.anchoredPosition.y <= _slotResultPoint.anchoredPosition.y)
                SlotOnResult(_slots[1]);
        }
        
        private void SlotOnResult(SlotView slot)
        {
            slot.Detect();
            PlayFX();
            if (_isResultSpawned && slot == _resultView) 
                StopSpinning();
        }

        private void PlayFX()
        {
            foreach (var particle in _particleImages) 
                particle.Play();
            
            _audioEmitter.Play();
        }

        private void StopSpinning()
        {
            _isSpinning = false;
            OnStop?.Invoke(_result, _slots[1]);
        }

        private SlotView SpawnNewSlot(SlotData slotData, Vector3 position)
        {
            var slot = _pool.Get();
            slot.RectTransform.anchoredPosition = position;
            slot.Setup(slotData.Sprite, $"X{StringExtension.Instance.CutDouble(slotData.Amount, true)}");
            return slot;
        }

        private void RemoveLastSlot() => 
            _pool.Release(_slots[0]);

        private float GetDistance(RectTransform firstElement, RectTransform secondElement)
        {
            var distance = firstElement.anchoredPosition.y - secondElement.anchoredPosition.y;
            return Mathf.Abs(distance);
        }
    }
}
