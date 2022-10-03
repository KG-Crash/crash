using System;
using System.Collections.Generic;
using System.Linq;
using FixMath.NET;
using Game;
using Game.Service;
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

interface IUpdateLockStep
{
    void OnUpdateLockStep(Frame input, Frame output);
}

public partial class GameState
{
    private Dictionary<object, IUpdateLockStep> _updateSubscribers = new Dictionary<object, IUpdateLockStep>();
    private Dictionary<UnityEngine.Object, IUpdateLockStep> _updateUnitySubscribers = new Dictionary<UnityEngine.Object, IUpdateLockStep>();

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
    }
}