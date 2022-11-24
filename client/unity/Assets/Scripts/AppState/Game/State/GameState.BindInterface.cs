using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FixMath.NET;
using Game;
using Game.Service;
using KG.Reflection;
using Module;
using Network;
using UI;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public struct Frame
{
    public int currentFrame;
    public int currentTurn;
    public Fix64 deltaTime;
}

public interface IUpdateLockStep
{
    void OnUpdateLockStep(Frame input, Frame output);
}

public class UpdateLockStepAttribute : Attribute {}

public partial class GameState
{
    private Dictionary<object, IUpdateLockStep> _updateSubscribers = new Dictionary<object, IUpdateLockStep>();
    private Dictionary<UnityEngine.Object, IUpdateLockStep> _updateUnitySubscribers = new Dictionary<UnityEngine.Object, IUpdateLockStep>();
    private Dictionary<UnityEngine.Object, MethodInfo[]> _updateMethods = new Dictionary<UnityEngine.Object, MethodInfo[]>();

    private ParamOption[] _paramOptions = {
        new ParamOption() {name = "input", type = typeof(Frame), acceptFlag = MatchParamFlag.Self, required = true},
        new ParamOption() {name = "output", type = typeof(Frame), acceptFlag = MatchParamFlag.Self, required = true}
    };

    private object[] _expectedParamArray = new object[2];
    private object[] _paramArray = new object[2];

    public void BindSelfMethod()
    {
        _updateMethods[this] =
            Extractor.ExtractMethodAsAttr<GameState, UpdateLockStepAttribute>(
                BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic,
                x => x.GetCustomAttribute<UpdateLockStepAttribute>() != null
            );
    }
    
    public void Bind(object obj)
    {
        if (obj is UnityEngine.Object unityObj)
        {
            Bind(unityObj);
            return;
        }

        if (obj is IUpdateLockStep updater)
            _updateSubscribers[obj] = updater;
    }

    public void Bind(UnityEngine.Object obj)
    {
        if (obj is IUpdateLockStep updater && updater != null)
            _updateUnitySubscribers[obj] = updater;
    }

    public void Unbind(object obj)
    {
        if (obj is UnityEngine.Object unityObj)
        {
            Unbind(unityObj);
            return;
        }
        
        if (obj is IUpdateLockStep && _updateSubscribers.ContainsKey(obj))
            _updateSubscribers.Remove(obj);
    }

    public void Unbind(UnityEngine.Object obj)
    {
        if (obj is IUpdateLockStep updater && updater != null && _updateUnitySubscribers.ContainsKey(obj))
            _updateUnitySubscribers.Remove(obj);
    }

    public void ClearAllBinds()
    {
        _updateSubscribers.Clear();
        _updateUnitySubscribers.Clear();
        _updateMethods.Clear();
    }

    public void OnUpdateFrameForBinded(Frame input, Frame output)
    {
        foreach (var kv in _updateSubscribers)
        {
            kv.Value.OnUpdateLockStep(input, output);
        }
        foreach (var kv in _updateUnitySubscribers)
        {
            kv.Value.OnUpdateLockStep(input, output);
        }

        _expectedParamArray[0] = input;
        _expectedParamArray[1] = output;
        foreach (var kv in _updateMethods)
        {
            foreach (var methodInfo in kv.Value)
            {
                Invoker.Invoke(kv.Key, methodInfo, _expectedParamArray, _paramOptions, _ => _paramArray);
            }
        }
    }
}