using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public abstract class UIView : MonoBehaviour
{
    private static readonly Stack<UIView> _uiViewStack = new Stack<UIView>();

    public virtual async Task OnLoad()
    { }

    public virtual async Task OnClosed()
    { }

    public static async Task<T> Show<T>(bool hideBackView = false) where T : UIView
    {
        return await Show(UIPool.Get<T>(), hideBackView);
    }

    public static async Task<T> Show<T>(T view, bool hideBackView = false) where T : UIView
    {
        view.gameObject.SetActive(true);

        if (hideBackView && _uiViewStack.Count > 0)
        {
            var backView = _uiViewStack.Peek();
            backView?.gameObject.SetActive(false);
        }

        _uiViewStack.Push(view);
        await view.OnLoad();
        return view;
    }

    public static T Get<T>() where T : UIView
    {
        return _uiViewStack.FirstOrDefault(x => x.GetType() == typeof(T)) as T;
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
