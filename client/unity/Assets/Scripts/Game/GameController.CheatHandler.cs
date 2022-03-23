using System;
using System.Collections;
using System.Collections.Generic;
using FixMath.NET;
using Shared;
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
            // TODO : 로직은 GameController.Action.cs에 정의하고 (액션 프로토콜을 수신할 때)
            // 여기서는 EnqueueAction만 한다.
            Fix64 _startRadian = Fix64.Zero;
            FixVector2 rot;

            if (playerNumber == -1)
                rot = GetSpawnRotation(_player.spawnIndex);
            else
                rot = GetSpawnRotation(playerNumber);

            var ctx = new TemporalPlaceContext();
            var positionWS = ScreenMiddlePositionToWorldPosition();
            Player player;

            Debug.Log($"유닛 스폰 유닛id : {unitOriginId} + 갯수 : { count}");
            if (playerNumber == -1)
            {
                player = GetPlayer((uint)_playerID);
                for (int i = 0; i < count; i++)
                    SpawnUnitToPosition(unitOriginId, player, positionWS, ctx);
            }
            else
            {
                player = GetPlayer((uint)playerNumber);
                for (int i = 0; i < count; i++)
                    SpawnUnitToPlayerStart(unitOriginId, player, ctx);
            }
        }

        [BuildCommand("attack to")]
        public void AttackTo(int targetPlayerNumber, params int[] unitOriginIds)
        {
            // TODO : 로직은 GameController.Action.cs에 정의하고 (액션 프로토콜을 수신할 때)
            // 여기서는 EnqueueAction만 한다.
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
            Debug.Log($"현재 {TimeSpeed} 배속");
	        TimeSpeed = Fix64.One * times;
        }

		[BuildCommand("pause")]
		public void SetPause(bool pause)
		{
            paused = pause;
		}

        [BuildCommand("upgrade")]
        public void Upgrade(string value)
        {
            if (Enum.TryParse<Ability>(value, out var ability) == false)
                return;

            EnqueueUpgrade(ability);
        }
	}
}
