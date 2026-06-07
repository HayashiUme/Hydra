using HydraMenu.anticheat;
using UnityEngine;

namespace HydraMenu.ui.sections
{
	internal class AnticheatSection : ISection
	{
		public AnticheatSection() : base("反作弊") { }

		public override void Render()
		{
			Anticheat.Enabled = GUILayout.Toggle(Anticheat.Enabled, "启用 Hydra 反作弊");

			Anticheat.CheckSpoofedPlatforms = GUILayout.Toggle(Anticheat.CheckSpoofedPlatforms, "检测伪造的平台数据");

			GUILayout.Space(5);
			GUILayout.Label("需要检查的 RPC 类型:");
			foreach(var (rpcCall, handler) in Anticheat.RpcHandlers)
			{
				handler.Enabled = GUILayout.Toggle(handler.Enabled, $"{rpcCall}");
			}

			GUILayout.Space(5);
			GUILayout.Label("检测到作弊者时的处理:");
			Anticheat.sendNotification = GUILayout.Toggle(Anticheat.sendNotification, "发送通知");
			Anticheat.discardRpc = GUILayout.Toggle(Anticheat.discardRpc, "丢弃 RPC");

			GUILayout.BeginHorizontal();
			GUILayout.Label($"处罚方式: {Anticheat.punishment}");
			Anticheat.punishment = (Anticheat.Punishments)GUILayout.HorizontalSlider((float)Anticheat.punishment, 0, 3);
			GUILayout.EndHorizontal();
		}
	}
}
