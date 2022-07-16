using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Shared;
using UnityEngine;
using Object = UnityEngine.Object;

public class UIPool
{
    private readonly Dictionary<Type, UIView> _pool = new Dictionary<Type, UIView>();
    private Canvas canvas;

    public UIPool(Canvas canvas)
    {
        this.canvas = Object.Instantiate(canvas);
        Object.DontDestroyOnLoad(this.canvas);
    }
    
    public T Reserve<T>() where T : UIView
    {
        return Reserve(typeof(T)) as T;
    }
    
    public UIView Reserve(Type type)
    {
        if (!type.IsSubclassOf(typeof(UIView)) && type != typeof(UIView))
            return null;
    
        if (_pool.ContainsKey(type) == false)
        {
            var viewPrefab = Resources.Load<UIView>($"UI/{type.Name}");
            if (viewPrefab == null)
                throw new ClientException(ClientExceptionCode.NotFoundUIAttribute, "Not found UI Prefab.");
            
            var instance = Object.Instantiate(viewPrefab, canvas.transform);
            _pool[type] = instance;
            instance.gameObject.SetActive(false);
        }

        return _pool[type];
    }

    public void Remove<T>() where T : UIView
    {
        Remove(typeof(T));
    }

    public void Remove(Type type)
    {
        var view = _pool[type];
        _pool.Remove(type);
        Object.Destroy(view.gameObject);
    }

    public UIView Get(Type type)
    {
        if (!type.IsSubclassOf(typeof(UIView)) ||
            type == typeof(UIView))
            return null;
        
        if (_pool.ContainsKey(type) == false)
        {
            var viewPrefab = Resources.Load<UIView>($"UI/{type.Name}");
            if (viewPrefab == null)
                return null;
                // throw new ClientException(ClientExceptionCode.NotFoundUIAttribute, "Not found UI Prefab.");
                
            var instance = Object.Instantiate(viewPrefab, canvas.transform);
            _pool[type] = instance;
            instance.gameObject.SetActive(false);
        }

        return _pool[type];
    }

    public T Get<T>() where T : UIView
    {
        return Get(typeof(T)) as T;
    }
}
