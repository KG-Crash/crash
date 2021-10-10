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

        private HorizontalOrVerticalLayoutGroup _layoutGroup;
        public UIBehaviour prefab;

        public void Start()
        {
            _layoutGroup = GetComponentInChildren<HorizontalOrVerticalLayoutGroup>() ??
                throw new System.Exception($"Prefab {this.name} dose not contains layout group. You have to add layout group in [Scroll View/Viewport/Contents].");
        }

        public void Refresh<T>(IListener<T> listener)
        {
            if(listener == null)
                throw new System.Exception("지랄 니은");

            var enumerator = listener.OnRefresh();
            while (enumerator.MoveNext())
            {
                var item = Instantiate(prefab, _layoutGroup.transform);
                listener.OnCreated(enumerator.Current, item);

                enumerator.MoveNext();
            }
        }
    }
}
