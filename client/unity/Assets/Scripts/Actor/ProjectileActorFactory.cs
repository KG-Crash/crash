using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace Game
{
    public static class ProjectileActorFactory
    {
        public static ProjectileActor CreateProjectileActor(int projectileOriginID, ProjectileTable projectileTable, ProjectileActor.Listener listener)
        {
            var projectileOrigin = projectileTable.GetProjectileActor(projectileOriginID);
            var projectile = Object.Instantiate(projectileOrigin);
            projectile.Init(listener);
            return projectile;
        }
    }
}