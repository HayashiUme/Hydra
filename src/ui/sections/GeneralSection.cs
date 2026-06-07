using HydraMenu.features;
using UnityEngine;

namespace HydraMenu.ui.sections
{
	internal class GeneralSection : ISection
	{
		public GeneralSection() : base("欢迎") { }

		public override void Render() {
			GUILayout.Label("欢迎使用 Hydra！Hydra 是一个集实用工具、管理功能和恶搞功能于一体的菜单，\n旨在提升 Among Us 的游戏体验。我们提供便捷功能、朋友间整活娱乐功能，\n以及保护你的房间免受恶意玩家侵扰的功能。\n\n由于 Hydra 的部分功能可能被用于作弊，我必须明确声明：\nHydra 不应用于破坏其他玩家的游戏体验。\n滥用本模组进行恶意行为可能导致你的账号被封禁。\n\n汉化者：HayashiUme");

			Chat.OnChat.LogChatMessages = GUILayout.Toggle(Chat.OnChat.LogChatMessages, "记录聊天消息到控制台");

			if(GUILayout.Button("清除通知"))
			{
				Hydra.notifications.ClearNotifications();
				Hydra.notifications.Send("通知", "所有通知已清除。", 5);
			}
		}
	}
}
