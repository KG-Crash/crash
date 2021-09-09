using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class ChatManager : MonoBehaviour
    {
        [SerializeField] private Text _chatLog;
        [SerializeField] private InputField _input;
        [SerializeField] private ScrollRect _scrollRect;
        [SerializeField] private string _name;
        // Start is called before the first frame update
        void Start()
        {
            _scrollRect.verticalNormalizedPosition = 0.0f;
            _name = "ME";
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return))
                SendMessage();
        }

        public void SendMessage()
        {
            if (_input.text.Equals(" ") || _input.text.Equals(""))
                return;
            string msg = _input.text;
            RecvMessage(msg);
            _input.ActivateInputField();
            _input.text = "";
        }

        public void RecvMessage(string msg)
        {
            _chatLog.text += "\n" + _name + " : " + msg;
            _scrollRect.verticalNormalizedPosition = 0.0f;
        }
    }
}