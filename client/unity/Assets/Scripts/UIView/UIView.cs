using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIAttribute : System.Attribute
{ 
    public string Path { get; private set; }

    public UIAttribute(string path)
    {
        Path = path;
    }
}

public abstract class UIView : MonoBehaviour
{
    private string _resource = string.Empty;
    private static Stack<UIView> _uiViewStack = new Stack<UIView>();

    public UIView()
    { }

    // Start is called before the first frame update
    public virtual void Start()
    {
        
    }

    // Update is called once per frame
    public virtual void Update()
    {
        
    }

    public static void Show<T>() where T : UIView
    {
        var x = UIPool.Get<T>();
        x.gameObject.SetActive(true);

        _uiViewStack.Push(x);
    }

    public static void Close()
    {
        var x = _uiViewStack.Pop();
        x.gameObject.SetActive(false);
    }
}
