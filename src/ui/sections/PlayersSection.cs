using AmongUs.Data;
using AmongUs.GameOptions;
using HydraMenu.features;
using InnerNet;
using System;
using UnityEngine;

namespace HydraMenu.ui.sections
{
	internal class PlayersSection : ISection
	{
		public PlayersSection() : base("玩家") { }

		public static Vector2 PlayerPaneSize
		{
			get { return new Vector2(100 * MainUI.scale, MainUI.WindowSize.y - MainUI.HeaderSize.y); }
		}

		public static Vector2 PlayerPanePosition
		{
			get { return new Vector2(MainUI.SectionListPosition.x + MainUI.SectionListSize.x, MainUI.HeaderSize.y + MainUI.HeaderPosition.y); }
		}

		public static Vector2 PlayerButtonSize
		{
			get { return new Vector2(PlayerPaneSize.x, 30 * MainUI.scale); }
		}

		public static Vector2 PlayerOptionsSize
		{
			get { return new Vector2(MainUI.WindowSize.x - MainUI.SectionListSize.x - PlayerPaneSize.x, MainUI.WindowSize.y - MainUI.HeaderSize.y); }
		}

		public static Vector2 PlayerOptionsPosition
		{
			get { return new Vector2(PlayerPanePosition.x + PlayerPaneSize.x, MainUI.HeaderPosition.y + MainUI.HeaderSize.y); }
		}

		public static Vector2 PlayerColorBoxSize
		{
			get { return new Vector2(5 * MainUI.scale, PlayerButtonSize.y); }
		}

		public static PlayerControl selectedPlayer;
		private Vector2 subsectionScrollVector;

		private static Controls.PlayerColors selectedColor = Controls.PlayerColors.Red;

		public override void Render()
		{
			if(PlayerControl.AllPlayerControls.Count == 0)
			{
				GUILayout.Label("当前没有在线玩家。");
				return;
			}

			GUI.Box(new Rect(0, 0, PlayerPaneSize.x, PlayerPaneSize.y), "", Styles.MainBox);

			for(byte i = 0; i < PlayerControl.AllPlayerControls.Count; i++)
			{
				PlayerControl player = PlayerControl.AllPlayerControls[i];
				// Wait for player data to fully load
				if(player.Data == null) continue;

				RenderPlayerSelection(i, player);

				if(player == selectedPlayer)
				{
					GUILayout.BeginArea(new Rect(PlayerPaneSize.x, 0, PlayerOptionsSize.x, PlayerOptionsSize.y));
					subsectionScrollVector = GUILayout.BeginScrollView(subsectionScrollVector);

					RenderPlayerControls(player);

					GUILayout.EndScrollView();
					GUILayout.EndArea();
				}
			}
		}

		private void RenderPlayerSelection(byte position, PlayerControl player)
		{
			Rect playerInfo = new Rect(0, position * PlayerButtonSize.y, PlayerButtonSize.x, PlayerButtonSize.y);

			string playerName = player.Data.PlayerName;
			playerName += $"\n<color=\"{GetRoleColor(player.Data.RoleType)}\">{player.Data.RoleType}</color>";

			GUIStyle style = player == selectedPlayer ? Styles.PlayerBoxActive : Styles.PlayerBox;

			if(player.OwnerId == AmongUsClient.Instance.HostId)
			{
				style.normal.textColor = new Color(1.0f, 0.84f, 0.0f); // #FFD700
			}

			if(GUI.Button(playerInfo, playerName, style))
			{
				selectedPlayer = player;
			}

			Rect playerColor = new Rect(0, position * PlayerButtonSize.y, PlayerColorBoxSize.x, PlayerColorBoxSize.y);
			Controls.DrawCrewmateColorBox(playerColor, player.Data);
		}

		private string GetRoleColor(RoleTypes role)
		{
			return RoleManager.IsImpostorRole(role) ? "red" : "#8afcfc";
		}

		private static void RenderPlayerControls(PlayerControl target)
		{
			if(target == null || target.Data == null)
			{
				GUILayout.Label("指定的目标无效。");
				return;
			}

			ClientData clientData = AmongUsClient.Instance.GetClientFromCharacter(target);
			if(clientData != null)
			{
				PlatformSpecificData platform = clientData.PlatformData;

				bool streamerMode = DataManager.Settings.Gameplay.StreamerMode;

				GUILayout.Label(
					// If we want to get a player's name, we have to use NetworkedPlayerInfo::PlayerName instead of PlayerControl::name to avoid
					// getting the incorrect name if the player is shapeshifted to another player
					$"名字: {target.Data.PlayerName} ({Utilities.GetPlayerColor(target.Data)})" +
					$"\n角色: {target.Data.RoleType}" +
					$"\n状态: " + (target.Data.IsDead ? "死亡" : "存活") +
					$"\n好友码: " + (streamerMode ? "已隐藏" : target.Data.FriendCode) +
					$"\nPUID: " + (streamerMode ? "已隐藏" : target.Data.Puid) +
					$"\n等级: {target.Data.PlayerLevel + 1}" +
					$"\n设备: {platform.Platform}" +
					(target.OwnerId == AmongUsClient.Instance.HostId ? "\n主机: 是" : "")
				);
			}
			else
			{
				GUILayout.Label(
					$"名字: {target.Data.PlayerName} ({Utilities.GetPlayerColor(target.Data)})" +
					$"\n角色: {target.Data.RoleType}" +
					$"\n状态: " + (target.Data.IsDead ? "死亡" : "存活") +
					$"\n虚拟玩家: 是"
				);
			}

			Hydra.routines.playerFollower.following = Controls.PlayerSpecificToggle("跟随", target, Hydra.routines.playerFollower.following);

			if(GUILayout.Button("传送"))
			{
				// We do not want to use PlayerControl::GetTruePosition() here as it would teleport us to the player's feet
				Teleporter.TeleportTo(target.transform.position);
			}

			if(GUILayout.Button("杀死"))
			{
				AttemptMurder(target);
			}

			if(GUILayout.Button("复制外观"))
			{
				Utilities.CopyPlayer(target);
			}

			if(GUILayout.Button("报告尸体"))
			{
				AttemptReportBody(target);
			}

			GUILayout.Space(5);
			GUILayout.Label("仅主机可用:" + (AmongUsClient.Instance.AmHost ? "" : "\n(使用这些会害你被踢!)"));

			Troll.AutoReportBodies.source = Controls.PlayerSpecificToggle("自动报告尸体 身份", target, Troll.AutoReportBodies.source);

			if(GUILayout.Button("强制开会 身份"))
			{
				Utilities.OpenMeeting(target, null);
			}

			GUILayout.BeginHorizontal();
			if(GUILayout.Button("强制所有人投给"))
			{
				if(MeetingHud.Instance == null)
				{
					Hydra.notifications.Send("强制投票", "此功能仅在有活跃会议时可用。");
				}
				else
				{
					foreach(PlayerControl player in PlayerControl.AllPlayerControls)
					{
						PlayerVoteArea votingArea = MeetingHud.Instance.playerStates[player.PlayerId];

						votingArea.SetVote(target.PlayerId);
					}

					MeetingHud.Instance.SetDirtyBit(1);
					MeetingHud.Instance.CheckForEndVoting();
				}
			}

			if(GUILayout.Button("放逐"))
			{
				if(MeetingHud.Instance == null)
				{
					MeetingHud.Instance = UnityEngine.Object.Instantiate<MeetingHud>(HudManager.Instance.MeetingPrefab);
					AmongUsClient.Instance.Spawn(MeetingHud.Instance, -2, SpawnFlags.None);
				}

				// Show the Exile screen with the player being ejected
				MeetingHud.VoterState[] votes = Array.Empty<MeetingHud.VoterState>();
				MeetingHud.Instance.RpcVotingComplete(votes, target.Data, false);
				// If we created a MeetingHud object then it will be destroyed by the RpcClose function
				MeetingHud.Instance.RpcClose();
			}
			GUILayout.EndHorizontal();
			{
				PlayerControl randomPl = Utilities.GetRandomPlayer(false, false, false, false);
				Utilities.ShapeshiftPlayer(target, randomPl);
			}

			GUILayout.BeginHorizontal();
			if(GUILayout.Button("刷屏任务"))
			{
				byte[] taskIds = new byte[255];

				for(byte i = 0; i < 255; i++)
				{
					taskIds[i] = i;
				}

				target.Data.RpcSetTasks(taskIds);
			}

			if(GUILayout.Button("清空任务"))
			{
				target.Data.RpcSetTasks(Array.Empty<byte>());
			}
			GUILayout.EndHorizontal();

			GUILayout.Space(5);
			GUILayout.Label("游戏选项修改:");

			GUILayout.BeginHorizontal();
			if(GUILayout.Button("致盲"))
			{
				IGameOptions gameOptions = GameOptions.CreateCloneOptions(GameManager.Instance.LogicOptions.currentGameOptions);
				gameOptions.SetFloat(FloatOptionNames.CrewLightMod, -1.0f);
				gameOptions.SetFloat(FloatOptionNames.ImpostorLightMod, -1.0f);

				GameOptions.SendGameOptionsToClient(gameOptions, target.OwnerId);
			}

			if(GUILayout.Button("全亮"))
			{
				IGameOptions gameOptions = GameOptions.CreateCloneOptions(GameManager.Instance.LogicOptions.currentGameOptions);
				gameOptions.SetFloat(FloatOptionNames.CrewLightMod, 1000f);
				gameOptions.SetFloat(FloatOptionNames.ImpostorLightMod, 1000f);

				GameOptions.SendGameOptionsToClient(gameOptions, target.OwnerId);
			}
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			if(GUILayout.Button("减速"))
			{
				IGameOptions gameOptions = GameOptions.CreateCloneOptions(GameManager.Instance.LogicOptions.currentGameOptions);
				gameOptions.SetFloat(FloatOptionNames.PlayerSpeedMod, 0.1f);

				GameOptions.SendGameOptionsToClient(gameOptions, target.OwnerId);
			}

			if(GUILayout.Button("超速"))
			{
				// The vanilla anticheat prevents us from being able to exceed speeds greater than 3.0f
				float maxSpeed = Utilities.IsAnticheatPresent() ? 3.0f : 5.0f;

				IGameOptions gameOptions = GameOptions.CreateCloneOptions(GameManager.Instance.LogicOptions.currentGameOptions);
				gameOptions.SetFloat(FloatOptionNames.PlayerSpeedMod, maxSpeed);

				GameOptions.SendGameOptionsToClient(gameOptions, target.OwnerId);
			}
			GUILayout.EndHorizontal();

			/*
			// The problem with changing the TaskBarMode is that if we remove the task bar, we are not able to bring it back
			GUILayout.BeginHorizontal();
			if(GUILayout.Button("Hide Task Bar"))
			{
				IGameOptions gameOptions = GameOptions.CreateCloneOptions(GameManager.Instance.LogicOptions.currentGameOptions);
				gameOptions.SetInt(Int32OptionNames.TaskBarMode, (int)TaskBarMode.Invisible);

				GameOptions.SendGameOptionsToClient(gameOptions, target.OwnerId);
			}

			if(GUILayout.Button("Show Task Bar"))
			{
				IGameOptions gameOptions = GameOptions.CreateCloneOptions(GameManager.Instance.LogicOptions.currentGameOptions);
				gameOptions.SetInt(Int32OptionNames.TaskBarMode, (int)TaskBarMode.Normal);

				GameOptions.SendGameOptionsToClient(gameOptions, target.OwnerId);
			}
			GUILayout.EndHorizontal();
			*/

			if(GUILayout.Button("恢复默认"))
			{
				IGameOptions gameOptions = GameOptions.CreateCloneOptions(GameManager.Instance.LogicOptions.currentGameOptions);
				GameOptions.SendGameOptionsToClient(gameOptions, target.OwnerId);
			}

			GUILayout.Space(5);
			GUILayout.Label($"更改颜色为: {selectedColor}");
			selectedColor = Controls.HorizontalColorSlider(selectedColor);

			if(GUILayout.Button("设置颜色"))
			{
				target.RpcSetColor((byte)selectedColor);
			}

		}

		private static void AttemptMurder(PlayerControl target)
		{
			bool hasAnticheat = Utilities.IsAnticheatPresent();

			if(hasAnticheat && ShipStatus.Instance == null)
			{
				Hydra.notifications.Send("Murder Player", $"游戏开始后才能杀人。");
				return;
			}

			if(AmongUsClient.Instance.AmHost)
			{
				Hydra.Log.LogInfo($"Attempting to murder {target.Data.PlayerName}, we are the host so we can use the MurderPlayer RPC");
				PlayerControl.LocalPlayer.RpcMurderPlayer(target, true);
				Hydra.notifications.Send("Murder Player", $"已杀死 {target.Data.PlayerName}.", 5);
				return;
			}

			if(!hasAnticheat)
			{
				Hydra.Log.LogInfo($"Attempting to murder {target.Data.PlayerName}, we are are in a host-authoritative lobby so we can use the MurderPlayer RPC");
				PlayerControl.LocalPlayer.RpcMurderPlayer(target, true);
				Hydra.notifications.Send("Murder Player", $"已杀死 {target.Data.PlayerName}.", 5);
				return;
			}

			Hydra.Log.LogInfo($"Attempting to kill {target.Data.PlayerName}, we are not the host so we have to use the CheckMurder RPC");

			// The CheckMurder RPC handler will not authorize kills if you are not the imposter or you are inside of a meeting
			// There are more checks, but I do not think it is worth adding them all here
			if(!RoleManager.IsImpostorRole(PlayerControl.LocalPlayer.Data.RoleType))
			{
				Hydra.notifications.Send("Murder Player", "只有内鬼才能杀人，或者你是房主。");
				return;
			}

			if(MeetingHud.Instance != null)
			{
				Hydra.notifications.Send("Murder Player", "只能在会议之外杀人，或者你是房主。");
				return;
			}

			Hydra.notifications.Send("Murder Player", $"尝试杀死 {target.Data.PlayerName}.", 5);
			PlayerControl.LocalPlayer.CmdCheckMurder(target);
		}

		private static void AttemptReportBody(PlayerControl target)
		{
			if(AmongUsClient.Instance.AmHost)
			{
				Hydra.Log.LogInfo($"Attempting to report {target.Data.PlayerName}'s body, we are the host so we directly use the StartMeeting RPC");
				Utilities.OpenMeeting(PlayerControl.LocalPlayer, target.Data);
				return;
			}

			Hydra.Log.LogInfo($"Attempting to report {target.Data.PlayerName}'s body, we are not the host so we have to use the ReportDeadBody RPC");

			if(Utilities.IsAnticheatPresent())
			{
				// It may seem like this check is redundant as there should be no way for a player to be dead inside the lobby
				// however there are ways that players can use to mark themselves as dead in the lobby
				if(LobbyBehaviour.Instance != null)
				{
					Hydra.notifications.Send("报告尸体", "此选项需要游戏已开始。");
					return;
				}

				if(!target.Data.IsDead)
				{
					Hydra.notifications.Send("报告尸体", "只能报告本轮已死亡玩家的尸体。");
					return;
				}

				bool bodyExists = false;
				// Loop over every single dead body that exists and check if it matches our target's player id
				// From PlayerControl::ReportClosest
				foreach(Collider2D collider in Physics2D.OverlapCircleAll(new Vector2(0, 0), 99999f, Constants.PlayersOnlyMask))
				{
					if(collider.tag != "DeadBody") continue;

					DeadBody bodyComponent = collider.GetComponent<DeadBody>();
					if(bodyComponent && bodyComponent.ParentId == target.PlayerId)
					{
						bodyExists = true;
						break;
					}
				}

				if(!bodyExists)
				{
					Hydra.notifications.Send("报告尸体", "找不到该玩家的尸体, you can only report a player's body if they have died this round and their body has not dissolved.");
					return;
				}
			}

			Hydra.Log.LogInfo($"All checks passed, we are able to report {target.Data.PlayerName}'s body.");

			PlayerControl.LocalPlayer.CmdReportDeadBody(target.Data);
		}
	}
}