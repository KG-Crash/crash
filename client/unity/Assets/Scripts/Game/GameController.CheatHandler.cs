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
		public static void OperationCwal(int value1, int value2)
		{
			Debug.Log("빠른생산 param1 : " + value1 + ", param2 : " + value2);
		}

		[BuildCommand("power overwhelming")]
		public static void PowerOverwhelming(string value1, string value2)
		{
			Debug.Log("무적 param1 : " + value1 + ", param2 : " + value2);
		}

		[BuildCommand("show me the money")]
		public static void ShowMeTheMoney(int value1, int value2)
		{
			Debug.Log("쇼미더머니 param1 : " + value1 + ", param2 : " + value2);
		}

		[BuildCommand("test cheat")]
		public static void TestCheat(string value1, int value2, string value3)
        {
			Debug.Log("테스트 치트 param1 : " + value1 + ", param2 : " + value2 + ", param3 : " + value3);
		}

		[BuildCommand("empty cheat")]
		public static void EmptyCheat()
		{
			Debug.Log("테스트 치트 인자없는");
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