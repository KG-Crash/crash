using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace KG
{
    public abstract class UIView : MonoBehaviour
    {
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
}
