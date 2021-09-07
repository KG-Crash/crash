using FixMath.NET;
using UnityEngine;

namespace Game
{
    public partial class GameController 
    {
        [Header("Debug")]
        public Transform spawnMine;
        public Transform spawnEnemy;

        public int _spawnUnitOriginID = 0;
        public uint _spawnPlayerID = 0;
        
        public FixVector3 ScreenPositionToWorldPosition(Vector2 ss)
        {
            var objs = UnityResources._instance.Get("Objects");
            var cam = objs.GetCamera();
            var ray = cam.ScreenPointToRay(ss);
            if (FixVector3.Dot(ray.direction, FixVector3.Down) > 0)
            {
                var t = -ray.origin.y / ray.direction.y;
                return ray.GetPoint(t);
            }
            else
            {
                return FixVector3.Zero;
            }
        }
    }
}