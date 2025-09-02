using System;
using System.Collections.Generic;
using AssetKits.ParticleImage;
using PleasantlyGames.RPG.Runtime.Audio.Contract;
using PleasantlyGames.RPG.Runtime.Audio.Model;
using PleasantlyGames.RPG.Runtime.Core.AudioTypes;
using PleasantlyGames.RPG.Runtime.Core.Roulette.Sheet;
using PleasantlyGames.RPG.Runtime.UnityExtension;
using Sirenix.OdinInspector;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using Random = UnityEngine.Random;

namespace PleasantlyGames.RPG.Runtime.Core.UI.RouletteWheel
{
    [DisallowMultipleComponent, HideMonoScript]
    public class RouletteWheelView : MonoBehaviour
    {
        [SerializeField] private bool _initializeOnAwake = false;
        [Header("References :")]
        [SerializeField] private GameObject _linePrefab;
        [SerializeField] private Transform _linesParent;
        [SerializeField] private RectTransform _wheelCircle;
        [SerializeField] private WheelPieceView _piecePrefab;
        [SerializeField] private Transform _wheelPiecesParent;
        
        [Space] [Header("VFX :")]
        [SerializeField] private List<ParticleImage> _particleImages; 

        [Space] [Range(4, 20)] [SerializeField] private int _spinDuration = 4;
        [Header("Pieces (2 - 8):")] [SerializeField] private List<WheelPieceData> _wheelPieces;

        private bool _isSpinning = false;
        private const int PiecesMin = 2;
        private const int PiecesMax = 8;
        private float _pieceAngle;
        private float _halfPieceAngle;
        private float _halfPieceAngleWithPaddings;
        private Vector3 _targetRotation;
        private float _elapsedTime;
        private float _startRotation;
        private float _finalRotation;
        private float _prevAngle, _currentAngle;
        private bool _isIndicatorOnTheLine;
        private readonly List<WheelPieceView> _pieceViews = new();

        private WheelPieceData _pieceData;

        [Inject] private IAudioService _audioService;
        [Inject] private IObjectResolver _objectResolver;
        private AudioEmitter _audioEmitter;
        private IReadOnlyDictionary<int, RouletteSheet.Elem> _pieceDictionary;

        public bool IsSpinning => _isSpinning;
        public event Action OnStart;
        public event Action<WheelPieceData, WheelPieceView> OnStop;

        private void OnValidate()
        {
            if (_wheelPieces.Count > PiecesMax || _wheelPieces.Count < PiecesMin)
                Debug.LogError("[ PickerWheel ]  pieces length must be between " + PiecesMin + " and " + PiecesMax);
        }

        private void Awake()
        {
            if (_initializeOnAwake) 
                Initialize();
        }

        public void Initialize(List<WheelPieceData> dataList = null, IReadOnlyDictionary<int, RouletteSheet.Elem> pieceDictionary = null)
        {
            _pieceDictionary = pieceDictionary;
            if (dataList != null) 
                _wheelPieces = dataList;
            
            _pieceAngle = 360 / _wheelPieces.Count;
            _halfPieceAngle = _pieceAngle / 2f;
            _halfPieceAngleWithPaddings = _halfPieceAngle - (_halfPieceAngle / 4f);

            Generate();
            for (var i = 0; i < _wheelPieces.Count; i++) 
                _wheelPieces[i].Index = i;
            
            _audioEmitter = _audioService.CreateLocalSound(UI_Effect.UI_RouletteTick).DontRelease().Build();
        }

        private void Generate()
        {
            for (var i = 0; i < _wheelPieces.Count; i++)
                DrawPiece(i);
        }

        private void DrawPiece(int index)
        {
            var pieceData = _wheelPieces[index];
            var piece = InstantiatePiece();
            piece.Setup(pieceData.Sprite, $"X{pieceData.Amount.ToString()}", _pieceDictionary[index].ResourceType);

            var lineTransform = Instantiate(_linePrefab, _linesParent.position, Quaternion.identity, _linesParent).transform;
            lineTransform.RotateAround(_wheelPiecesParent.position, Vector3.back, (_pieceAngle * index) + _halfPieceAngle);
            piece.transform.RotateAround(_wheelPiecesParent.position, Vector3.back, _pieceAngle * index);
            
            _pieceViews.Add(piece);
        }

        private WheelPieceView InstantiatePiece()
        {
            var piece = _objectResolver.Instantiate(_piecePrefab, Vector3.zero, Quaternion.identity, _wheelPiecesParent);
            piece.transform.localPosition = Vector3.zero;
            return piece;
        }

        public void Spin()
        {
            if (_isSpinning) return;

            _isSpinning = true;
            OnStart?.Invoke();

            int index = _wheelPieces.WeightedRandomElement().Index;
            _pieceData = _wheelPieces[index];

            var angle = -(_pieceAngle * index);

            var rightOffset = (angle - _halfPieceAngleWithPaddings) % 360;
            var leftOffset = (angle + _halfPieceAngleWithPaddings) % 360;

            var randomAngle = Random.Range(leftOffset, rightOffset);

            var targetRotation = Vector3.back * (randomAngle + 2 * 360 * _spinDuration);

            _prevAngle = _currentAngle = _wheelCircle.eulerAngles.z;
            _isIndicatorOnTheLine = false;
            
            _startRotation = _wheelCircle.eulerAngles.z;
            _finalRotation = targetRotation.z + 360 * 5;
            _elapsedTime = 0f;
        }

        public void Cancel() => 
            _isSpinning = false;

        private void Update()
        {
            if (!_isSpinning) return;

            var diff = Mathf.Abs(_prevAngle - _currentAngle);
            if (diff >= _halfPieceAngle)
            {
                if (_isIndicatorOnTheLine)
                {
                    _audioEmitter.Play();
                    foreach (var particleImage in _particleImages) 
                        particleImage.Play();
                } 

                _prevAngle = _currentAngle;
                _isIndicatorOnTheLine = !_isIndicatorOnTheLine;
            }

            _currentAngle = _wheelCircle.eulerAngles.z;
            _elapsedTime += Time.deltaTime;

            var newRotation = Mathf.SmoothStep(_startRotation, _finalRotation, _elapsedTime / _spinDuration);

            _wheelCircle.eulerAngles = new Vector3(0, 0, newRotation);

            if (_elapsedTime < _spinDuration) return;
            
            StopSpinning();
        }

        private void StopSpinning()
        {
            _isSpinning = false;
            _wheelCircle.eulerAngles = new Vector3(0, 0, _finalRotation % 360);
            OnStop?.Invoke(_pieceData, _pieceViews[_pieceData.Index]);
        }
    }
}