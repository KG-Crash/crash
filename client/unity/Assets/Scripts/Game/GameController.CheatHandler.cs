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
        public void SpawnUnit(int unitOriginId, uint count, int playerNumber = -1)
        {
            Fix64 _startRadian = Fix64.Zero;
            FixVector2 rot;

            if (playerNumber == -1)
                rot = GetSpawnRotation(_player.spawnIndex);
            else
                rot = GetSpawnRotation(playerNumber);

            var ctx = new TemporalPlaceContext();
            var positionWS = ScreenMiddlePositionToWorldPosition();

            Debug.Log($"유닛 스폰 유닛id : {unitOriginId} + 갯수 : { count}");
            if (playerNumber == -1)
            {
                for (int i = 0; i < count; i++)
                    SpawnUnitToPosition(unitOriginId, GetPlayer((uint)_playerID), positionWS, ctx);
            }
            else
            {
                for (int i = 0; i < count; i++)
                    SpawnUnitToPlayerStart(unitOriginId, GetPlayer((uint)playerNumber), ctx);
            }
        }

        [BuildCommand("attack to")]
        public void AttackTo(int targetPlayerNumber, params int[] unitOriginIds)
        {
            if (_playerID == targetPlayerNumber)
                return;

            Debug.Log($"어택땅 타겟 플레이어 : {targetPlayerNumber}");
            foreach (var unitID in unitOriginIds)
            {
                Debug.Log(unitID);
            }

            Player player = GetPlayer((uint)targetPlayerNumber);
            var targetPosition = GetSpawnPosition(player.spawnIndex);

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
        public void Faster(int times = 2)
        {
            GameController.TimeSpeed *= times;
            Debug.Log($"현재 {GameController.TimeSpeed} 배속");
        }
		[BuildCommand("setfps")]
		public void SetFPS(int fps)
		{
			if (!IsNetworkMode)
				FPS = fps;
		}
		
		[BuildCommand("refreshfps")]
		public void RefreshFPS()
		{
			if (!IsNetworkMode)
				FPS = Shared.Const.Time.FPS;
		}

		[BuildCommand("settps")]
		public void SetTPS(int tps)
		{
			if (!IsNetworkMode)
				TPS = tps;
		}

		[BuildCommand("refreshtps")]
		public void RefreshTPS()
		{
			if (!IsNetworkMode)
				TPS = Shared.Const.Time.TPS;
		}
	}
}
