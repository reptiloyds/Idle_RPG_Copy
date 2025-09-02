using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;

namespace PleasantlyGames.RPG.Runtime.Core.Location.View.Component.Background
{
    public class ParallaxMesh : BaseParallax
    {
        [SerializeField] private Mesh _mesh;
        [SerializeField] private Material _material;
        [SerializeField, HideInEditorMode] private bool _editColor;
        [SerializeField] private Color _color;
        [SerializeField] private string _propertyColorName = "_Color";
        [SerializeField] private Vector3 _rotation;
        [SerializeField, ReadOnly] private List<MeshRenderer> _renderers = new();
        
        private MaterialPropertyBlock _mpb;

        protected override void Awake()
        {
            _mpb = new MaterialPropertyBlock();
            _mpb.SetColor(_propertyColorName, _color);
            
            base.Awake();
            
            foreach (var meshRenderer in _renderers) 
                meshRenderer.SetPropertyBlock(_mpb);
        }

        public override void Clear()
        {
            _renderers.Clear();
            base.Clear();
        }

        protected override GameObject GetViewObject()
        {
            var view = new GameObject();
            view.transform.SetParent(transform);
            view.transform.localEulerAngles = _rotation;
            var meshFilter = view.AddComponent<MeshFilter>();
            meshFilter.mesh = _mesh;
            var meshRenderer = view.AddComponent<MeshRenderer>();
            meshRenderer.sharedMaterial = _material;
            meshRenderer.SetPropertyBlock(_mpb);
            meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
            _renderers.Add(meshRenderer);

            return view;
        }

        protected override void Update()
        {
            base.Update();
            
            if(!_editColor) return;
            
            _mpb.SetColor(_propertyColorName, _color);
            foreach (var meshRenderer in _renderers) 
                meshRenderer.SetPropertyBlock(_mpb);
        }
    }
}