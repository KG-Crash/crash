using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    private static readonly Stack<UIView> _uiViewStack = new Stack<UIView>();

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

    public virtual async Task OnLoad()
    { }

    public virtual async Task OnClosed()
    { }

    public static async Task Show<T>(bool hideBackView = false) where T : UIView
    {
        var x = UIPool.Get<T>();
        x.gameObject.SetActive(true);

        if (hideBackView && _uiViewStack.Count > 0)
        {
            var backView = _uiViewStack.Peek();
            backView?.gameObject.SetActive(false);
        }

        await x.OnLoad();
        _uiViewStack.Push(x);
    }

    public static async Task Close()
    {
        var x = _uiViewStack.Pop();
        
        await x.OnClosed();
        x.gameObject.SetActive(false);

        if (_uiViewStack.Count > 0)
        {
            var backView = _uiViewStack.Peek();
            backView?.gameObject.SetActive(true);
        }
    }
}
