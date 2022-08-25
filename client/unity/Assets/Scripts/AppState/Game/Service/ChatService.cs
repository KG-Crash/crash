using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Game.Service
{
    public class ChatService : MonoBehaviour
    {
        [SerializeField] public bool _useCheat;

        [SerializeField] private Text _chatLog;
        [SerializeField] private InputField _input;
        [SerializeField] private TMP_InputField _inputTmp;
        [SerializeField] private ScrollRect _scrollRect;

        private GameState _gameState;

        // Start is called before the first frame update
        void Start()
        {
            _gameState = EntryPoint.appStateService.Get<GameState>();
            _scrollRect.verticalNormalizedPosition = 0.0f;
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return))
                SendMessage();
        }

        public bool SendMessage()
        {
            if (string.IsNullOrEmpty(_inputTmp.text.Trim()))
                return false;

            var msg = _inputTmp.text;

            _inputTmp.ActivateInputField();
            _inputTmp.text = string.Empty;

            var resultMsg = CheatService.ParseMessage(msg, _gameState);
            if (string.IsNullOrEmpty(resultMsg))
                return true;

            _gameState.actionService.Send(resultMsg);
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