using System;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "ProjectileTable", menuName = "Crash/ProjectileTable", order = 1)]
    public class ProjectileTable : ScriptableObject
    {
        [SerializeField] private Projectile[] _projectileOrigin;

        public Projectile GetOrigin(int projectileOriginID)
        {
            return Array.Find(_projectileOrigin, projetile => projetile.projectileOriginID == projectileOriginID);
        }
    }
}