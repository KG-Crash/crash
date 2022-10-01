using System.Reflection;
using KG;

public static class AppStateInvoker
{
    private static readonly ParamOption[] initializeParamOptions =
    {
        new ParamOption {type = typeof(StateTransition), required = false, acceptFlag = MatchParamFlag.SubClass},
        new ParamOption {type = typeof(SceneContext), required = false, acceptFlag = MatchParamFlag.SubClass}
    };

    public static bool IsInitializeMethod(MethodInfo method)
    {
        return KG.DynamicInvoker.MatchAttrAndParam(method, typeof(InitializeMethodAttribute), initializeParamOptions);
    }

    public static void InvokeInitializeMethod(AppState state, MethodInfo initMethod, StateTransition transition, SceneContext context)
    {
        DynamicInvoker.Invoke(
            state, initMethod, new object[] {transition, context}, 
            initializeParamOptions, x => new object[x]
        );
    }

    private static readonly ParamOption[] clearParamOptions =
    {
        new ParamOption {type = typeof(AppState), required = false, acceptFlag = MatchParamFlag.All}
    };

    public static bool IsClearMethod(MethodInfo method)
    {
        return KG.DynamicInvoker.MatchAttrAndParam(method, typeof(ClearMethodAttribute), clearParamOptions);
    }
    
    public static void InvokeClearMethod(AppState state, MethodInfo clearMethod, AppState nextState)
    {
        DynamicInvoker.Invoke(
            state, clearMethod, new object[] {nextState}, 
            initializeParamOptions, x => new object[x]
        );
    }
}