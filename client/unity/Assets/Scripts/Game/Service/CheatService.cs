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

            foreach (var method in GetCommandMethodInfos())
            { 
                #region ParseRegex
                var command = method.GetCustomAttribute<BuildCommandAttribute>().command;
                var regex = new Regex("(?<command>" + command + ")" + @"\s*(?<param>.*)\s*");
                var match = regex.Match(msg);

                int defaultParamCount = 0;

                if (string.IsNullOrEmpty(match.Groups["command"].ToString()))
                    continue;

                methodInfo = method;
                var matchedParams = Array.Empty<object>();
                var paramInfos = methodInfo.GetParameters();

                if (paramInfos.Length == 0)
                    break;
                #endregion

                defaultParamCount = GetDefaultParamCount(paramInfos);

                if (!string.IsNullOrEmpty(match.Groups["param"].ToString()))
                    matchedParams = match.Groups["param"].ToString().Split();

                if (paramInfos.Length - matchedParams.Length > defaultParamCount)
                    return msg;
                if (matchedParams.Length != paramInfos.Length && defaultParamCount == 0)
                    return msg;

                if(!ConvertParams(paramListToCall, matchedParams, paramInfos, defaultParamCount))
                    return msg;

                break;
            }

            if (methodInfo is null)
                return msg;

            if (paramListToCall.Count > 0)
                methodInfo.Invoke(gameController, paramListToCall.ToArray());
            else
                methodInfo.Invoke(gameController, (object[])null);

            return string.Empty;
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

        static public bool ConvertParams(List<object> paramListToCall, object[] matchedParams, ParameterInfo[] paramInfos,int defaultParamCount )
        {
            for (int i = 0; i < paramInfos.Length; i++)
            {
                object param = null;
                object convertedParam = null;

                if (i >= matchedParams.Length)
                    param = paramInfos[i].DefaultValue;
                else
                    param = matchedParams[i];


                if (param is null)
                    convertedParam = null;
                else if (Nullable.GetUnderlyingType(paramInfos[i].ParameterType) != null)
                    convertedParam = Convert.ChangeType(param, Nullable.GetUnderlyingType(paramInfos[i].ParameterType));// getunderlying typ e
                else
                    convertedParam = Convert.ChangeType(param, paramInfos[i].ParameterType);

                paramListToCall.Add(convertedParam);
            }

            if (paramListToCall.Count != paramInfos.Length)
                return false;

            return true;
        }

        static public int GetDefaultParamCount(ParameterInfo[] paramInfos)
        {
            var count = 0;
            foreach (var param in paramInfos)
            {
                if (param.HasDefaultValue)
                    count++;
            }
            return count;
        }
    }
}