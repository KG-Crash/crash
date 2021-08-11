using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public static class ProjectileFactory
    {
        public static Projectile GetNewProjectile(int projectileOriginID, Unit ownerUnit, uint ownerUnitID, ProjectileTable projectileTable)
        {
            var projectileOrigin = projectileTable.GetOrigin(projectileOriginID);
            var projectile = Object.Instantiate(projectileOrigin);
            projectile.transform.position = ownerUnit.position;
            projectile.ownerUnitID = ownerUnitID;
            Debug.Log("bullet create");
            return projectile;
        }
    }
}