using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Game;
using KG;
using KG.Reflection;
using Module;
using Network;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AppStateService
{
    private Dictionary<Type, AppState> _appStates;
    private Dictionary<AppState, AppStateBinds> _binds;
    private AppState _current;
    private UIPool _uiPool;
    private UIStack _uiStack;

    public AppStateService(AppState[] appStates, UIPool uiPool)
    {
        _appStates = appStates.ToDictionary(x => x.GetType(), x => x);
        _binds = appStates.ToDictionary(state => state, x => AppStateBindsFactory.Extract(x.GetType()));
        _uiPool = uiPool;
        _uiStack = new UIStack();
        
        foreach (var appState in appStates)
            appState.uiStack = _uiStack;

        if (_binds.Any(kv => !kv.Value.valid))
            throw new Exception($"not valid state = {_binds.First(kv => !kv.Value.valid).Key.GetType().Name}");
    }

    public T Get<T>() where T : AppState
    {
        return _appStates[typeof(T)] as T;
    }

    public async Task<AppState> LoadEntrySceneAsync(StateTransition transition = null)
    {
        var entryState = GetEntryScene();

        if (!_binds.TryGetValue(entryState, out var bind))
            throw new Exception($"{nameof(entryState)} has not bind info");
        
        // 0. load scene
        if (!IsSceneLoaded(entryState.sceneName))
            await SceneManager.LoadSceneAsync(entryState.sceneName);
        var scene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);

        // 1. bind 
        if (bind.autoFlatBufferBind)
            Handler.Bind(entryState, Dispatcher.Instance);
        if (bind.autoActionBind)
            ActionHandler.Bind(entryState);
        
        // 2. create & register ui & scene
        var refViewTypes = _binds[entryState].uiViewTypes;
        var instancedViews = refViewTypes.Select(_uiPool.Reserve).ToArray();
        entryState.Register(instancedViews, SceneManager.GetActiveScene());
        
        // 3. show before initialize
        var showBeforeInit = _binds[entryState].showBeforeInit;
        for (var i = 0; i < showBeforeInit.Length; i++)
            if (showBeforeInit[i])
                _ = entryState.ShowView(instancedViews[i]);
        Canvas.ForceUpdateCanvases();

        // 4. initialize appstate
        SceneContext context = null;
        foreach (var rootGO in scene.GetRootGameObjects())
        {
            context = rootGO.transform.GetComponentInChildren<SceneContext>();
            if (context != null) break;
        }

        InvokeInitializeMethod(entryState, _binds[entryState].initMethod, transition, context);

        return _current = entryState;
    }

    private AppState GetEntryScene()
    {
        if (_appStates.Count <= 0)
            return null;

        var entryAppState = _appStates.FirstOrDefault(x => x.Value.entryScene).Value;
        if (entryAppState != null)
            return entryAppState;

        return _appStates.FirstOrDefault().Value;
    }

    public async Task<T> MoveStateAsync<T>(StateTransition transition = null) where T : AppState  
    {
        var moveAppState = _appStates[typeof(T)];
        var mustLoadScene = _current.sceneName != moveAppState.sceneName;

        // ---------------------------------------------------------------------------------------------------------
        // >> clear previous appstate
        // 0. clear prev appstate
        InvokeClearMethod(_current, _binds[_current].clearMethod, moveAppState);
        
        // 1. stop all coroutines
        _current.StopAllCoroutine();
        
        // 2. remove UI
        {
            _uiStack.Clear();
            _current.ClearViews();
            var refViewTypes = _binds[_current].uiViewTypes;
            for (var i = 0; i < refViewTypes.Length; i++)
                _uiPool.Remove(refViewTypes[i]);
        }

        // 3. unbind 
        {
            if (_binds.TryGetValue(_current, out var bind))
            {
                if (bind.autoFlatBufferBind)
                    Handler.Unbind(_current);
            
                if (bind.autoActionBind)
                    ActionHandler.Unbind(_current);
            }
        }

        // ---------------------------------------------------------------------------------------------------------
        // >> load/unload unity scene
        // 0. load next scene
        if (mustLoadScene)
            await SceneManager.LoadSceneAsync(moveAppState.sceneName);

        // 1. unload prev scene 
        if (mustLoadScene && IsSceneLoaded(_current.sceneName))
            await SceneManager.UnloadSceneAsync(_current.sceneName);

        var scene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);

        // ---------------------------------------------------------------------------------------------------------
        // >> initialize appstate
        // 0. bind 
        {
            if (_binds.TryGetValue(moveAppState, out var bind))
            {
                if (bind.autoFlatBufferBind)
                    Handler.Bind(moveAppState, Dispatcher.Instance);   
             
                if (bind.autoActionBind)
                    ActionHandler.Bind(moveAppState);
            }
        }
       
        {
            // 1. create & register ui & scene
            var refViewTypes = _binds[moveAppState].uiViewTypes;
            var instancedViews = refViewTypes.Select(_uiPool.Reserve).ToArray();
            moveAppState.Register(instancedViews, SceneManager.GetActiveScene());
                    
            // 2. show before initialize
            var showBeforeInit = _binds[moveAppState].showBeforeInit;
            for (var i = 0; i < showBeforeInit.Length; i++)
                if (showBeforeInit[i])
                    _ = moveAppState.ShowView(instancedViews[i]); 
            Canvas.ForceUpdateCanvases();
        }
        
        // 3. initialize appstate
        SceneContext context = null;
        foreach (var rootGO in scene.GetRootGameObjects())
        {
            context = rootGO.transform.GetComponentInChildren<SceneContext>();
            if (context != null) break;
        }

        InvokeInitializeMethod(moveAppState, _binds[moveAppState].initMethod, transition, context);

        return (_current = moveAppState) as T;
    }

    private static bool IsSceneLoaded(string sceneName)
    {
        for (var i = 0; i < SceneManager.sceneCount; i++)
        {
            var scene = SceneManager.GetSceneAt(i);
            if (scene.name == sceneName)
                return true;
        }

        return false;
    }

    private static void InvokeInitializeMethod(AppState state, MethodInfo initMethod, StateTransition transition, SceneContext context)
    {
        Invoker.Invoke(
            state, initMethod, new object[] {transition, context}, 
            AppStateBinds.initializeParamOptions, x => new object[x]
        );
    }

    private static void InvokeClearMethod(AppState state, MethodInfo clearMethod, AppState nextState)
    {
        Invoker.Invoke(
            state, clearMethod, new object[] {nextState}, 
            AppStateBinds.clearParamOptions, x => new object[x]
        );
    }
}