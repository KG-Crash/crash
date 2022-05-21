using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
	// Actor Pool
    public class ProjectileActorPool
    {
        // Start is called before the first frame update
        private Dictionary<int, Queue<ProjectileActor>> _objectQueueDict;
        private ProjectileTable _prefabTable;
        private uint _initialCreateQuantity;
        private ProjectileActor.Listener _listener;
        private Transform _poolOffset;

        public ProjectileActorPool(ProjectileTable table, uint len, ProjectileActor.Listener listener, Transform poolOffset)
		{
            _initialCreateQuantity = len;
            _prefabTable = table;
            _listener = listener;
            _poolOffset = poolOffset;
            _objectQueueDict = new Dictionary<int, Queue<ProjectileActor>>();

            foreach(var kv in _prefabTable.GetEnumerable())
			{
                _objectQueueDict[kv.Key] = new Queue<ProjectileActor>();
                for(int i = 0; i < _initialCreateQuantity; i++)
                    _objectQueueDict[kv.Key].Enqueue(CreateNewProjectile(kv.Key));
			}
		}

        private ProjectileActor CreateNewProjectile(int type)
		{
            var projectile = ProjectileActorFactory.CreateProjectileActor(type, _prefabTable, _listener);
            ClearProjectile(projectile);
            return projectile;
		}

        public ProjectileActor GetProjectileActor(int type)
        {
	        if (!_objectQueueDict.ContainsKey(type))
	        {
		        return null;
	        }
	        var projectileActor = _objectQueueDict[type].Count > 0 ? _objectQueueDict[type].Dequeue() : CreateNewProjectile(type);
			projectileActor.gameObject.SetActive(true);
            return projectileActor;
        }
        
        public void ReturnProjectile(int type, ProjectileActor projectileActor)
		{
            ClearProjectile(projectileActor);
            _objectQueueDict[type].Enqueue(projectileActor);
		}

        private void ClearProjectile(ProjectileActor projectile)
		{
			// TODO :: 로지컬 오브젝트까지 풀링 ㅎ쉴?
            // projectile.Disable();
            projectile.SetParent(_poolOffset);
            projectile.position = FixMath.NET.FixVector3.Zero;
            projectile.gameObject.SetActive(false);
        }
    }
}