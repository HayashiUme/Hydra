using BepInEx.Unity.IL2CPP.Utils.Collections;
using HydraMenu.features;
using InnerNet;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace HydraMenu.ui.sections
{
	internal class HostSection : ISection
	{
		public HostSection() : base("主机") { }

		private byte selectedMap = 0;

		public override void Render()
		{
			if(PlayerControl.LocalPlayer == null)
			{
				GUILayout.Label("你当前不在游戏中，这些选项将无法使用。");
			} else if(!AmongUsClient.Instance.AmHost)
			{
				GUILayout.Label("你不是当前房间的主机。使用这些选项要么无效，要么会导致你被反作弊封禁。");
			}

			Host.BanMidGame.Enabled = GUILayout.Toggle(Host.BanMidGame.Enabled, "允许游戏中封禁玩家");

			Host.FlippedSkeld = GUILayout.Toggle(Host.FlippedSkeld, "使用翻转版 Skeld 地图");

			Host.DisableSabotages.Enabled = GUILayout.Toggle(Host.DisableSabotages.Enabled, "禁用破坏");
			Host.DisableCloseDoors.Enabled = GUILayout.Toggle(Host.DisableCloseDoors.Enabled, "禁用关门");
			Host.DisableCameras.Enabled = GUILayout.Toggle(Host.DisableCameras.Enabled, "禁用监控摄像头");
			Host.DisableGameEnd.Enabled = GUILayout.Toggle(Host.DisableGameEnd.Enabled, "禁止游戏结束");
			Host.NoKillCooldown.Enabled = GUILayout.Toggle(Host.NoKillCooldown.Enabled, "无杀人冷却");

			GUILayout.BeginHorizontal();
			Host.BlockLowLevels.Enabled = GUILayout.Toggle(Host.BlockLowLevels.Enabled, $"踢出等级低于 {Host.BlockLowLevels.MinLevel} 的玩家");
			Host.BlockLowLevels.MinLevel = (uint)GUILayout.HorizontalSlider(Host.BlockLowLevels.MinLevel, 0, 100);
			GUILayout.EndHorizontal();

			if(GUILayout.Button("强制开始游戏"))
			{
				AmongUsClient.Instance.StartGame();
			}

			if(GUILayout.Button("杀死所有人"))
			{
				foreach(PlayerControl player in PlayerControl.AllPlayerControls)
				{
					PlayerControl.LocalPlayer.RpcMurderPlayer(player, true);
				}
			}

			GUILayout.BeginHorizontal();
			if(GUILayout.Button("强制船员胜利"))
			{
				Host.DisableGameEnd.Enabled = false;
				GameManager.Instance.RpcEndGame(GameOverReason.CrewmatesByTask, false);
				Hydra.notifications.Send("游戏结束", "你以船员方胜利结束了游戏。", 5);
			}

			if(GUILayout.Button("强制内鬼胜利"))
			{
				Host.DisableGameEnd.Enabled = false;
				GameManager.Instance.RpcEndGame(GameOverReason.ImpostorsByKill, false);
				Hydra.notifications.Send("游戏结束", "你以内鬼方胜利结束了游戏。", 5);
			}
			GUILayout.EndHorizontal();

			GUILayout.Space(5);
			GUILayout.Label("地图生成器:");

			GUILayout.Label($"选中地图: {(MapNames)selectedMap}");
			selectedMap = (byte)GUILayout.HorizontalSlider(selectedMap, 0, 5);

			GUILayout.BeginHorizontal();
			if(GUILayout.Button("移除地图"))
			{
				if(ShipStatus.Instance != null)
				{
					ShipStatus.Instance.Despawn();
					Hydra.notifications.Send("地图", "当前地图已被移除。", 5);
				}
				else
				{
					Hydra.notifications.Send("地图", "地图已经被移除了。", 5);
				}
			}

			if(GUILayout.Button("生成地图"))
			{
				AmongUsClient.Instance.StartCoroutine(SpawnMap(selectedMap).WrapToIl2Cpp());
			}
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			if(GUILayout.Button("移除大厅"))
			{
				if(LobbyBehaviour.Instance != null)
				{
					LobbyBehaviour.Instance.Despawn();
					Hydra.notifications.Send("大厅", "大厅地图已被移除。", 5);
				}
				else
				{
					Hydra.notifications.Send("大厅", "大厅地图已经被移除了。", 5);
				}
			}

			if(GUILayout.Button("生成大厅"))
			{
				LobbyBehaviour.Instance = UnityEngine.Object.Instantiate<LobbyBehaviour>(GameStartManager.Instance.LobbyPrefab);
				AmongUsClient.Instance.Spawn(LobbyBehaviour.Instance, -2, SpawnFlags.None);
				Hydra.notifications.Send("大厅", "已生成新的大厅地图实例。", 5);
			}
			GUILayout.EndHorizontal();

			GUILayout.Space(5);
			GUILayout.Label("下一轮角色分配:");
			Host.AlwaysImposter.Enabled = GUILayout.Toggle(Host.AlwaysImposter.Enabled, "启用");
			GUILayout.Label($"要分配的角色: {Host.AlwaysImposter.assignedRole}");
			Host.AlwaysImposter.assignedRole = Controls.HorizontalRoleSlider(Host.AlwaysImposter.assignedRole);

			GUILayout.Space(5);
			GUILayout.Label("会议控制:");
			Host.DisableMeetings.Enabled = GUILayout.Toggle(Host.DisableMeetings.Enabled, "禁用会议");
			Hydra.routines.reportBodySpam.Enabled = GUILayout.Toggle(Hydra.routines.reportBodySpam.Enabled, "刷屏报告尸体");

			if(GUILayout.Button("跳过投票"))
			{
				if(MeetingHud.Instance == null)
				{
					Hydra.notifications.Send("跳过会议", "此选项只能在会议中使用。");
				}
				else
				{
					MeetingHud.VoterState[] votes = Array.Empty<MeetingHud.VoterState>();
					MeetingHud.Instance.RpcVotingComplete(votes, null, false);
					MeetingHud.Instance.RpcClose();
				}
			}

			GUILayout.Space(5);
			GUILayout.Label("变形控制:");
			if(GUILayout.Button("让所有人变成我"))
			{
				AmongUsClient.Instance.StartCoroutine(ShapeshiftAll(PlayerControl.LocalPlayer).WrapToIl2Cpp());
			}

			if(GUILayout.Button("让所有人随机变形"))
			{
				PlayerControl target = Utilities.GetRandomPlayer(false, false, false, false);
				AmongUsClient.Instance.StartCoroutine(ShapeshiftAll(target).WrapToIl2Cpp());
			}

			if(GUILayout.Button("还原所有人变形"))
			{
				AmongUsClient.Instance.StartCoroutine(RevertAllShapeshift().WrapToIl2Cpp());
			}

			GUILayout.Space(5);
			GUILayout.Label("迪斯科派对:");
			Hydra.routines.discoHost.Enabled = GUILayout.Toggle(Hydra.routines.discoHost.Enabled, "启用");

			GUILayout.Label($"颜色随机化延迟: {Hydra.routines.discoHost.randomizationDelay:F2}秒");
			Hydra.routines.discoHost.randomizationDelay = GUILayout.HorizontalSlider(Hydra.routines.discoHost.randomizationDelay, 0.1f, 2.0f);
		}

		private static IEnumerator SpawnMap(byte mapId)
		{
			Hydra.Log.LogInfo($"尝试生成地图 ID: {mapId}");

			AsyncOperationHandle<GameObject> asyncHandle = AmongUsClient.Instance.ShipPrefabs[mapId].InstantiateAsync(null, false);
			yield return asyncHandle;

			ShipStatus ship = asyncHandle.Result.GetComponent<ShipStatus>();
			AmongUsClient.Instance.Spawn(ship, -2, SpawnFlags.None);

			Hydra.notifications.Send("地图生成器", $"{(MapNames)mapId} 已生成。", 5);
		}

		private static IEnumerator ShapeshiftAll(PlayerControl target)
		{
			if(Utilities.IsAnticheatPresent() && !AmongUsClient.Instance.AmHost)
			{
				Hydra.notifications.Send("变形控制", "你需要是房主才能使用此功能。");
				yield break;
			}

			foreach(PlayerControl player in PlayerControl.AllPlayerControls)
			{
				if(player == target || player.shapeshiftTargetPlayerId == target.PlayerId) continue;

				Utilities.ShapeshiftPlayer(player, target);

				// 此函数可以一次发送15条可靠消息，所以需要延迟以避免被踢
				yield return Effects.Wait(0.05f);
			}
		}

		private static IEnumerator RevertAllShapeshift()
		{
			if(Utilities.IsAnticheatPresent() && !AmongUsClient.Instance.AmHost)
			{
				Hydra.notifications.Send("变形控制", "你需要是房主才能使用此功能。");
				yield break;
			}

			foreach(PlayerControl player in PlayerControl.AllPlayerControls)
			{
				if(player.shapeshiftTargetPlayerId == -1) continue;

				Utilities.ShapeshiftPlayer(player, player);

				yield return Effects.Wait(0.05f);
			}
		}
	}
}
