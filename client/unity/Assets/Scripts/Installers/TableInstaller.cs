using System.Collections;
using System.Collections.Generic;
using Game;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

[CreateAssetMenu]
public class TableInstaller : ScriptableObjectInstaller<TableInstaller>
{
    [FormerlySerializedAs("_unitTable")] [SerializeField] private UnitTable unitTable;
    [FormerlySerializedAs("_projectileTable")] [SerializeField] private ProjectileTable projectileTable;
    
    public override void InstallBindings()
    {
        Container.Bind<UnitTable>().FromInstance(unitTable);
        Container.Bind<ProjectileTable>().FromInstance(projectileTable);
    }
}
