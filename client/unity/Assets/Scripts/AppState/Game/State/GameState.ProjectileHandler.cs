using System.Collections;
using System.Collections.Generic;
using FixMath.NET;
using UnityEngine;
using Game;

public partial class GameState : Projectile.Listener, ProjectileActor.Listener
{
    // <projectileID, targetUnit>
    public Dictionary<Projectile, Unit> _fireHistory { get; set; }
    public ProjectileCollection _projectiles;

    private void InitializeProjectileHandle()
    {
        _fireHistory = new Dictionary<Projectile, Unit>();
        _projectiles = new ProjectileCollection(null, this);
    }

    public void OnSpawned(Projectile projectile)
    {
        Bind(projectile);
        _projectiles.Append(projectile);

        if (!unitActorMaps.TryGetValue(projectile.owner, out var unitActor))
            return;

        var actor = projectileActorPool.GetProjectileActor(projectile.type);
        actor.SetParent(unitActor.parent);
        actor.position = projectile.position;
        unitActorMaps.Add(projectile, actor);
    }

    public void OnProjectileReach(Projectile projectile, Unit target)
    {
        _projectiles.Delete(projectile);

        if (_fireHistory.TryGetValue(projectile, out var fireUnit))
        {
            if (!Unit.IsNullOrDead(fireUnit))
            {
                if (target != null)
                    target.AddAttacker(fireUnit);

                var damage = projectile.damage;
                fireUnit.AddHP(-damage, fireUnit);
            }

            _fireHistory.Remove(projectile);
        }

        if (projectile.owner != null)
            projectile.owner.projectiles.Delete(projectile);

        var actor = unitActorMaps[projectile];
        if (actor is ProjectileActor projectileActor)
            projectileActorPool.ReturnProjectile(projectile.type, projectileActor);
    }
}