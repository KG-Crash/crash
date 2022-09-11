using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace KG
{
    [RequireComponent(typeof(RectTransform))]
    public abstract class UIView : UIBehaviour
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

        public virtual async Task OnLoad()
        { }

        public virtual async Task OnClosed()
        { }

        public async Task Close()
        {
            await OnClosed();
            gameObject.SetActive(false);
        }
    }
}
