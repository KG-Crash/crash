using System;
using Cysharp.Threading.Tasks;
using KG;
using UnityEngine;

namespace UI
{
    public class BoolPopup : UIView, IPopup<bool>
    {
        [SerializeField] private TextLabel _title;
        [SerializeField] private KG.ButtonSingle _yesButton;
        [SerializeField] private KG.ButtonSingle _noButton;
        
        UniTaskCompletionSource<bool> utcs = new UniTaskCompletionSource<bool>();

        public void SetPopupTexts(string title, string yes, string no)
        {
            _title.text = title;
            _yesButton.text = yes;
            _noButton.text = no;
        }

        protected override void OnEnable()
        {
            _yesButton.onClick.AddListener(OnYes);
            _noButton.onClick.AddListener(OnNo);
        }

        protected override void OnDisable()
        {
            _yesButton.onClick.RemoveListener(OnYes);
            _noButton.onClick.RemoveListener(OnNo);
        }

        protected override void OnDestroy()
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