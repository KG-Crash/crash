using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public partial class GameController : Projectile.Listener
    {
        public Dictionary<uint, Unit> fireHistory { get; set; } = new Dictionary<uint, Unit>();
        // Start is called before the first frame update
        public void OnProjectileReach(Unit owner, Projectile projectile)
        {
            if (fireHistory.ContainsKey(projectile.projectileID))
            {
                if (!fireHistory[projectile.projectileID].IsDead)
                {
                    var damage = this._unitPrefabTable.GetOrigin(owner.unitOriginID).damage;
                    fireHistory[projectile.projectileID].AddHP(-damage, owner);
                }                
                _projectilePool.ReturnProjectile(projectile);
                fireHistory.Remove(projectile.projectileID);
            }
        }
    }
}