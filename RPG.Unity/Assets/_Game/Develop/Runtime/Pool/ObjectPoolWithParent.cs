using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using Object = UnityEngine.Object;

namespace PleasantlyGames.RPG.Runtime.Pool
{
    public class ObjectPoolWithParent<T> where T : Component
    {
        private readonly int _defaultCapacity; 
        private readonly Func<T> _createFunc;
        private readonly Action<T> _actionOnGet;
        private readonly Action<T> _actionOnRelease;
        private readonly Action<T> _actionOnDestroy;
        private readonly ObjectPool<T> _pool;
        private readonly Transform _parent;
        private readonly bool _parentIsTemporary;

        public Transform Parent => _parent;
 
        public ObjectPoolWithParent(string parentName,
            Func<T> createFunc,
            Action<T> actionOnGet = null,
            Action<T> actionOnRelease = null,
            Action<T> actionOnDestroy = null,
            int defaultCapacity = 10, int maxSize = 100)
        {
            _createFunc = createFunc;
            _actionOnGet = actionOnGet;
            _actionOnRelease = actionOnRelease;
            _actionOnDestroy = actionOnDestroy;
            _parent = new GameObject(parentName).transform;
            _parentIsTemporary = true;
        
            _pool = new ObjectPool<T>(
                createFunc: CreateFunc,
                actionOnGet: ActionOnGet,
                actionOnRelease: ActionOnRelease,
                actionOnDestroy: ActionOnDestroy,
                defaultCapacity: defaultCapacity,
                maxSize: maxSize
            );
        }
        
        public ObjectPoolWithParent(Transform parent,
            Func<T> createFunc,
            Action<T> actionOnGet = null,
            Action<T> actionOnRelease = null,
            Action<T> actionOnDestroy = null,
            int defaultCapacity = 10, int maxSize = 100)
        {
            _createFunc = createFunc;
            _actionOnGet = actionOnGet;
            _actionOnRelease = actionOnRelease;
            _actionOnDestroy = actionOnDestroy;
            _parent = parent;
        
            _pool = new ObjectPool<T>(
                createFunc: CreateFunc,
                actionOnGet: ActionOnGet,
                actionOnRelease: ActionOnRelease,
                actionOnDestroy: ActionOnDestroy,
                defaultCapacity: defaultCapacity,
                maxSize: maxSize
            );
        }
        
        public void PreloadObjects(int count)
        {
            var objects = new List<T>();
            for (int i = 0; i < count; i++)
            {
                T obj = _pool.Get();
                objects.Add(obj);
            }

            foreach (var obj in objects) 
                _pool.Release(obj);
        }

        private T CreateFunc()
        {
            var obj = _createFunc.Invoke();
            if(obj.transform.parent != _parent)
                obj.transform.SetParent(_parent);
            return obj;
        }

        private void ActionOnGet(T obj)
        {
            obj.gameObject.SetActive(true);
            _actionOnGet?.Invoke(obj);
        }

        private void ActionOnRelease(T obj)
        {
            _actionOnRelease?.Invoke(obj);
            obj.gameObject.SetActive(false);
            if(obj.transform.parent != _parent)
                obj.transform.SetParent(_parent);
        }

        private void ActionOnDestroy(T obj)
        {
            _actionOnDestroy?.Invoke(obj);
            Object.Destroy(obj.gameObject);
        }
        
        public T Get()
        {
            return _pool.Get();
        }
        
        public void Release(T obj)
        {
            _pool.Release(obj);
        }
        
        public void Clear()
        {
            _pool.Clear();
        }

        public void Destroy()
        {
            Clear();
            if(_parentIsTemporary)
                Object.Destroy(_parent.gameObject);
        }
    }
}