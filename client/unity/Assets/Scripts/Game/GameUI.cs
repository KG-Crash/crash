using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Game
{
    public class GameUI : MonoBehaviour
    {
        [SerializeField] private UnityEngine.UI.Image _uiImage;

        public void ActiveDragRect(bool enable)
        {
            _uiImage.enabled = enable;
        }
        
        public void UpdateDragRect(Rect rectSS)
        {
            var rect = _uiImage.GetComponent<RectTransform>();
            rect.position = rectSS.position;
            rect.sizeDelta = rectSS.size;
        }

    }
}