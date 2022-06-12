using UnityEngine;

namespace Game
{
    public static class ProjectileActorFactory
    {
        public static ProjectileActor CreateProjectileActor(int projectileOriginID, ProjectileTable projectileTable)
        {
            var projectileOrigin = projectileTable.GetProjectileActor(projectileOriginID);
            var projectile = Object.Instantiate(projectileOrigin);
            projectile.Init();
            return projectile;
        }
    }
}