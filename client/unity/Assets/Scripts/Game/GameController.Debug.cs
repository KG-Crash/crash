using FixMath.NET;
using Shared;
using UnityEngine;

namespace Game
{
    public partial class GameController 
    {
        [Header("Debug")]
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
        
        void UpdateForDebug()
        {
            if (Input.GetKeyUp(KeyCode.Alpha1))
                _player.StartUpgrade(Ability.UPGRADE_1);
            if (Input.GetKeyUp(KeyCode.Alpha2))
                _player.StartUpgrade(Ability.UPGRADE_2);
            if (Input.GetKeyUp(KeyCode.Alpha3))
                _player.StartUpgrade(Ability.UPGRADE_3);
            if (Input.GetKeyUp(KeyCode.Alpha4))
                _player.StartUpgrade(Ability.UPGRADE_4);
            if (Input.GetKeyUp(KeyCode.Alpha5))
                _player.StartUpgrade(Ability.UPGRADE_5);
            if (Input.GetKeyUp(KeyCode.Alpha6))
                _player.StartUpgrade(Ability.UPGRADE_6);
            if (Input.GetKeyUp(KeyCode.Alpha7))
                _player.StartUpgrade(Ability.UPGRADE_7);
            if (Input.GetKeyUp(KeyCode.Alpha8))
                _player.StartUpgrade(Ability.UPGRADE_8);
            if (Input.GetKeyUp(KeyCode.Alpha9))
                _player.StartUpgrade(Ability.UPGRADE_9);

            if (Input.GetKeyDown(KeyCode.Space))
            {
                _player.targetPlayerID = 1;
            }
            else if (Input.GetKeyUp(KeyCode.Space))
            {
                _player.targetPlayerID = null;
            }
        }
    }
}