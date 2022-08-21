using System;
using Cysharp.Threading.Tasks;
using KG;
using UnityEngine;
using TMPro;

namespace UI
{
    public class BoolPopupTMP : UIView, IPopup<bool>
    {
        [SerializeField] private TextMeshProLabel _title;
        [SerializeField] private KG.ButtonSingleTMP _yesButton;
        [SerializeField] private KG.ButtonSingleTMP _noButton;
        
        UniTaskCompletionSource<bool> utcs = new UniTaskCompletionSource<bool>();

        public void SetPopupTexts(string title, string yes, string no)
        {
            _title.text = title;
            _yesButton.text = yes;
            _noButton.text = no;
        }
        
        private void OnEnable()
        {
            _yesButton.onClick.AddListener(OnYes);
            _noButton.onClick.AddListener(OnNo);
        }

        private void OnDisable()
        {
            _yesButton.onClick.RemoveListener(OnYes);
            _noButton.onClick.RemoveListener(OnNo);
        }

        private void OnDestroy()
        {
            utcs.TrySetCanceled();
        }

        private void OnYes()
        {
            utcs.TrySetResult(true);
        }

        private void OnNo()
        {
            utcs.TrySetResult(true);
        }

        public void SetActive(bool active)
        {
            gameObject.SetActive(active);
        }
        
        public UniTask<bool> Response()
        {
            return utcs.Task;
        }
    }
}