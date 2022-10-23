using FixMath.NET;
using Game;
using Shared.Type;
using UnityEngine;

public partial class GameState
{
    [Header("Debug")] public int _spawnUnitOriginID = 0;
    public int _spawnPlayerID = 0;

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
            me.target = teams.Find(1);
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            me.target = null;
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
}