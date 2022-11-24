using System;

[System.AttributeUsage(AttributeTargets.Class)]
public class StateBindAttribute : System.Attribute
{
    public bool FlatBuffer { get; private set; }
    public bool Action { get; private set;}

    public StateBindAttribute(bool flatBuffer, bool action = false)
    {
        this.FlatBuffer = flatBuffer;
        this.Action = action;
    }
}

[System.AttributeUsage(AttributeTargets.Class)]
public class UIBindAttribute : System.Attribute
{
    public Type[] types { get; private set; }
    public bool[] showBeforeInits { get; private set; }

    public UIBindAttribute(Type type, bool showBeforeInit = false)
    {
        types = new[] {type};
        showBeforeInits = new[] {showBeforeInit};
    }

    public UIBindAttribute(Type t1, bool s1, Type t2, bool s2)
    {
        types = new[] {t1, t2};
        showBeforeInits = new[] {s1, s2};
    }

    public UIBindAttribute(Type t1, bool s1, Type t2, bool s2, Type t3, bool s3)
    {
        types = new[] {t1, t2, t3};
        showBeforeInits = new[] {s1, s2, s3};
    }

    public UIBindAttribute(Type t1, bool s1, Type t2, bool s2, Type t3, bool s3, Type t4, bool s4)
    {
        types = new[] {t1, t2, t3, t4};
        showBeforeInits = new[] {s1, s2, s3, s4};
    }

    public UIBindAttribute(Type[] types, bool[] showBeforeInits)
    {
        this.types = types;
        this.showBeforeInits = showBeforeInits;
    }
}

[System.AttributeUsage(AttributeTargets.Method)]
public class InitializeMethodAttribute : System.Attribute
{
    public Type transition;
    public Type context;
}

[System.AttributeUsage(AttributeTargets.Method)]
public class ClearMethodAttribute : System.Attribute
{
    public Type nextAppState;
}