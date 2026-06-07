using HydraMenu.features;
using System.Collections.Generic;
using UnityEngine;

namespace HydraMenu.ui.sections
{
	internal class MovementSection : ISection
	{
		public MovementSection() : base("移动") { }

		public override void Render()
		{
			if(PlayerControl.LocalPlayer == null)
			{
				GUILayout.Label("你当前不在游戏中，这些选项将无法使用。");
				GUILayout.Toggle(false, "穿墙模式");
			}
			else
			{
				Vector2 position = PlayerControl.LocalPlayer.transform.position;

				GUILayout.Label($"当前地图: {Utilities.GetCurrentMap()}\n当前位置:\nX: {position.x:F2}\nY: {position.y:F2}");

				PlayerControl.LocalPlayer.Collider.enabled = !GUILayout.Toggle(!PlayerControl.LocalPlayer.Collider.enabled, "穿墙模式");
			}

			GUILayout.Label($"速度倍率: {Self.PlayerSpeedModifier.Multiplier:F2}x");
			Self.PlayerSpeedModifier.Multiplier = GUILayout.HorizontalSlider(Self.PlayerSpeedModifier.Multiplier, 0f, 5f);

			Teleporter.UseSnapToRPC = GUILayout.Toggle(Teleporter.UseSnapToRPC, "使用 SnapTo RPC 传送");
			GUILayout.Label("传送到位置:");

			Dictionary<string, Vector2> teleportLocations = Teleporter.GetTeleportLocations();

			byte i = 0;
			foreach(var (key, value) in teleportLocations)
			{
				if(i % 2 == 0)
				{
					GUILayout.BeginHorizontal();
				}

				if(GUILayout.Button(key))
				{
					Teleporter.TeleportTo(value);
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
	}
}
