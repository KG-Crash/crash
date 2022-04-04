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
        public void SpawnUnit(int unitType, uint count, int? x = null, int? y = null)
        {
            // TODO : 로직은 GameController.Action.cs에 정의하고 (액션 프로토콜을 수신할 때)
            // 여기서는 EnqueueAction만 한다.

            for (int i = 0; i < count; i++)
            {
                if(x is null || y is null) 
                    EnqueueSpawn((uint)unitType, count, new FixVector2(_spawnPositions[_playerID].position));
                else
                    EnqueueSpawn((uint)unitType, count, new FixVector2(_spawnPositions[_playerID].position) + new FixVector2((int)x,(int)y));
            }
        }

        [BuildCommand("attack to")]
        public void AttackTo(int targetPlayerNumber)
        {
            // TODO : 로직은 GameController.Action.cs에 정의하고 (액션 프로토콜을 수신할 때)
            // 여기서는 EnqueueAction만 한다.

            EnqueueAttack((uint)targetPlayerNumber);
        }

        [BuildCommand("faster")]
        public void Faster(int times = 2)
        {
            Debug.Log($"현재 {TimeSpeed} 배속");
            EnqueueSpeed(times);
        }

		[BuildCommand("pause")]
		public void Pause(bool pause)
		{
            if (pause)
            {
                EnqueuePause();   
            }
            else
            {
                SendResume();
            }
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
