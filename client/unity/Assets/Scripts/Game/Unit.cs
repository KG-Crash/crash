using System;
using UnityEngine;

namespace Game
{   
    
    public interface ISelectable
    {
        bool selectable { get; set; }
        Bounds bounds { get; }
    }

    public interface IRenderable
    {
        Renderer[] renderers { get; }
    }
    
    public class Unit : MonoBehaviour, ISelectable, IRenderable
    {
        public bool selectable { get; set; }
        public Bounds bounds { get => _totalBounds; }
        public Renderer[] renderers { get => _rendereres; }

        [NonSerialized] private Bounds _totalBounds = new Bounds();
        [SerializeField] private Renderer[] _rendereres;

        private void OnEnable()
        {
            foreach (var renderer in _rendereres)
            {
                _totalBounds.Encapsulate(renderer.bounds);
            }
        }
    }
}