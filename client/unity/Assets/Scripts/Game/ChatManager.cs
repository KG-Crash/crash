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

            resultMsg = ParseMessage(msg);
            if(msg.Equals(resultMsg))
                RecvMessage(msg);
            else               
                Debug.Log(resultMsg);
            return;            
        }

        public void RecvMessage(string msg)
        {
            _chatLog.text += "\n" + _name + " : " + msg;
            _scrollRect.verticalNormalizedPosition = 0.0f;
        }

        public string ParseMessage(string msg)
		{
            MethodInfo methodInfo = null;
            List<int> parameter = new List<int>();

            foreach (var method in GetCommandMethodInfos())
			{
                Match m = Regex.Match(msg, method.GetCustomAttribute<BuildCommandAttribute>().command);
                if (m.Success)
                    methodInfo = method;
            }

            if (methodInfo == null)
                return msg;


            string pattern = @"\d";

            MatchCollection matches = Regex.Matches(msg, pattern);
            if (matches.Count <= 0)
                return msg;
            

            foreach(var m in matches)
			{
                parameter.Add(int.Parse(m.ToString()));
			}

            methodInfo.Invoke(null , new object[] { parameter.ToArray() });
            msg = methodInfo.GetCustomAttribute<BuildCommandAttribute>().command + "Chit enable";
            return msg;
        }

        public List<string> GetCommandList()
		{
            List<string> commandList = new List<string>();

            var methods = typeof(GameController).GetMethods().Where(x =>
            {
                if (x.GetCustomAttribute<BuildCommandAttribute>() == null)
                    return false;
                if (x.ReturnType != typeof(void))
                    return false;

                var parameters = x.GetParameters();
                if (parameters.Length != 1)
                    return false;

                if (parameters[0].ParameterType.GetTypeInfo() != typeof(int[]))
                    return false;

                return true;

            });
            
            foreach (var method in methods)
            {
                var a = method.GetCustomAttribute<BuildCommandAttribute>();
                commandList.Add(a.command);
            }

            return commandList;
		}

        public List<MethodInfo> GetCommandMethodInfos()
        {
            List<MethodInfo> methodList = new List<MethodInfo>();

            var methods = typeof(GameController).GetMethods().Where(x =>
            {
                if (x.GetCustomAttribute<BuildCommandAttribute>() == null)
                    return false;
                if (x.ReturnType != typeof(void))
                    return false;

                var parameters = x.GetParameters();
                if (parameters.Length != 1)
                    return false;

                if (parameters[0].ParameterType.GetTypeInfo() != typeof(int[]))
                    return false;

                return true;

            });

            return methods.ToList();
        }
    }
}