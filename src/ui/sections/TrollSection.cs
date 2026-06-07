using HydraMenu.features;
using UnityEngine;

namespace HydraMenu.ui.sections
{
	internal class TrollSection : ISection
	{
		public TrollSection() : base("恶搞") { }

		public override void Render()
		{
			if(PlayerControl.LocalPlayer == null)
			{
				GUILayout.Label("你当前不在游戏中，这些选项将无法使用。");
			}

			Troll.AutoReportBodies.source = Controls.PlayerSpecificToggle("自动报告尸体", PlayerControl.LocalPlayer, Troll.AutoReportBodies.source);
			Hydra.routines.autoTriggerSpores.Enabled = GUILayout.Toggle(Hydra.routines.autoTriggerSpores.Enabled, "自动触发孢子");
			Troll.BlockSabotages.Enabled = GUILayout.Toggle(Troll.BlockSabotages.Enabled, "阻止破坏");
			Troll.BlockVenting.Enabled = GUILayout.Toggle(Troll.BlockVenting.Enabled, "禁用管道");

			if(GUILayout.Button("触发所有孢子"))
			{
				if(Utilities.GetCurrentMap() != MapNames.Fungle)
				{
					Hydra.notifications.Send("触发孢子", "此功能仅适用于 Fungle 地图。");
				}
				else
				{
					FungleShipStatus shipStatus = ShipStatus.Instance.Cast<FungleShipStatus>();

					foreach(Mushroom mushroom in shipStatus.sporeMushrooms.Values)
					{
						PlayerControl.LocalPlayer.RpcTriggerSpores(mushroom);
					}

					Hydra.notifications.Send("触发孢子", "所有孢子已触发。", 5);
				}
			}

			if(GUILayout.Button("复制随机玩家外观"))
			{
				PlayerControl randomPl = Utilities.GetRandomPlayer();
				Utilities.CopyPlayer(randomPl);
			}

			GUILayout.Space(5);

			GUILayout.Label("门板整活:");
			Hydra.routines.doorTroller.Enabled = GUILayout.Toggle(Hydra.routines.doorTroller.Enabled, "启用");

			GUILayout.Label($"开关门延迟: {Hydra.routines.doorTroller.lockAndUnlockDelay:F2}秒");
			Hydra.routines.doorTroller.lockAndUnlockDelay = GUILayout.HorizontalSlider(Hydra.routines.doorTroller.lockAndUnlockDelay, 0.1f, 2.0f);
		}
	}
}
