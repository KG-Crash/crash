using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Module;
using Shared;
using UnityEngine;
using UnityEngine.SceneManagement;
using KG;

public abstract class AppState : ScriptableObject
{
    [SerializeField] private string _sceneName;
    [SerializeField] private bool _entryScene;
    
    // Contexts
    private Dictionary<Type, UIView> _uiViews = new Dictionary<Type, UIView>();
    private List<Coroutine> _coroutines = new List<Coroutine>();
    protected Scene _scene;
    private UIStack _uiStack = null;

    public void Ctor(UIStack stack)
    {
        _uiStack = stack;
    }
    
    public void Register(UIView[] views, Scene scene)
    {
        Array.ForEach(views, view => _uiViews.Add(view.GetType(), view));
        _scene = scene;
    }

    public void ClearViews()
    {
        _uiViews.Clear();
    }

    public T GetView<T>() where T : UIView
    {
        var type = typeof(T);
        if (_uiViews.ContainsKey(type))
            return _uiViews[type] as T;
        else
            return null;
    }

    public bool TryGetView<T>(out T view) where T : UIView
    {
        var type = typeof(T);
        if (_uiViews.ContainsKey(type))
            view = _uiViews[type] as T;
        else
            view = null;

        return view != null;
    }
    
    public T ShowView<T>(bool hideBackView = false) where T : UIView
    {
        var view = GetView<T>();
        if (view == null)
            throw new ClientException(ClientExceptionCode.NotFoundUIAttribute, $"not exist UI attribute in {typeof(T).Name}");
        
        return ShowView<T>(view, hideBackView);
    }

    public T ShowView<T>(T view, bool hideBackView = false) where T : UIView
    {
        _ = _uiStack.Show(view, hideBackView);
        return view;
    }

    public async Task CloseView<T>(bool showBackView) where T : UIView
    {
        var view = GetView<T>();
        await CloseView<T>(view, showBackView);
    }

    public async Task CloseView<T>(UIView view, bool showBackView) where T : UIView
    {
        await _uiStack.Close<T>(view);
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