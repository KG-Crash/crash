using System;
using System.Collections;
using System.Collections.Generic;
using Module;
using UnityEngine;
using Zenject;

public class CrashLobbyInstaller : MonoInstaller<CrashLobbyInstaller>
{
    [SerializeField] private Transform canvas;
    
    public override void InstallBindings()
    {
        Container.BindInterfacesAndSelfTo<LobbyController>().AsSingle();

        Container.Bind<UI.IntroView>()
            .FromComponentInNewPrefabResource(PathOfPrefab<UI.IntroView>())
            .UnderTransform(canvas)
            .AsSingle();
        Container.BindInterfacesAndSelfTo<UI.IntroPanel>().AsSingle();
    }

    private static string PathOfPrefab<T>() where T : class
    {
        return typeof(T).FullName.Replace(".", "/");
    }
}