using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace Game
{
    public static class ProjectileFactory
    {
        private static uint _sequence = 0;

        public static Projectile CreateNewProjectile(int projectileOriginID, ProjectileTable projectileTable, Projectile.Listener listener)
        {
            var projectileOrigin = projectileTable.GetOrigin(projectileOriginID);
            var projectile = Object.Instantiate(projectileOrigin);
            projectile.projectileID = _sequence++;
            projectile.listener = listener;
            projectile.Disable();
            
            var frameUpdateDisposable = GameController.gameFrameStream.Subscribe(projectile.OnUpdateFrame);
            projectile.OnDestroyAsObservable().Subscribe(_ => frameUpdateDisposable.Dispose());

            return projectile;
        }

    }
}