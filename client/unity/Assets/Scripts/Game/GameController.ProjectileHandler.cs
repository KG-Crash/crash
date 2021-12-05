using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public partial class GameController : Projectile.Listener
    {
        public Dictionary<uint, Unit> fireHistory { get; set; } = new Dictionary<uint, Unit>(); // <projectileID, targetUnit>

        public void OnProjectileReach(Projectile projectile, Unit target)
        {
            if (projectile.currentState != Shared.ProjectileState.Hit)
                return;

            if (fireHistory.TryGetValue(projectile.projectileID, out var fireUnit))
            {
                if (!Unit.IsNullOrDead(fireUnit))
                {
                    if (target != null)
                        target.AddAttacker(fireUnit);

                    var damage = projectile.damage;
                    fireUnit.AddHP(-damage, fireUnit);
                }
            } 

            _projectilePool.ReturnProjectile(projectile);
            fireHistory.Remove(projectile.projectileID);
        }
    }
}