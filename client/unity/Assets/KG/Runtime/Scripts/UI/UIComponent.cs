using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;

namespace KG
{
    [RequireComponent(typeof(RectTransform))]
    public abstract class UIComponent : UIBehaviour
    {
        private RectTransform _rectTransform;
        public RectTransform rectTransform
        {
            get
            {
                if (_rectTransform == null)
                    _rectTransform = GetComponent<RectTransform>();
                return _rectTransform;
            }
        }
    }
}