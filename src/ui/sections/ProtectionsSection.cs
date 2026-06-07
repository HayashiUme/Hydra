using HydraMenu.features;
using UnityEngine;

namespace HydraMenu.ui.sections
{
	internal class ProtectionsSection : ISection
	{
		public ProtectionsSection() : base("防护") { }
		public override void Render()
		{
			Protections.ForceDTLS.Enabled = GUILayout.Toggle(Protections.ForceDTLS.Enabled, "强制启用 DTLS 加密网络数据");

			Protections.BlockServerTeleports.Enabled = GUILayout.Toggle(Protections.BlockServerTeleports.Enabled, "阻止服务器位置更新");

			Protections.HardenedReadPackedUInt.Enabled = GUILayout.Toggle(Protections.HardenedReadPackedUInt.Enabled, "使用强化版 packed int 反序列化器");

			Protections.BypassShapeshiftRatelimits.Enabled = GUILayout.Toggle(Protections.BypassShapeshiftRatelimits.Enabled, "绕过变形 RPC 频率限制");
			Protections.Votekicks.Enabled = GUILayout.Toggle(Protections.Votekicks.Enabled, "防止作为主机被投票踢出");
		}
	}
}
