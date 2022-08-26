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
    public (Type type, bool showBeforeInit)[] TypeAndShow { get; private set; }

    public UIBindAttribute(Type type, bool showBeforeInit = false)
    {
        TypeAndShow = new[] {(type, showBeforeInit)};
    }
    
    public UIBindAttribute(Type t1, bool s1, Type t2, bool s2)
    {
        TypeAndShow = new [] {(t1, s1), (t2, s2)};
    }
    
    public UIBindAttribute(Type t1, bool s1, Type t2, bool s2, Type t3, bool s3)
    {
        TypeAndShow = new [] {(t1, s1), (t2, s2), (t3, s3)};
    }
    
    public UIBindAttribute(Type t1, bool s1, Type t2, bool s2, Type t3, bool s3, Type t4, bool s4)
    {
        TypeAndShow = new [] {(t1, s1), (t2, s2), (t3, s3), (t4, s4)};
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