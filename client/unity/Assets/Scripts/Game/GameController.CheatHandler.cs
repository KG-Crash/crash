using System;
using System.Collections;
using System.Collections.Generic;
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
		[BuildCommand("operation cwal")]
		public static void OperationCwal(params int[] playerNumber)
		{
			foreach (var player in playerNumber)
			{
				Debug.Log("빠른생산 target player : " + player);
			}
		}

		[BuildCommand("power overwhelming")]
		public static void PowerOverwhelming(params int[] playerNumber)
		{
			foreach (var player in playerNumber)
			{
				Debug.Log("무적 target player : " + player);
			}
		}

		[BuildCommand("show me the money")]
		public static void ShowMeTheMoney(params int[] playerNumber)
		{
			foreach (var player in playerNumber)
			{
				Debug.Log("쇼미더머니 target player : " + player);
			}
		}
	}
}