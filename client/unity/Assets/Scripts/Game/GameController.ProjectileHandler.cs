using System.Collections;
using System.Collections.Generic;
using FixMath.NET;
using UnityEngine;

namespace Game
{
    public partial class GameController : Projectile.Listener, ProjectileActor.Listener
    {
        // <projectileID, targetUnit>
        public Dictionary<Projectile, Unit> fireHistory { get; set; } = new Dictionary<Projectile, Unit>();

        public void OnSpawned(Projectile projectile)
        {
            if (!unitActorMaps.TryGetValue(projectile.owner, out var unitActor))
                return;
            
            var actor = _projectileActorPool.GetProjectileActor(projectile.type);
            actor.SetParent(unitActor.parent);
            actor.position = projectile.position;
            unitActorMaps.Add(projectile, actor);
        }
        
        public void OnProjectileReach(Projectile projectile, Unit target)
        {
            if (fireHistory.TryGetValue(projectile, out var fireUnit))
            {
                if (!Unit.IsNullOrDead(fireUnit))
                {
                    if (target != null)
                        target.AddAttacker(fireUnit);

                    var damage = projectile.damage;
                    fireUnit.AddHP(-damage, fireUnit);
                }
                
                fireHistory.Remove(projectile);
            }

            if (projectile.owner != null)
                projectile.owner.projectiles.Remove(projectile.uniqueID);

            var actor = unitActorMaps[projectile];
            if (actor is ProjectileActor projectileActor) 
                _projectileActorPool.ReturnProjectile(projectile.type, projectileActor);
        }
    }
}