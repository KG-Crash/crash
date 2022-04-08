using UnityEngine;
using UnityEngine.UI;

namespace Game.Service
{
    public class ChatService : MonoBehaviour
    {
        [SerializeField] public bool _useCheat;

        [SerializeField] private Text _chatLog;
        [SerializeField] private InputField _input;
        [SerializeField] private ScrollRect _scrollRect;

        private GameController _gameController;

        // Start is called before the first frame update
        void Start()
        {
            _gameController = this.gameObject.GetComponent<GameController>();
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
            if (string.IsNullOrEmpty(_input.text.Trim()))
                return false;

            var msg = _input.text;

            _input.ActivateInputField();
            _input.text = string.Empty;

            var resultMsg = CheatService.ParseMessage(msg, _gameController);
            if (string.IsNullOrEmpty(resultMsg))
                return true;

            _gameController.ActionService.Send(resultMsg);
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