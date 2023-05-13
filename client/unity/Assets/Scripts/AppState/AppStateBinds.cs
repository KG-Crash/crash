using System;
using System.Reflection;
using KG.Reflection;

public class AppStateBinds
{
    public bool autoActionBind { get; private set; }
    public Type[] uiViewTypes { get; private set; }
    public bool[] showBeforeInit { get; private set; }
    public MethodInfo initMethod { get; private set; }
    public MethodInfo clearMethod { get; private set; }
    
    public bool valid => uiViewTypes != null && showBeforeInit != null && initMethod != null && clearMethod != null;

    public AppStateBinds(bool autoFlatBufferBind, bool autoActionBind, Type[] uiViewTypes, bool[] showBeforeInit,
        MethodInfo initMethod, MethodInfo clearMethod)
    {
        this.autoActionBind = autoActionBind;
        this.uiViewTypes = uiViewTypes;
        this.showBeforeInit = showBeforeInit;
        this.initMethod = initMethod;
        this.clearMethod = clearMethod;
    }
        
    public static readonly ParamOption[] initializeParamOptions =
    {
        new ParamOption {type = typeof(StateTransition), required = false, acceptFlag = MatchParamFlag.SubClass},
        new ParamOption {type = typeof(SceneContext), required = false, acceptFlag = MatchParamFlag.SubClass}
    };

    public static readonly ParamOption[] clearParamOptions =
    {
        new ParamOption {type = typeof(AppState), required = false, acceptFlag = MatchParamFlag.All}
    };
}