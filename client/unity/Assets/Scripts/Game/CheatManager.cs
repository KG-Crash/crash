using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Linq;
using UnityEngine;

namespace Game
{
    public class CheatManager
    {
        static public string ParseMessage(string msg, GameController gameController)
        {
            List<object> paramListToCall = new List<object>();
            object[] matchedParams = Array.Empty<object>();

            Regex regex;
            Match match;
            String command;

            ParameterInfo[] paramInfos;
            MethodInfo methodInfo = null;

            int defaultParamQuantity;
            bool hasLastArrayParam;
            List<int> arrayParamList = null;

            foreach (var method in GetCommandMethodInfos())
            {
                command = method.GetCustomAttribute<BuildCommandAttribute>().command;
                regex = new Regex("(?<command>" + command + ")" + @"\s*(?<param>.*)\s*");
                match = regex.Match(msg);

                defaultParamQuantity = 0;
                hasLastArrayParam = false;

                if( !string.IsNullOrEmpty(match.Groups["command"].ToString()))
                {
                    matchedParams = Array.Empty<object>();
                    methodInfo = method;
                    paramInfos = methodInfo.GetParameters();

                    foreach (var param in paramInfos)
                    {
                        if (param.HasDefaultValue)
                            defaultParamQuantity++;
                    }

                    if (paramInfos.Length > 0 && paramInfos.Last().ParameterType.IsArray)
                    {
                        hasLastArrayParam = true;
                        arrayParamList = new List<int>();
                    }

                    if (!string.IsNullOrEmpty(match.Groups["param"].ToString()))
                        matchedParams = match.Groups["param"].ToString().Split();

                    if (paramInfos.Length - matchedParams.Length > defaultParamQuantity + Convert.ToInt32(hasLastArrayParam))
                        return msg;

                    if (paramInfos.Length == 0)
                        break;
                    else
                    {
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


                                if (i >= paramInfos.Length - 1 && arrayParamList != null)
                                {
                                    Debug.Log(i.GetType());
                                    convertedParam = Convert.ChangeType(param, i.GetType());
                                    arrayParamList.Add((int)convertedParam);
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
                }
            }

            if (methodInfo == null)
                return msg;

            if (arrayParamList != null)
                paramListToCall.Add(arrayParamList.ToArray());

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