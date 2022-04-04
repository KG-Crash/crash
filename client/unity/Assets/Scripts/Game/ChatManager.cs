using System.Collections;
using System.Collections.Generic;
using System;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using Network;
using Protocol.Request;

namespace Game
{
    public class ChatManager : MonoBehaviour
    {
        [SerializeField] public bool _useCheat;

        [SerializeField] private Text _chatLog;
        [SerializeField] private InputField _input;
        [SerializeField] private ScrollRect _scrollRect;
        [SerializeField] private string _name;

        private GameController _gameController;

        // Start is called before the first frame update
        void Start()
        {
            _gameController = this.gameObject.GetComponent<GameController>();
            _scrollRect.verticalNormalizedPosition = 0.0f;
            _name = "ME";
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return))
                SendMessage();
        }

        public async Task<bool> SendMessage()
        {
            if (_input.text.Equals(" ") || _input.text.Equals(""))
                return false;

            string msg = _input.text;
            string resultMsg = "";

            _input.ActivateInputField();
            _input.text = "";

            resultMsg = CheatManager.ParseMessage(msg, _gameController);

            if(resultMsg != "")
                _gameController.EnqueueChatAction(resultMsg);

            return true;
        }

        public void RecvMessage(string msg, string user)
        {
            string resultMsg = msg;

            

            _chatLog.text += $"\n {user}  :  {resultMsg}";
            _scrollRect.verticalNormalizedPosition = 0.0f;
        }
    }
}