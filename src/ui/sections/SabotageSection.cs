using System.Collections.Generic;
using UnityEngine;

namespace HydraMenu.ui.sections
{
	internal class SabotageSection : ISection
	{
		public SabotageSection() : base("破坏") { }

		public override void Render()
		{
			if(ShipStatus.Instance == null)
			{
				GUILayout.Label("你当前不在游戏中或游戏尚未开始，这些选项将无法使用。");
			}

			Sabotage.UpdateSystemsDirectly = GUILayout.Toggle(Sabotage.UpdateSystemsDirectly, "直接更新破坏系统");

			Dictionary<string, SystemTypes> sabotages = Sabotage.GetSabotages();
			Dictionary<string, SystemTypes> doors = Sabotage.GetDoors();

			GUILayout.BeginHorizontal();
			if(GUILayout.Button("全部破坏"))
			{
				Sabotage.SabotageAll();
				Hydra.notifications.Send("破坏", "所有破坏已启用。", 5);
			}

			if(GUILayout.Button("关闭所有门"))
			{
				Sabotage.LockAll();
				Hydra.notifications.Send("破坏", "所有门已关闭。", 5);
			}
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			if(GUILayout.Button("修复所有破坏"))
			{
				Sabotage.FixAllSabotages();
				Hydra.notifications.Send("破坏", "所有破坏已修复。", 5);
			}

			if(GUILayout.Button("解锁所有门"))
			{
				if(Sabotage.CanUnlockDoors())
				{
					Sabotage.UnlockAll();
					Hydra.notifications.Send("破坏", "所有门已解锁。", 5);
				} else {
					Hydra.notifications.Send("破坏", "当前地图不支持解锁门。", 10);
				}
			}
			GUILayout.EndHorizontal();

			GUILayout.Space(5);
			GUILayout.Label("破坏项 (左键破坏/右键修复):");
			foreach(var (key, value) in sabotages)
			{
				if(GUILayout.Button(key))
				{
					HandleSabotage(value);
				}
			}

			GUILayout.Label("关门:");
			if(doors.Count == 0)
			{
				GUILayout.Label("当前地图没有可关闭的门。");
				return;
			}

			byte i = 0;
			foreach(var (key, value) in doors)
			{
				if(i % 2 == 0)
				{
					GUILayout.BeginHorizontal();
				}

				if(GUILayout.Button(key))
				{
					Sabotage.LockDoor(value);
				}

				if(i % 2 != 0)
				{
					GUILayout.EndHorizontal();
				}

				i++;
			}

			if(i % 2 != 0)
			{
				GUILayout.EndHorizontal();
			}
		}

		private void HandleSabotage(SystemTypes system)
		{
			Event currentEvent = Event.current;

			if(currentEvent.button == 0)
			{
				Sabotage.SabotageSystem(system);
				Hydra.notifications.Send("破坏", $"{system} 已被破坏。", 5);
			}
			else if(currentEvent.button == 1)
			{
				Sabotage.FixSabotage(system);
				Hydra.notifications.Send("破坏", $"{system} 已修复。", 5);
			}
		}
	}
}
