using System;
using System.Collections;
using System.Collections.Generic;
using FixMath.NET;
using UnityEngine;


namespace Game
{
	[AttributeUsage(AttributeTargets.Method)]
	public class BuildCommandAttribute : Attribute
	{
		public string command { get; private set; }
		public BuildCommandAttribute(string command)
		{
			this.command = command;
		}
	}

    public partial class GameController
    {
        [BuildCommand("spawn unit")]
        public static void SpawnUnit(int unitOriginId, uint count, int playerNumber = -1)
        {
            Fix64 _startRadian = Fix64.Zero;
            FixVector2 rot;

            if (playerNumber == -1)
                rot = ist.GetSpawnRotation(ist._player.spawnIndex);
            else
                rot = ist.GetSpawnRotation(playerNumber);

            var ctx = new TemporalPlaceContext() { _startRadian = Fix64.Pi / 180.0f * rot.y };
            var positionWS = ist.ScreenMiddlePositionToWorldPosition();

            Debug.Log("유닛 스폰 유닛id : " + unitOriginId + "갯수 : " + count);
            if (playerNumber == -1)
            {
                for (int i = 0; i < count; i++)
                    ist.SpawnUnitToPosition(unitOriginId, ist.GetPlayer((uint)ist._playerID), positionWS, ctx);
            }
            else
            {
                for (int i = 0; i < count; i++)
                    ist.SpawnUnitToPlayerStart(unitOriginId, ist.GetPlayer((uint)playerNumber), ctx);
            }
        }

        [BuildCommand("attack to")]
        public static void AttackTo(int targetPlayerNumber, params int[] unitOriginIds)
        {
            if (ist._playerID == targetPlayerNumber)
                return;

            Debug.Log($"어택땅 타겟 플레이어 : {targetPlayerNumber}");
            foreach (var unitID in unitOriginIds)
            {
                Debug.Log(unitID);
            }

            Player player = ist.GetPlayer((uint)targetPlayerNumber);
            var targetPosition = ist.GetSpawnPosition(player.spawnIndex);

            if (unitOriginIds.Length == 0)
            {
                player.targetPlayerID = (uint)targetPlayerNumber;
            }
            else
            {
                foreach (var unit in player.units)
                {
                    for (int i = 0; i < unitOriginIds.Length; i++)
                    {
                        if (unit.unitOriginID == unitOriginIds[i])
                        {
                            unit.MoveTo(targetPosition);
                        }
                    }
                }
            }
        }

        [BuildCommand("faster")]
        public static void Faster(int times = 2)
        {
            GameController.TimeSpeed *= times;
            Debug.Log($"현재 {GameController.TimeSpeed} 배속");
        }
		[BuildCommand("setfps")]
		public static void SetFPS(int fps)
		{
			if (!IsNetworkMode)
				FPS = fps;
		}
		
		[BuildCommand("refreshfps")]
		public static void RefreshFPS()
		{
			if (!IsNetworkMode)
				FPS = Shared.Const.Time.FPS;
		}

		[BuildCommand("settps")]
		public static void SetTPS(int tps)
		{
			if (!IsNetworkMode)
				TPS = tps;
		}

		[BuildCommand("refreshtps")]
		public static void RefreshTPS()
		{
			if (!IsNetworkMode)
				TPS = Shared.Const.Time.TPS;
		}
	}
}
