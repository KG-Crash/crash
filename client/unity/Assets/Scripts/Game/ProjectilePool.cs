using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class ProjectilePool
    {
        // Start is called before the first frame update
        private Dictionary<int, Queue<Projectile>> _objectQueueDict;
        private ProjectileTable _prefabTable;
        private uint _initialCreateQuantity;
        private Projectile.Listener _listener;
        private Transform _poolOffset;

        public ProjectilePool(ProjectileTable table, uint len, Projectile.Listener listener, Transform poolOffset)
		{
            _initialCreateQuantity = len;
            _prefabTable = table;
            _listener = listener;
            _poolOffset = poolOffset;
            _objectQueueDict = new Dictionary<int, Queue<Projectile>>();

            var projectileOriginArray = _prefabTable.projectileOrigin;
            foreach(var origin in projectileOriginArray)
			{
                _objectQueueDict[origin.projectileOriginID] = new Queue<Projectile>();
                for(int i = 0; i < _initialCreateQuantity; i++)
                    _objectQueueDict[origin.projectileOriginID].Enqueue(CreateNewProjectile(origin.projectileOriginID));
			}
		}

        private Projectile CreateNewProjectile(int projectileOriginID)
		{
            Debug.Log("create in pool");
            var projectile = ProjectileFactory.CreateNewProjectile(projectileOriginID, _prefabTable, _listener);
            ClearProjectile(projectile);
            return projectile;
		}

        public Projectile GetProjectile(int projectileOriginID, Unit owner, Unit target) 
		{
            Projectile projectile;

            if(_objectQueueDict[projectileOriginID].Count > 0)
                projectile = _objectQueueDict[projectileOriginID].Dequeue();
			else
                projectile = CreateNewProjectile(projectileOriginID);

            InitProjectile(projectile,owner, target );
            return projectile;
        }
        public void ReturnProjectile(Projectile projectile)
		{
            ClearProjectile(projectile);
            _objectQueueDict[projectile.projectileOriginID].Enqueue(projectile);
		}

        private void InitProjectile(Projectile projectile, Unit owner, Unit target)
		{
            projectile.gameObject.SetActive(true);
            projectile.transform.SetParent(owner.transform);
            projectile.position = owner.position;
            projectile.owner = owner;
            projectile.target = target;
            projectile.damage = owner.damage;
            projectile.Shoot();  
        }
        private void ClearProjectile(Projectile projectile)
		{
            projectile.Disable();
            projectile.transform.SetParent(_poolOffset);
            projectile.position = FixMath.NET.FixVector3.Zero;
            projectile.gameObject.SetActive(false);
        }
    }
}