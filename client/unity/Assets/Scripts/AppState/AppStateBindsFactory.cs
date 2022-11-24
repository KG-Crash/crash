using System;
using System.Reflection;
using KG.Reflection;

public class AppStateBindsFactory
{
    private static readonly BindingFlags methodFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase;

    private static bool IsInitializeMethod(MethodInfo method)
    {
        return Predicates.MatchAttrAndParam(method, typeof(InitializeMethodAttribute), AppStateBinds.initializeParamOptions);
    }
    
    private static bool IsClearMethod(MethodInfo method)
    {
        return Predicates.MatchAttrAndParam(method, typeof(ClearMethodAttribute), AppStateBinds.clearParamOptions);
    }
    
    public static AppStateBinds Extract(Type type)
    {
        var (stateBind, uiBind) = Extractor.GetAttributes<StateBindAttribute, UIBindAttribute>(type);
        var (initMethod, clearMethod) = Extractor.GetMethods(type, methodFlags, IsInitializeMethod, IsClearMethod);

        return new AppStateBinds(
            stateBind.FlatBuffer, stateBind.Action, uiBind.types, uiBind.showBeforeInits, initMethod, clearMethod
        );
    }
}