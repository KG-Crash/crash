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
        static public string ParseMessage(string msg)
        {
            MethodInfo methodInfo = null;
            List<object> paramList = null;

            foreach (var method in GetCommandMethodInfos())
            {
                String command = method.GetCustomAttribute<BuildCommandAttribute>().command;

                Regex regex = new Regex("(?<command>" + command + ")" + @"\s*(?<param>.*)\s*");
                Match match = regex.Match(msg);

                if (!match.Groups["command"].ToString().Equals(""))
                {
                    object[] matchParams = Array.Empty<object>();
                    methodInfo = method;
                    var paramInfos = methodInfo.GetParameters();

                    if (!match.Groups["param"].ToString().Equals(""))
                        matchParams = match.Groups["param"].ToString().Split();

                    if (paramInfos.Length != matchParams.Length)
                        return msg;

                    if (paramInfos.Length == 0)
                        break;
                    else
                    {
                        paramList = new List<object>();
                        try
                        {
                            for (int i = 0; i < paramInfos.Length; i++)
                            {
                                var convertedParam = Convert.ChangeType(matchParams[i], paramInfos[i].ParameterType);
                                paramList.Add(convertedParam);
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

            if (paramList != null)
                methodInfo.Invoke(null, paramList.ToArray());
            else
                methodInfo.Invoke(null, (object[])null);

            msg = methodInfo.GetCustomAttribute<BuildCommandAttribute>().command + " Cheat enable";
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