using HydraMenu.features;
using UnityEngine;

namespace HydraMenu.ui.sections
{
	internal class VisualSection : ISection
	{
		public VisualSection() : base("视觉") { }

		public override void Render()
		{
			Visuals.SkipShhhAnimation.Enabled = GUILayout.Toggle(Visuals.SkipShhhAnimation.Enabled, "跳过静音动画");
			Visuals.AccurateDisconnectReasons.Enabled = GUILayout.Toggle(Visuals.AccurateDisconnectReasons.Enabled, "显示精确断开原因");

			Visuals.Fullbright.Enabled = GUILayout.Toggle(Visuals.Fullbright.Enabled, "全局照明");
			Visuals.ShowProtections.Enabled = GUILayout.Toggle(Visuals.ShowProtections.Enabled, "显示守护天使护盾");

			Chat.AlwaysVisibleChat.Enabled = GUILayout.Toggle(Chat.AlwaysVisibleChat.Enabled, "聊天始终可见");

			Visuals.ShowGhosts.Enabled = GUILayout.Toggle(Visuals.ShowGhosts.Enabled, "显示幽灵");
			Chat.OnChat.ShowMessagesByGhosts = GUILayout.Toggle(Chat.OnChat.ShowMessagesByGhosts, "显示幽灵的聊天消息");
		}
	}
}
