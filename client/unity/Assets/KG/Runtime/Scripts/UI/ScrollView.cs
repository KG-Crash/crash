using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace KG
{
    public class ScrollView : MonoBehaviour
    {
        public interface IListener<T1, T2> where T2 : UIComponent 
        {
            IEnumerator<T1> OnRefresh();
            void OnCreated(T1 data, T2 uiComponent);
            void OnDestroyed(T2 uiComponent);
        }

        [SerializeField]
        private HorizontalOrVerticalLayoutGroup _layoutGroup;
        public UIComponent prefab;

        public void Refresh<T1, T2>(IListener<T1, T2> listener) where T2 : UIComponent
        {
            if(listener == null)
                throw new System.ArgumentNullException("Refresh<T>(IListener<T> listener) == null");

            foreach (Transform item in _layoutGroup.transform)
            {
                GameObject.Destroy(item.gameObject);
            }

            var enumerator = listener.OnRefresh();
            while (enumerator.MoveNext())
            {
                var item = Instantiate(prefab, _layoutGroup.transform);
                listener.OnCreated(enumerator.Current, item as T2);
            }
        }
    }
}
