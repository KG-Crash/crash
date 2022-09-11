using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using KG;
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

    public AppStateService(AppState[] appStates, KG.UIPool uiPool)
    {
        _appStates = appStates.ToDictionary(x => x.GetType(), x => x);
        _binds = appStates.ToDictionary(state => state, AppStateBinds.GetBinds);
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

        // 1. bind flatbuffer
        if (bind.autoFlatBufferBind)
            Handler.Bind(entryState, Dispatcher.Instance);
        
        // 2. create & register ui & scene
        var refViewTypes = _binds[entryState].uiViewTypes;
        var instancedViews = refViewTypes.Select(_uiPool.Reserve).ToArray();
        entryState.Register(instancedViews, SceneManager.GetActiveScene());
        
        // 3. show before initialize
        var showBeforeInit = _binds[entryState].showBeforeInit;
        for (var i = 0; i < showBeforeInit.Length; i++)
            if (showBeforeInit[i])
                _ = entryState.ShowView(instancedViews[i]); 
        
        // 4. initialize appstate
        bind.InvokeInitializeMethod(entryState, transition);

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
        // 0. clear appstate
        _binds[_current].InvokeClearMethod(_current, moveAppState);
        
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

        // 3. unbind flatbuffer
        {
            if (_binds.TryGetValue(_current, out var bind) && bind.autoFlatBufferBind)
                Handler.Unbind(_current);
        }

        // ---------------------------------------------------------------------------------------------------------
        // 0. load next scene
        if (mustLoadScene)
            await SceneManager.LoadSceneAsync(moveAppState.sceneName);

        // 1. unload prev scene 
        if (mustLoadScene && IsSceneLoaded(_current.sceneName))
            await SceneManager.UnloadSceneAsync(_current.sceneName);
        
        // ---------------------------------------------------------------------------------------------------------

        // 0. bind flatbuffer
        {
            if (_binds.TryGetValue(moveAppState, out var bind) && bind.autoFlatBufferBind)
                Handler.Bind(moveAppState, Dispatcher.Instance);
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
        }
        
        // 3. initialize appstate
        _binds[moveAppState].InvokeInitializeMethod(moveAppState, transition);

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
}