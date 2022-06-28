using FixMath.NET;
using Shared;
using UnityEngine;

namespace Game
{
    public partial class GameController
    {
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
        public FixVector3 ScreenMiddlePositionToWorldPosition()
        {
            var objs = UnityResources._instance.Get("Objects");
            var cam = objs.GetCamera();
            var ray = cam.ScreenPointToRay(new Vector2(cam.pixelWidth / 2, cam.pixelHeight / 2));
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

        void OnUpdateAlwaysDebug()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                _me.target = _teams.Find(1);
            }
            else if (Input.GetKeyUp(KeyCode.Space))
            {
                _me.target = null;
            }
            
            if (Input.GetKeyUp(KeyCode.F1))
            {
                if (paused)
                {
                    SendResume();   
                }
                else
                {
                    EnqueuePause();
                }
            }

            if (Input.GetKeyUp(KeyCode.F2))
            {
                EnqueueSpeed(1);
            }
            if (Input.GetKeyUp(KeyCode.F3))
            {
                EnqueueSpeed(2);
            }
            if (Input.GetKeyUp(KeyCode.F4))
            {
                EnqueueSpeed(3);
            }
        }

        void OnUpdateFrameDebug(Frame f)
        {
            if (Input.GetKeyUp(KeyCode.Alpha1))
                _me.upgrade.Start(Ability.UPGRADE_1, f.currentFrame);
            if (Input.GetKeyUp(KeyCode.Alpha2))
                _me.upgrade.Start(Ability.UPGRADE_2, f.currentFrame);
            if (Input.GetKeyUp(KeyCode.Alpha3))
                _me.upgrade.Start(Ability.UPGRADE_3, f.currentFrame);
            if (Input.GetKeyUp(KeyCode.Alpha4))
                _me.upgrade.Start(Ability.UPGRADE_4, f.currentFrame);
            if (Input.GetKeyUp(KeyCode.Alpha5))
                _me.upgrade.Start(Ability.UPGRADE_5, f.currentFrame);
            if (Input.GetKeyUp(KeyCode.Alpha6))
                _me.upgrade.Start(Ability.UPGRADE_6, f.currentFrame);
            if (Input.GetKeyUp(KeyCode.Alpha7))
                _me.upgrade.Start(Ability.UPGRADE_7, f.currentFrame);
            if (Input.GetKeyUp(KeyCode.Alpha8))
                _me.upgrade.Start(Ability.UPGRADE_8, f.currentFrame);
            if (Input.GetKeyUp(KeyCode.Alpha9))
                _me.upgrade.Start(Ability.UPGRADE_9, f.currentFrame);
        }
    }
}