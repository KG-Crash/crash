using FixMath.NET;
using UnityEngine;

namespace Game
{
    public interface ISelectable
    {
        bool selectable { get; set; }
        Bounds bounds { get; }
    }

    public partial class UnitActor : MonoBehaviour, IActor
    {
        public interface Listener
        {
            void OnClear(UnitActor actor);
        }

        private Listener _listener;

        [SerializeField] private GameObject _highlighted;
        [SerializeField] public Animator animator;

        public GameObject highlighted
        {
            get => _highlighted;
            set
            {
                _highlighted = value;
                _highlighted.transform.SetParent(transform);
            }
        }

        public FixVector3 position { set => this.position = value; }

        public void Init(Listener listener)
        {
            InitAnimation();
            LoadMaterials();

            _listener = listener;
        }
    }
}
