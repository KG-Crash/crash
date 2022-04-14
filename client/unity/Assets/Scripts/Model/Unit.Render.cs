using System;
using System.Collections.Generic;
using System.Linq;
using FixMath.NET;
using UnityEngine;

namespace Game
{    
    public interface IRenderable
    {
        Renderer[] renderers { get; }
    }

    public partial class UnitActor : IRenderable 
    {
        [Header("Render")]
        [SerializeField] private Renderer[] _rendereres;
        [SerializeField] private Material[] _deadMaterials;
        [NonSerialized] private Bounds _totalBounds = new Bounds();
        [NonSerialized] private Material[] _liveMaterials;
        [NonSerialized] private MaterialPropertyBlock[] _livePropertyBlocks;
        [NonSerialized] private int[] _blockToRendererIndices;
        [NonSerialized] private int[] _blockToMaterialIndices;

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

        private void LoadMaterials()
        {
            _liveMaterials = _rendereres.SelectMany(renderer => renderer.sharedMaterials).ToArray();
            _livePropertyBlocks = Enumerable.Repeat<MaterialPropertyBlock>(null, _liveMaterials.Length)
                .Select(_null => new MaterialPropertyBlock()).ToArray();
            _blockToRendererIndices = _rendereres
                .SelectMany((renderer, i) => Enumerable.Repeat(i, renderer.sharedMaterials.Length)).ToArray();
            _blockToMaterialIndices = _rendereres
                .SelectMany(renderer => Enumerable.Range(0, renderer.sharedMaterials.Length)).ToArray();
        }
        
        public void UpdateBounds()
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

        public void SetTintByHP(Fix64 curhp, Fix64 maxhp)
        {
            var normalizedHp = curhp / maxhp;

            for (var i = 0; i < _liveMaterials.Length; i++)
            {
                var liveMaterial = _liveMaterials[i];
                var block = _livePropertyBlocks[i];
                
                ShaderUtility.SetColorForHP(liveMaterial.shader, block, normalizedHp);

                var rendererIndex = _blockToRendererIndices[i];
                var materialIndex = _blockToMaterialIndices[i];
                
                _rendereres[rendererIndex].SetPropertyBlock(block, materialIndex); 
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