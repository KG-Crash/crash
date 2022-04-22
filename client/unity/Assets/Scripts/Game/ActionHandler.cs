using System;
using System.Collections.Generic;
using System.Reflection;
using JetBrains.Annotations;
using Shared;
using UnityEngine;

namespace Game
{
    public class ActionHandleParam
    {
        public int userId;
    }
    
    public class ActionHandlerAttribute : Attribute
    {
        public ActionKind kind { get; private set; }

        public ActionHandlerAttribute(ActionKind kind)
        {
            this.kind = kind;
        }
    }

    public class TargetMethods
    {
        public object target;
        public Dictionary<ActionKind, MethodInfo[]> actions;

        public TargetMethods(object target, Dictionary<ActionKind, MethodInfo[]> actions)
        {
            this.target = target;
            this.actions = actions;
        }
    }
    
    public static class ActionHandler
    {        
        private static Dictionary<Type, TargetMethods> _actionHandleMethodDict;
        private static ActionHandleParam _actionHandleParam;
        private static object[] _actionMethodParam;

        static ActionHandler()
        {
            _actionHandleMethodDict = new Dictionary<Type, TargetMethods>();
            _actionHandleParam = new ActionHandleParam();
            _actionMethodParam = new object[2];
        }
        
        public static void Bind<T>(T target) where T : class
        {
            var actions = ActionHandleExtractor.ExtractActionHandles<T>();
            _actionHandleMethodDict.Add(typeof(T), new TargetMethods(target, actions));
        }

        public static void Execute<T>(int userId, Protocol.Response.Action action) where T : class
        {
            if (!_actionHandleMethodDict.TryGetValue(typeof(T), out var targetAndAction))
                return;
            
            var actionKind = (ActionKind)action.Id;
            var methods = targetAndAction.actions[actionKind];

            _actionHandleParam.userId = userId;
            
            _actionMethodParam[0] = action;
            _actionMethodParam[1] = _actionHandleParam; 
            
            for (var i = 0; i < methods.Length; i++)
                methods[i].Invoke(targetAndAction.target, _actionMethodParam);
            
            Debug.Assert(methods.Length > 0, $"{actionKind} 처리 메소드가 없음");
        }
    }
}