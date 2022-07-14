using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Module;
using Shared;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class AppState : ScriptableObject
{
    [SerializeField] private string _sceneName;
    [SerializeField] private bool _entryScene;
    
    // Contexts
    private Dictionary<Type, UIView> _views = new Dictionary<Type, UIView>();
    private List<Coroutine> _coroutines = new List<Coroutine>();
    protected Scene _scene;

    public void Register(UIView[] views, Scene scene)
    {
        Array.ForEach(views, view => _views.Add(view.GetType(), view));
        _scene = scene;
    }

    public void ClearViews()
    {
        _views.Clear();
    }

    public T GetView<T>() where T : UIView
    {
        var type = typeof(T);
        if (_views.ContainsKey(type))
            return _views[type] as T;
        else
            return null;
    }

    public bool TryGetView<T>(out T view) where T : UIView
    {
        var type = typeof(T);
        if (_views.ContainsKey(type))
            view = _views[type] as T;
        else
            view = null;

        return view != null;
    }

    public T ShowView<T>(bool hideBackView = false) where T : UIView
    {
        var view = GetView<T>();
        if (view == null)
            throw new ClientException(ClientExceptionCode.NotFoundUIAttribute, $"not exist UI attribute in {typeof(T).Name}");
        
        _ = UIView.Show(view, hideBackView);
        return view;
    }
    
    public string sceneName => _sceneName;
    public bool entryScene => _entryScene;
    
    public async Task<T> MoveStateAsync<T>(StateTransition transition = null) where T : AppState 
    {
        return await EntryPoint.appStateService.MoveStateAsync<T>(transition);
    }

    public Coroutine StartCoroutine(IEnumerator enumerator)
    {
        var coroutine = Dispatcher.Instance.StartCoroutine(enumerator);
        _coroutines.Add(coroutine);
        return coroutine;
    }

    public void StopCoroutine(Coroutine coroutine)
    {
        if (_coroutines.Exists(other => other == coroutine))
        {
            Dispatcher.Instance.StopCoroutine(coroutine);
            _coroutines.Remove(coroutine);
        }
    }

    public void StopAllCoroutine()
    {
        _coroutines.ForEach(coroutine => { Dispatcher.Instance.StopCoroutine(coroutine); });
        _coroutines.Clear();
    }
}