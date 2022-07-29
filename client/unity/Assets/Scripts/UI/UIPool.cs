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
    private string uiBundleName;

    public UIPool(Canvas canvas, string uiBundleName)
    {
        this.canvas = Object.Instantiate(canvas);
        this.uiBundleName = uiBundleName;
        Object.DontDestroyOnLoad(this.canvas);
    }

    public string GetPath(Type type)
    {
        if (string.IsNullOrEmpty(uiBundleName))
            return $"UI/{type.Name}";
        else
            return $"UI/{uiBundleName}/{type.Name}";
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
            var path = GetPath(type);
            var viewPrefab = Resources.Load<UIView>(path);
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
            var path = GetPath(type);
            var viewPrefab = Resources.Load<UIView>(path);
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
