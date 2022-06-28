using System.Collections;
using System.Collections.Generic;
using Module;
using Network;
using UnityEngine;
using Zenject;

public class GlobalNetworkInstaller : MonoInstaller<GlobalNetworkInstaller>
{
    public override void InstallBindings()
    {
        Container.Bind<Dispatcher>()
            .FromComponentInNewPrefabResource(nameof(Dispatcher))
            .AsSingle()
            .NonLazy();

        Container.Bind<Client>().FromInstance(Client.Instance);
    }
}
