using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Shared;
using UnityEngine;

namespace Game
{
    public struct ActionHandleParam
    {
    }
    
    public class ActionHandlerAttribute : Attribute
    {
        public ActionKind kind { get; private set; }

        public ActionHandlerAttribute(ActionKind kind)
        {
            this.kind = kind;
        }
    }

    public static class MethodExtractor
    {
        public static Dictionary<ActionKind, MethodInfo[]> ExtractActionHandleMethod<Target>() where Target : class
        {
            var values = (ActionKind[]) Enum.GetValues(typeof(ActionKind));
            var methodDict =
                typeof(Target).GetMethods()
                    .Select(x => (attr: x.GetCustomAttribute<ActionHandlerAttribute>(), method: x))
                    .Where(x =>
                    {
                        var paramInfos = x.method.GetParameters();
                        return x.attr != null && paramInfos.Length == 2 && 
                               paramInfos[0].ParameterType == typeof(Protocol.Response.Action) && 
                               paramInfos[1].ParameterType == typeof(ActionHandleParam);
                    })
                    .GroupBy(x => x.attr.kind, x => x.method)
                    .ToDictionary(x => x.Key, x => x.ToArray());

            for (var i = 0; i < values.Length; i++)
            {
                if (!methodDict.ContainsKey(values[i]))
                {
                    methodDict.Add(values[i], new MethodInfo[0]);
                    Debug.LogWarning($"ActionKind.{values[i]} 핸들러가 없음");
                }
            }

            return methodDict;
        }
    }
}