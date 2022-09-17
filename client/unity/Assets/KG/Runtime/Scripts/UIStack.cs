using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace KG
{
    public class UIStack
    {
        private readonly Stack<UIView> _uiViewStack = new Stack<UIView>();

        public async Task<T> Show<T>(T view, bool hideBackView = false) where T : UIView
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

        public T Get<T>() where T : UIView
        {
            return _uiViewStack.FirstOrDefault(x => x.GetType() == typeof(T)) as T;
        }

        public async Task<T> CloseTopView<T>() where T : UIView
        {
            if (_uiViewStack.Peek() is T view && view)
            {
                Debug.Assert(_uiViewStack.Pop() == view, "peek/pop result not equal.."); 
                await view.Close();

                if (_uiViewStack.Count > 0)
                {
                    var backView = _uiViewStack.Peek();
                    backView?.gameObject.SetActive(true);
                }

                return view;
            }

            return null;
        }

        public async Task<UIView> Close<T>() where T : UIView
        {
            var view = Get<T>();
            return await Close<T>(view);
        }

        public async Task<UIView> Close<T>(UIView view) where T : UIView
        {
            await view.Close();

            if (_uiViewStack.Count > 0)
            {
                var backView = _uiViewStack.Peek();
                backView?.gameObject.SetActive(true);
            }

            return view;
        }

        public void Clear() => _uiViewStack.Clear();
    }
}