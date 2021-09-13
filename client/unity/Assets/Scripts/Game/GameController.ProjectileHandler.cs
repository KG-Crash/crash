using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public partial class GameController : Projectile.Listener
    {
        public Dictionary<uint, Unit> fireHistory { get; set; } = new Dictionary<uint, Unit>();
        // Start is called before the first frame update
        public void OnProjectileReach(Projectile projectile)
        {
            if (projectile.currentState != Shared.ProjectileState.Hit)
                return;

            if (fireHistory.ContainsKey(projectile.projectileID))
            {
                if (!fireHistory[projectile.projectileID].IsDead || projectile.owner != null)
                {
                    var damage = projectile.owner.damage;
                    fireHistory[projectile.projectileID].AddHP(-damage, projectile.owner);
                }                
                _projectilePool.ReturnProjectile(projectile);
                fireHistory.Remove(projectile.projectileID);
            }
        }
    }
}