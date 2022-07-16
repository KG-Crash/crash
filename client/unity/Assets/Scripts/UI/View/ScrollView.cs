using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace KG
{
    public class ScrollView : MonoBehaviour
    {
        public interface IListener<T>
        {
            IEnumerator<T> OnRefresh();
            void OnCreated(T data, UIBehaviour item);
            void OnDestroyed(UIBehaviour item);
        }

        [SerializeField]
        private HorizontalOrVerticalLayoutGroup _layoutGroup;
        public UIBehaviour prefab;

        public void Refresh<T>(IListener<T> listener)
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
                listener.OnCreated(enumerator.Current, item);
            }
        }
    }
}
