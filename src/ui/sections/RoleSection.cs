using AmongUs.GameOptions;
using HydraMenu.features;
using UnityEngine;

namespace HydraMenu.ui.sections
{
	internal class RolesSection : ISection
	{
		public RolesSection() : base("角色") { }

		private RoleTypes selectedRole = RoleTypes.Crewmate;

		public override void Render()
		{
			Roles.AllowVentingForCrewmates = GUILayout.Toggle(Roles.AllowVentingForCrewmates, "船员可以钻管道");
			Roles.MoveModifier.MoveInVents = GUILayout.Toggle(Roles.MoveModifier.MoveInVents, "在管道中可移动");

			Roles.SkipSabotageChecks.SabotageAsCrewmate = GUILayout.Toggle(Roles.SkipSabotageChecks.SabotageAsCrewmate, "船员可破坏");
			Roles.SkipSabotageChecks.SabotageInVents = GUILayout.Toggle(Roles.SkipSabotageChecks.SabotageInVents, "内鬼可在管道内破坏");

			Roles.DisableShapeshiftAnimation = GUILayout.Toggle(Roles.DisableShapeshiftAnimation, "禁用变形动画");

			GUILayout.Label($"更改角色为: {selectedRole}");
			GUILayout.BeginHorizontal();
			selectedRole = Controls.HorizontalRoleSlider(selectedRole);

			if(GUILayout.Button("应用角色" + (AmongUsClient.Instance.AmHost ? "" : " (仅本地)")) && PlayerControl.LocalPlayer)
			{
				Hydra.Log.LogInfo($"正在更新角色为 {selectedRole}");
				UpdateRole(selectedRole);

				if(AmongUsClient.Instance.AmHost)
				{
					Hydra.Log.LogInfo("因为我们是主机，可以发送 SetRole RPC 同步角色到服务器");
					PlayerControl.LocalPlayer.RpcSetRole(selectedRole, true);
				}

				Hydra.notifications.Send("角色更新", $"你的角色已更新为 {selectedRole}。");
			}
			GUILayout.EndHorizontal();
		}

		public static void UpdateRole(RoleTypes role)
		{
			bool isGhost = RoleManager.IsGhostRole(role);

			HudManager.Instance.ReportButton.gameObject.SetActive(!isGhost);

			RoleManager.Instance.SetRole(PlayerControl.LocalPlayer, role);
		}
	}
}
