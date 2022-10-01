using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using KG;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AppStateBinds
{
    public bool autoFlatBufferBind { get; private set; }
    public bool autoActionBind { get; private set; }
    public Type[] uiViewTypes { get; private set; }
    public bool[] showBeforeInit { get; private set; }
    public MethodInfo initMethod { get; private set; }
    public MethodInfo clearMethod { get; private set; }
    
    public bool valid => uiViewTypes != null && showBeforeInit != null && initMethod != null && clearMethod != null;

    public AppStateBinds(bool autoFlatBufferBind, bool autoActionBind, Type[] uiViewTypes, bool[] showBeforeInit,
        MethodInfo initMethod, MethodInfo clearMethod)
    {
        this.autoFlatBufferBind = autoFlatBufferBind;
        this.autoActionBind = autoActionBind;
        this.uiViewTypes = uiViewTypes;
        this.showBeforeInit = showBeforeInit;
        this.initMethod = initMethod;
        this.clearMethod = clearMethod;
    }
}