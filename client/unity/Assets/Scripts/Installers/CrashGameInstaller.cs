using System;
using System.Collections;
using System.Collections.Generic;
using Game;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

public class CrashGameInstaller : MonoInstaller<CrashGameInstaller>
{        
    [SerializeField] private Transform projectilePoolParent; 
    [SerializeField] private Transform focusTransform;
        
    [SerializeField] private KG.Map map;
    [SerializeField] private bool networkMode;
    [SerializeField] private Transform[] spawnPositions;

    public override void InstallBindings()
    {
        Container.BindInterfacesAndSelfTo<GameController>().AsSingle().NonLazy();
        Container.Bind<Transform>()
            .FromInstance(projectilePoolParent)
            .When(c => IsEqualStr(c.MemberName, nameof(projectilePoolParent)))
            .Lazy();
        Container.Bind<Transform>()
            .FromInstance(focusTransform)
            .When(c => IsEqualStr(c.MemberName, nameof(focusTransform)))
            .Lazy();
        Container.Bind<KG.Map>().FromInstance(map);
        Container.Bind<bool>()
            .FromInstance(networkMode)
            .When(c => IsEqualStr(c.MemberName, nameof(networkMode)))
            .Lazy();
        Container.Bind<Transform[]>()
            .FromInstance(spawnPositions)
            .When(c => IsEqualStr(c.MemberName, nameof(spawnPositions)))
            .Lazy();
    }

    private static bool IsEqualStr(string lhs, string rhs)
    {
        return string.Compare(lhs, rhs, StringComparison.InvariantCultureIgnoreCase) == 0;
    }
}
