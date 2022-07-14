using System;

[System.AttributeUsage(AttributeTargets.Class)]
public class AutoBindAttribute : System.Attribute
{
    public bool FlatBuffer { get; private set; }

    public AutoBindAttribute(bool flatBuffer)
    {
        this.FlatBuffer = flatBuffer;
    }
}

[System.AttributeUsage(AttributeTargets.Class)]
public class UIBindAttribute : System.Attribute
{ 
    public Type Type { get; private set; }
    public bool ShowBeforeInit { get; private set; }

    public UIBindAttribute(Type type, bool showBeforeInit)
    {
        Type = type;
        ShowBeforeInit = showBeforeInit;
    }
}

[System.AttributeUsage(AttributeTargets.Method)]
public class InitializeMethodAttribute : System.Attribute
{
}

[System.AttributeUsage(AttributeTargets.Method)]
public class ClearMethodAttribute : System.Attribute
{
}