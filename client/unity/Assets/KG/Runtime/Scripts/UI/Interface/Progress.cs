using System;
using UnityEngine;
using UnityEngine.UI;

namespace KG
{
    [RequireComponent(typeof(Image))]
    public class Progress : MonoBehaviour
    {
        public float progress
        {
            get => progressImage.fillAmount;
            set => progressImage.fillAmount = value;
        }

        private Image _progressImage;

        private Image progressImage
        {
            get
            {
                if (_progressImage == null)
                    _progressImage = GetComponent<Image>();

                return _progressImage;
            }
        }
    }
}