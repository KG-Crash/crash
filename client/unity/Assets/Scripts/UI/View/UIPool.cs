using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Shared;
using UnityEngine;

public class UIPool : MonoBehaviour
{
    public static UIPool ist { get; private set; }

    private readonly Dictionary<Type, GameObject> _pool = new Dictionary<Type, GameObject>();

    void Awake()
    {
        ist = this;
    }
    
    public static T Get<T>() where T : UIView
    {
        return ist.GetInternal<T>();
    }
    
    private T GetInternal<T>() where T : UIView
    {
        try
        {
            if (_pool.ContainsKey(typeof(T)) == false)
            {
                var attribute = typeof(T).GetCustomAttributes(typeof(UIAttribute), true).FirstOrDefault() as UIAttribute ??
                                throw new ClientException(ClientExceptionCode.NotFoundUIAttribute, "Not found UI attribute.");

                var prefab = Resources.Load<GameObject>($"UI/{attribute.Path}");
                var instance = Instantiate(prefab, transform);
                _pool[typeof(T)] = instance;
                instance.SetActive(false);
            }

            return _pool[typeof(T)].GetComponent<T>() ??
                   throw new ClientException(ClientExceptionCode.NotContainUIScript,
                       $"{_pool[typeof(T)].name} does not contains {typeof(T).Name} script.");
        }
        catch (Exception e)
        {
            return null;
        }
    }
}
