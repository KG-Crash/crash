using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Game.Service
{
    public class CheatService
    {
        static public string ParseMessage(string msg, GameController gameController)
        {
            List<object> paramListToCall = new List<object>();

            MethodInfo methodInfo = null;

            List<int> arrayTypeParamList = null;

            foreach (var method in GetCommandMethodInfos())
            {
                var command = method.GetCustomAttribute<BuildCommandAttribute>().command;
                var regex = new Regex("(?<command>" + command + ")" + @"\s*(?<param>.*)\s*");
                var match = regex.Match(msg);

                int defaultParamQuantity = 0;
                bool hasLastArrayParam = false;

                if (string.IsNullOrEmpty(match.Groups["command"].ToString()))
                    continue;

                var matchedParams = Array.Empty<object>();
                var paramInfos = methodInfo.GetParameters();

                methodInfo = method;

                if (paramInfos.Length == 0)
                    break;

                foreach (var param in paramInfos)
                {
                    if (param.HasDefaultValue)
                        defaultParamQuantity++;
                }

                if (paramInfos.Length > 0 && paramInfos.Last().ParameterType.IsArray)
                {
                    hasLastArrayParam = true;
                    arrayTypeParamList = new List<int>();
                }

                if (!string.IsNullOrEmpty(match.Groups["param"].ToString()))
                    matchedParams = match.Groups["param"].ToString().Split();

                if (paramInfos.Length - matchedParams.Length > defaultParamQuantity + Convert.ToInt32(hasLastArrayParam))
                    return msg;



                try
                {
                    int loopCount = matchedParams.Length + defaultParamQuantity;

                    if (matchedParams.Length == paramInfos.Length && defaultParamQuantity > 0)
                        loopCount = matchedParams.Length;

                    for (int i = 0; i < loopCount; i++)
                    {
                        object param = null;
                        object convertedParam = null;

                        if (i >= matchedParams.Length)
                            param = paramInfos[i].DefaultValue;
                        else
                            param = matchedParams[i];


                        if (i >= paramInfos.Length - 1 && arrayTypeParamList != null)
                        {
                            Debug.Log(i.GetType());
                            convertedParam = Convert.ChangeType(param, i.GetType());
                            arrayTypeParamList.Add((int)convertedParam);
                        }
                        else
                        {
                            if (param is null)
                                convertedParam = null;
                            else
                                convertedParam = Convert.ChangeType(param, paramInfos[i].ParameterType);

                            paramListToCall.Add(convertedParam);
                        }
                    }
                }
                catch (FormatException e)
                {
                    return msg;
                }
            }

            if (methodInfo is null)
                return msg;

            if (arrayTypeParamList != null)
                paramListToCall.Add(arrayTypeParamList.ToArray());

            if (paramListToCall.Count > 0)
                methodInfo.Invoke(gameController, paramListToCall.ToArray());
            else
                methodInfo.Invoke(gameController, (object[])null);

            msg = string.Empty;
            return msg;
        }

        static public List<string> GetCommandList()
        {
            List<string> commandList = new List<string>();

            var methods = typeof(GameController).GetMethods().Where(x =>
            {
                if (x.GetCustomAttribute<BuildCommandAttribute>() == null)
                    return false;
                if (x.ReturnType != typeof(void))
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

        static public List<MethodInfo> GetCommandMethodInfos()
        {
            List<MethodInfo> methodList = new List<MethodInfo>();

            var methods = typeof(GameController).GetMethods().Where(x =>
            {
                if (x.GetCustomAttribute<BuildCommandAttribute>() == null)
                    return false;
                if (x.ReturnType != typeof(void))
                    return false;

                return true;
            });

            return methods.ToList();
        }
    }
}