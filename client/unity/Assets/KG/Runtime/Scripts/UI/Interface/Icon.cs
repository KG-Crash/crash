using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace KG
{
    [RequireComponent(typeof(Image))]
    public class Icon : MonoBehaviour
    {
        private Image _image;
        private Image image
        {
            get
            {
                if (_image == null)
                    _image = GetComponent<Image>();

                return _image;
            }
        }

        public Sprite icon
        {
            get => image.sprite;
            set => image.sprite = value;
        }
    }
}