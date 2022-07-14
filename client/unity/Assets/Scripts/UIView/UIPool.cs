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

    public static T Reserve<T>() where T : UIView
    {
        return Reserve(typeof(T)) as T;
    }
    
    public static UIView Reserve(Type type)
    {
        if (!type.IsSubclassOf(typeof(UIView)) && type != typeof(UIView))
        {
            throw new ClientException(ClientExceptionCode.NotFoundUIAttribute, "Not found UI attribute.");
        }
    
        if (ist._pool.ContainsKey(type) == false)
        {
            var prefab = Resources.Load<GameObject>($"UI/{type.Name}");
            if (prefab == null)
                throw new ClientException(ClientExceptionCode.NotFoundUIAttribute, "Not found UI Prefab.");
            
            var instance = Instantiate(prefab, ist.transform);
            ist._pool[type] = instance;
            instance.SetActive(false);
        }

        return ist._pool[type].GetComponent(type) as UIView;
    }

    public static void Remove<T>() where T : UIView
    {
        Remove(typeof(T));
    }

    public static void Remove(Type type)
    {
        var view = ist._pool[type];
        ist._pool.Remove(type);
        Destroy(view);
    }

    public static T Get<T>() where T : UIView
    {
        try
        {
            var type = typeof(T);
            if (ist._pool.ContainsKey(typeof(T)) == false)
            {
                var prefab = Resources.Load<GameObject>($"UI/{type.Name}");
                if (prefab == null)
                    throw new ClientException(ClientExceptionCode.NotFoundUIAttribute, "Not found UI Prefab.");
                
                var instance = Instantiate(prefab, ist.transform);
                ist._pool[typeof(T)] = instance;
                instance.SetActive(false);
            }

            return ist._pool[typeof(T)].GetComponent<T>() ??
                   throw new ClientException(ClientExceptionCode.NotContainUIScript,
                       $"{ist._pool[typeof(T)].name} does not contains {typeof(T).Name} script.");
        }
        catch (Exception e)
        {
            return null;
        }
    }
}
