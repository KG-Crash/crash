using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AppStateBinds
{
    public bool autoFlatBufferBind { get; private set; }
    public bool autoActionBind { get; private set; }
    public Type[] uiViewTypes { get; private set; }
    public bool[] showBeforeInit { get; private set; }
    public bool receiveTransitionInInitMethod { get; private set; }
    public MethodInfo initMethod { get; private set; }
    public bool receiveNextStateInClearMethod { get; private set; }
    public MethodInfo clearMethod { get; private set; }

    private object[] _emptyParam = new object[0];
    private object[] _oneParam = new object[1];
    
    public bool valid => uiViewTypes != null && showBeforeInit != null && initMethod != null && clearMethod != null;

    public void InvokeInitializeMethod(AppState stateToInit, StateTransition transition)
    {
        if (receiveTransitionInInitMethod)
        {
            _oneParam[0] = transition;
            initMethod.Invoke(stateToInit, _oneParam);   
        }
        else
        {
            initMethod.Invoke(stateToInit, _emptyParam);
        }
    }
    public void InvokeClearMethod(AppState stateToClear, AppState nextState)
    {
        if (receiveNextStateInClearMethod)
        {
            _oneParam[0] = nextState;
            clearMethod.Invoke(stateToClear, _oneParam);
        }
        else
        {
            clearMethod.Invoke(stateToClear, _emptyParam);   
        }
    }

    public static AppStateBinds GetBinds(AppState state)
    {
        var type = state.GetType();
        var bindFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase;
        var allMethods = type.GetMethods(bindFlags);
        
        var binds = new AppStateBinds()
        {
            autoFlatBufferBind = type.GetCustomAttributes()
                .Where(attr => attr is AutoBindAttribute)
                .Select(attr => (attr as AutoBindAttribute).FlatBuffer)
                .FirstOrDefault(),
            autoActionBind = type.GetCustomAttributes()
                .Where(attr => attr is AutoBindAttribute)
                .Select(attr => (attr as AutoBindAttribute).Action)
                .FirstOrDefault(),
            uiViewTypes = type.GetCustomAttributes()
                .Where(attr => attr is UIBindAttribute)
                .SelectMany(attr => (attr as UIBindAttribute).TypeAndShow.Select(x => x.type))
                .ToArray(),
            showBeforeInit = type.GetCustomAttributes()
                .Where(attr => attr is UIBindAttribute)
                .SelectMany(attr => (attr as UIBindAttribute).TypeAndShow.Select(x => x.showBeforeInit))
                .ToArray(),
            initMethod = allMethods.FirstOrDefault(method =>
            {
                if (method.CustomAttributes.All(attr => attr.AttributeType != typeof(InitializeMethodAttribute)))
                    return false;
                
                if (method.ReturnType != typeof(void))
                    return false;
                
                var paramInfo = method.GetParameters();
                if (paramInfo.Length > 1)
                    return false;

                if (paramInfo.Length == 1)
                {
                    return
                        paramInfo[0].ParameterType.IsSubclassOf(typeof(StateTransition)) &&
                        !paramInfo[0].ParameterType.IsAbstract;
                }
                else
                    return true;
            }),
            clearMethod = allMethods.FirstOrDefault(method =>
            {
                if (method.CustomAttributes.All(attr => attr.AttributeType != typeof(ClearMethodAttribute)))
                    return false;
                
                if (method.ReturnType != typeof(void))
                    return false;
                
                var paramInfo = method.GetParameters();

                if (paramInfo.Length > 1)
                    return false;

                if (paramInfo.Length == 1)
                    return paramInfo[0].ParameterType == typeof(AppState) || 
                           paramInfo[0].ParameterType.IsSubclassOf(typeof(AppState));
                else
                    return true;
            }),
        };

        binds.receiveTransitionInInitMethod = binds.initMethod.GetParameters().Length > 0;
        binds.receiveNextStateInClearMethod = binds.clearMethod.GetParameters().Length > 0;
        
        return binds;
    }
}
