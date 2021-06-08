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
        [SerializeField] private int _unitOriginID;
        [SerializeField] private GameObject highlighted;

        public int team
        {
            get => _team;
            set => _team = value;
        }

        public bool selectable
        {
            get => _selectable;
            set => _selectable = value;
        }

        public int unitOriginID
        {
            get => _unitOriginID;
        }
        
        public int unitID
        {
            get => _unitID;
            set => _unitID = value;
        }

        [SerializeField] private int _team;
        [SerializeField] private bool _selectable = true;
        [SerializeField] private int _unitID;
        
        public Bounds bounds { get => _totalBounds; }
        public Renderer[] renderers { get => _rendereres; }

        [NonSerialized] private Bounds _totalBounds = new Bounds();
        [SerializeField] private Renderer[] _rendereres;

        [ContextMenu("Gather renderers")]
        private void OnRefreshRenderers()
        {
            _rendereres = GetComponentsInChildren<Renderer>();
        }

        private void Update()
        {
            _totalBounds = new Bounds();
            foreach (var renderer in _rendereres)
            {
                _totalBounds.Encapsulate(renderer.bounds);
            }
        }

        public void Selected(bool select)
        {
            if (highlighted) highlighted.SetActive(select);
        }

        public void MoveTo(Vector3 position)
        {
        }
    }
}