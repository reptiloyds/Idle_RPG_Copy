using System.Collections.Generic;
using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.Location.View.Component.Background
{
    [DisallowMultipleComponent, HideMonoScript]
    public abstract class BaseParallax : MonoBehaviour
    {
        [SerializeField, Min(0)] protected float _viewLength;
        [SerializeField, Min(1)] protected int _amount;
        [SerializeField, Range(0, 1)] protected float _speedK;
        [SerializeField, Min(0)] protected float _scale = 1;
        [SerializeField, ReadOnly] private bool _isBuilt;
        [SerializeField, ReadOnly] protected List<GameObject> Views;
        [SerializeField] private float _offset;
        
        [SerializeField, HideInInspector] protected float HalfTotalLength;
        [SerializeField, HideInInspector] private float _totalLength;
        [SerializeField, HideInInspector] private float _halfLength;
        [SerializeField, HideInInspector] private Vector3 _borderPosition;
        
        private readonly Vector3 _moveDirection = Vector3.left;
        
        private bool _isRunning;
        private float _speed;
        private Tween _stopMovement;
        public bool IsBuilt => _isBuilt;

        protected virtual void Awake()
        {
            if(_isBuilt) return;
            Build();
        }

        public virtual void Build()
        {
            Views = new List<GameObject>(_amount);
            _totalLength = _amount * _viewLength;
            HalfTotalLength = _totalLength / 2;
            _borderPosition = _moveDirection * HalfTotalLength;
            _halfLength = _viewLength / 2;
            
            for (var i = 0; i < _amount; i++)
            {
                var view = GetViewObject();
                view.name = GetViewName(i);
                Views.Add(view);
                view.transform.localPosition = GetPositionForView(i);
            }

            foreach (var view in Views)
            {
                view.transform.localPosition += _moveDirection * _offset;
            }
            
            transform.localScale = Vector3.one * _scale;
            
            _isBuilt = true;
        }

        public virtual void Clear()
        {
            if (Views != null)
            {
                foreach (var view in Views) 
                    DestroyImmediate(view);
                Views.Clear();   
            }
            
            transform.localScale = Vector3.one;
            
            _isBuilt = false;
        }

        protected abstract GameObject GetViewObject();

        protected string GetViewName(int index) => 
            $"View_{index}";

        protected Vector3 GetPositionForView(int viewIndex)
        {
            var xPosition = _viewLength * viewIndex + _halfLength - HalfTotalLength;
            return new Vector3(xPosition, 0, 0);
        }
        
        public void Run(float speed)
        {
            _speed = speed;
            _isRunning = true;
            _stopMovement.Stop();
        }
        
        public void Stop(float timeToStop)
        {
            _stopMovement.Stop();
            _stopMovement = Tween.Custom(_speed, 0, timeToStop, (value) => _speed = value)
                .OnComplete(() => _isRunning = false);
        }
        
        protected virtual void Update()
        {
            if (!_isRunning) return;

            foreach (var view in Views) 
                view.transform.localPosition += _moveDirection * (_speed * _speedK * Time.deltaTime);

            var closestToDestruction = GetClosestToDestruction();
            if (closestToDestruction.transform.localPosition.x <= _borderPosition.x) 
                closestToDestruction.transform.localPosition = GetFurthestToDestruction().transform.localPosition + new Vector3(_viewLength, 0, 0);
        }
        
        private GameObject GetClosestToDestruction()
        {
            float minX = float.MaxValue;
            GameObject result = null;
            foreach (var view in Views)
            {
                var posX = view.transform.localPosition.x;
                if (posX < minX)
                {
                    result = view;
                    minX = posX;
                }
            }

            return result;
        }

        private GameObject GetFurthestToDestruction()
        {
            float maxX = float.MinValue;
            GameObject result = null;
            foreach (var view in Views)
            {
                var posX = view.transform.localPosition.x;
                if (posX > maxX)
                {
                    result = view;
                    maxX = posX;
                }
            }

            return result;
        }
    }
}