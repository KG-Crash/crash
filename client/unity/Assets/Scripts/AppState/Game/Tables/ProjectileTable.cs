using System;
using System.Collections.Generic;
using KG;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "ProjectileTable", menuName = "Crash/ProjectileTable", order = 1)]
    public class ProjectileTable : UnityTable<ProjectileTable>
    {
        [SerializeField] private SerializableDictionary<int, ProjectileActor> _projectiles;

        public ProjectileActor GetProjectileActor(int projectileOriginID)
        {
            return _projectiles[projectileOriginID];
        }
        
        public IEnumerable<KeyValuePair<int, ProjectileActor>> GetEnumerable()
        {
            return _projectiles;
        }
    }
}