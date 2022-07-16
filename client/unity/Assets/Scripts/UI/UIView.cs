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

    public async Task Close()
    {
        await OnClosed();
        gameObject.SetActive(false);
    }
}
