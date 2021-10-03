using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIPool : MonoBehaviour
{
    public static UIPool ist { get; private set; }

    private Dictionary<Type, GameObject> _pool = new Dictionary<Type, GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        ist = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static T Get<T>() where T : UIView
    {
        if (ist._pool.ContainsKey(typeof(T)) == false)
        {
            var attribute = typeof(T).GetCustomAttributes(typeof(UIAttribute), true).FirstOrDefault() as UIAttribute ??
                throw new System.Exception($"Not found UI attribute.");

            var prefab = Resources.Load<GameObject>($"UI/{attribute.Path}");
            var instance = Instantiate(prefab, ist.transform);
            ist._pool[typeof(T)] = instance;
            instance.SetActive(false);
        }

        return ist._pool[typeof(T)].GetComponent<T>() ??
            throw new Exception($"{ist._pool[typeof(T)].name} does not contains {typeof(T).Name} script.");
    }
}
