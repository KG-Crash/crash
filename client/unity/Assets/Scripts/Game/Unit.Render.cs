using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{    
    public interface IRenderable
    {
        Renderer[] renderers { get; }
    }

    public partial class Unit : IRenderable 
    {
        [Header("Render")]
        [SerializeField] private Renderer[] _rendereres;
        [SerializeField] private Material[] _deadMaterials;
        [NonSerialized] private Bounds _totalBounds = new Bounds();
        
        public Bounds bounds { get => _totalBounds; }
        public Renderer[] renderers { get => _rendereres; }
        public Material[] deadMaterials
        {
            get => _deadMaterials;
            set => _deadMaterials = value;
        }
        
        [ContextMenu("Gather renderers")]
        public void OnRefreshRenderers()
        {
            _rendereres = GetComponentsInChildren<Renderer>();
        }
        
        private void UpdateBounds()
        {
            _totalBounds = new Bounds();
            foreach (var renderer in _rendereres)
            {
                if (_totalBounds == new Bounds())
                {
                    _totalBounds = renderer.bounds;
                }
                else
                {
                    _totalBounds.Encapsulate(renderer.bounds);
                }
            }
        }

        private void SetFadeMaterialAndAlpha(float alpha)
        {
            for (int i = 0; i < _rendereres.Length; i++)
            {
                _rendereres[i].sharedMaterial = _deadMaterials[i];
                var color = _deadMaterials[i].color;
                color.a = alpha;
                _deadMaterials[i].color = color;
            }
        }
        
        private void SetFadeAlpha(float alpha)
        {
            for (int i = 0; i < _rendereres.Length; i++)
            {
                var color = _deadMaterials[i].color;
                color.a = alpha;
                _deadMaterials[i].color = color;
            }
        }
    }
}