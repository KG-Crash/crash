using UnityEngine;
using UnityEngine.EventSystems;

namespace KG
{
    public abstract class UIAutoComponent<T> : UIComponent where T : MonoBehaviour
    {
        private T _instance;

        public T instance
        {
            get
            {
                if (_instance == null)
                    _instance = GetComponentInChildren<T>();
                return _instance;
            }
        }
    }
    
    public abstract class UIAutoComponent<T0, T1> : UIComponent where T0 : MonoBehaviour where T1 : MonoBehaviour
    {
        private T0 _instance0;
        public T0 instance0
        {
            get
            {
                if (_instance0 == null)
                    _instance0 = GetComponentInChildren<T0>();
                return _instance0;
            }
        }

        private T1 _instance1;
        public T1 instance1
        {
            get
            {
                if (_instance1 == null)
                    _instance1 = GetComponentInChildren<T1>();
                return _instance1;
            }
        }
    }

}