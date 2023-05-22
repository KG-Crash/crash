using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using KG;

namespace UI
{
    public class ChattingView : UIComponent
    {
        [SerializeField]
        private ScrollView _chatLog; 
        [SerializeField]
        private ButtonSingleTMP _sendButton;
        [SerializeField]
        private TextFieldTMP _inputField;

   
        public ScrollView chatLog => _chatLog;
        public ButtonSingleTMP sendButton => _sendButton;
        public string inputFieldText => _inputField.text;

    }
}