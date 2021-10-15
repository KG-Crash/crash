using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Linq;

namespace Game
{
    public class CheatManager
    {
        static public string ParseMessage(string msg)
        {
            MethodInfo methodInfo = null;
            List<int> parameter = new List<int>();

            foreach (var method in GetCommandMethodInfos())
            {
                String command = method.GetCustomAttribute<BuildCommandAttribute>().command;

                Regex regex = new Regex("(?<command>" + command + ")" + @"\s*(?<param>.+\d)\s*");
                Match match = regex.Match(msg);

                if (!match.Groups["command"].ToString().Equals("")) // 해당 커맨드가 있으면
                {
                    methodInfo = method;
                    foreach (var param in match.Groups["param"].ToString().Split())
                    {
                        parameter.Add(int.Parse(param));
                    }
                }
            }

            if (methodInfo == null)
                return msg;

            if (parameter.Count < 1)
                return msg;

            methodInfo.Invoke(null, new object[] { parameter.ToArray() });
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

        static public List<MethodInfo> GetCommandMethodInfos()
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