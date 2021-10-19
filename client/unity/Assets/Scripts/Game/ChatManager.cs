using System.Collections;
using System.Collections.Generic;
using System;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
	public class ChatManager : MonoBehaviour
	{
		[SerializeField] public bool _useCheat;

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
			string resultMsg = "";

			_input.ActivateInputField();
			_input.text = "";

			resultMsg = msg;			
			if(_useCheat)
				resultMsg = CheatManager.ParseMessage(msg);

			if (msg.Equals(resultMsg))
				RecvMessage(resultMsg);
			else
			{
				Debug.Log(resultMsg);
				PrintLog(resultMsg);
			}
			return;
		}

		public void RecvMessage(string msg)
		{
			_chatLog.text += "\n" + _name + " : " + msg;
			_scrollRect.verticalNormalizedPosition = 0.0f;
		}

		public void PrintLog(string msg)
		{
			_chatLog.text += "\n" + msg;
			_scrollRect.verticalNormalizedPosition = 0.0f;
		}
	}
}