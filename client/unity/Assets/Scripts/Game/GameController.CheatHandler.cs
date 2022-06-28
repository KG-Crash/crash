using System;
using System.Collections;
using System.Collections.Generic;
using FixMath.NET;
using Network;
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
            FixVector2 spawnCenterPos;
            if (x.HasValue == false || y.HasValue == false)
            {
                var mySpawnPos = (FixVector3)spawnPositions[Client.Instance.id].position;
                spawnCenterPos = new FixVector2(mySpawnPos.x, mySpawnPos.z);
            }
            else
                spawnCenterPos = new FixVector2(Fix64.One *x.Value, Fix64.One *y.Value);

            EnqueueSpawn(unitType, count, spawnCenterPos);
        }

        [BuildCommand("attack to")]
        public void AttackTo(int targetPlayerNumber)
        {
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
