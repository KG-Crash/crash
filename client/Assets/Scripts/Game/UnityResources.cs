using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class UnityResources : ScriptableObject
    {
        public static UnityResources _instance { get; private set; }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        private static void CreateInstance()
        {
            _instance = CreateInstance<UnityResources>();
        }

        private Dictionary<string, UnityObjects> _unityObjs;

        private void Awake()
        {
            _unityObjs = new Dictionary<string, UnityObjects>();
        }

        public UnityObjects Get(string name)
        {
            var lowerName = name.Trim().ToLower();
            return _unityObjs.ContainsKey(lowerName)? _unityObjs[lowerName]: null;
        }

        public void Register(UnityObjects unityObjects)
        {
            _unityObjs.Add(unityObjects.name, unityObjects);
        }

        public void Unregister(UnityObjects unityObjects)
        {
            _unityObjs.Remove(unityObjects.name);
        }
    }
}