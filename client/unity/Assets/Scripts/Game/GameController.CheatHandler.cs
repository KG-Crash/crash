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
            for (int i = 0; i < count; i++)
            {
                if (x.HasValue == false)
                    x = 0;

                if (y.HasValue == false)
                    y = 0;

                EnqueueSpawn(unitType, new FixVector2(Fix64.One * x.Value, Fix64.One * y.Value));
                // TODO: spawn position 복구해야함
            }
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
