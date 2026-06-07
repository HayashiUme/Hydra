using BepInEx.Unity.IL2CPP.Utils.Collections;
using HydraMenu.features;
using System.Collections;
using UnityEngine;

namespace HydraMenu.ui.sections
{
	internal class SelfSection : ISection
	{
		public SelfSection() : base("自身") { }

		private uint level = 199;

		public override void Render()
		{
			if(PlayerControl.LocalPlayer == null || PlayerControl.LocalPlayer.Data == null)
			{
				GUILayout.Label("你当前不在游戏中，这些选项将无法使用。");
			}
			else
			{
				GUILayout.Label($"角色: {PlayerControl.LocalPlayer.Data.RoleType}");
			}

			Self.UpdateStatsFreeplay.Enabled = GUILayout.Toggle(Self.UpdateStatsFreeplay.Enabled, "自由模式下更新统计数据");
			Self.BypassIntentionalDisconnectionBlocks.Enabled = GUILayout.Toggle(Self.BypassIntentionalDisconnectionBlocks.Enabled, "取消故意断线等待惩罚");
			Immortality.Enabled = GUILayout.Toggle(Immortality.Enabled, "不死之身");
			Self.AlwaysShowTaskAnimations = GUILayout.Toggle(Self.AlwaysShowTaskAnimations, "始终显示任务动画");
			Self.NoLadderCooldown.Enabled = GUILayout.Toggle(Self.NoLadderCooldown.Enabled, "无爬梯冷却");
			Self.UnlimitedMeetings.enabled = GUILayout.Toggle(Self.UnlimitedMeetings.enabled, "无限紧急会议");

			if(GUILayout.Button("召开紧急会议"))
			{
				if(AmongUsClient.Instance.AmHost)
				{
					Hydra.Log.LogInfo("我们是主机，可以强制开会");
					Utilities.OpenMeeting(PlayerControl.LocalPlayer, null);
				}
				else
				{
					PlayerControl.LocalPlayer.CmdReportDeadBody(null);
				}
			}

			if(GUILayout.Button("完成所有任务"))
			{
				PlayerControl.LocalPlayer.StartCoroutine(CompleteAllTasks().WrapToIl2Cpp());
			}

			if(GUILayout.Button("随机化外观"))
			{
				if(AmongUsClient.Instance.AmConnected)
				{
					Utilities.RandomizePlayer(true);
					Hydra.notifications.Send("外观随机化", "你本局的外观已随机化。", 5);
				} else
				{
					AccountManager.Instance.RandomizeName();
					Utilities.RandomizePlayer();
					Hydra.notifications.Send("外观随机化", "你的名字和外观已随机化。", 5);
				}
			}

			GUILayout.Label("任务动画:");
			GUILayout.BeginHorizontal();
			if(GUILayout.Button("开始医疗扫描"))
			{
				Network.SendSetScanner(true);
			}

			if(GUILayout.Button("结束医疗扫描"))
			{
				Network.SendSetScanner(false);
			}
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			if(GUILayout.Button("清除陨石"))
			{
				Network.SendPlayAnimation((byte)TaskTypes.ClearAsteroids);
			}

			if(GUILayout.Button("清空垃圾"))
			{
				Network.SendPlayAnimation((byte)TaskTypes.EmptyGarbage);
			}
			GUILayout.EndHorizontal();

			if(GUILayout.Button("激活护盾"))
			{
				Network.SendPlayAnimation((byte)TaskTypes.PrimeShields);
			}

			GUILayout.Space(5);
			GUILayout.Label($"修改等级为: {level + 1}");
			level = (uint)GUILayout.HorizontalSlider(level, 0, 199);

			if(GUILayout.Button("发送等级更新"))
			{
				PlayerControl.LocalPlayer.RpcSetLevel(level);
				Hydra.notifications.Send("等级修改", $"你的等级已改为 {level + 1}", 5);
			}
		}

		private IEnumerator CompleteAllTasks()
		{
			Il2CppSystem.Collections.Generic.List<PlayerTask> allTasks = PlayerControl.LocalPlayer.myTasks;

			Hydra.Log.LogInfo("正在完成所有任务...");
			foreach(PlayerTask task in allTasks)
			{
				if(task.IsComplete)
				{
					Hydra.Log.LogInfo($"任务 {task.Id} 已完成，跳过");
					continue;
				}

				Hydra.Log.LogInfo($"已发送任务 {task.Id} 的完成RPC");
				PlayerControl.LocalPlayer.RpcCompleteTask(task.Id);

				// 如果要完成超过6个任务，需要加延迟，否则原版反作弊会以频率限制为由踢出
				yield return Effects.Wait(0.05f);
			}

			Hydra.notifications.Send("任务完成", "你的所有任务已完成。", 5);
		}
	}
}
