using HarmonyLib;

namespace HydraMenu.features
{
	internal class Troll
	{
		[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.MurderPlayer))]
		public static class AutoReportBodies
		{
			public static PlayerControl source;

			public static bool Enabled
			{
				get { return source != null; }
				set
				{
					if(!value) source = null;
				}
			}

			static void Postfix(PlayerControl __instance, PlayerControl target, MurderResultFlags resultFlags)
			{
				if(!Enabled || !resultFlags.HasFlag(MurderResultFlags.Succeeded)) return;

				if(AmongUsClient.Instance.AmHost)
				{
					Utilities.OpenMeeting(source, target.Data);
					return;
				}

				if(PlayerControl.LocalPlayer.Data.IsDead) return;

				Hydra.notifications.Send("自动报告尸体", $"{target.Data.PlayerName} 被 by {__instance.Data.PlayerName} {Utilities.GetPlayerColor(__instance.Data)}, 的尸体已被自动报告。");
				PlayerControl.LocalPlayer.CmdReportDeadBody(target.Data);
			}
		}

		[HarmonyPatch(typeof(VentilationSystem), nameof(VentilationSystem.Deserialize))]
		public static class BlockVenting
		{
			public static bool Enabled { get; set; } = false;

			static void Postfix(VentilationSystem __instance)
			{
				if(!Enabled) return;

				Hydra.Log.LogInfo($"Received update for VentilationSystem, going to kick out all players who are inside a vent");

				if(__instance.PlayersInsideVents.Count >= PlayerControl.AllPlayerControls.Count)
				{
					Hydra.Log.LogInfo($"Apparently there are more people inside of vents than people inside the game, the host may be trying to overload our game! Players in vents: {__instance.PlayersInsideVents.Count}, total players: {PlayerControl.AllPlayerControls.Count}");
					return;
				}

				foreach(byte ventId in __instance.PlayersInsideVents.Values)
				{
					if(ventId >= ShipStatus.Instance.AllVents.Count) continue;

					Hydra.Log.LogInfo($"Kicked someone out of vent {ventId}");
					VentilationSystem.Update(VentilationSystem.Operation.StartCleaning, ventId);
				}
			}
		}

		// When the host recieves a Sabotage system update, it first ensures that there is no active meeting, and that the sabotage cooldown has ended
		// If all checks pass, the host sets the sabotage cooldown to 30.0s and then handles which system to update based off of the sabotage type
		// The only problem is that the host updates the sabotage cooldown without first confirming that the attempted sabotage actually succeeded
		// Meaning that if we were to sabotage a system that does not have an associated sabotage, the host would just reset the sabotage cooldown
		// We can use flaw to create an anti-sabotage by sabotaging an invalid system every time the sabotage cooldown ends
		// which gives the impostors pratically no time to be able to do any sabotages themselves
		[HarmonyPatch(typeof(SabotageSystemType), nameof(SabotageSystemType.Deserialize))]
		public static class BlockSabotages
		{
			private static bool enabled = false;
			public static bool Enabled
			{
				get { return enabled; }
				set
				{
					if(enabled == value) return;

					if(value && AmongUsClient.Instance.AmHost)
					{
						Hydra.notifications.Send("阻止破坏", "此选项应在你不是房主时使用。请改用主机页面的禁用破坏。");
						Host.DisableSabotages.Enabled = true;
						return;
					}

					enabled = value;
				}
			}

			static void Postfix(SabotageSystemType __instance)
			{
				if(!Enabled || __instance.Timer > 0.1f) return;

				Hydra.Log.LogMessage($"Sabotage cooldown has depleted to {__instance.Timer}, sending Sabotage system update");
				ShipStatus.Instance.RpcUpdateSystem(SystemTypes.Sabotage, 255);
			}
		}
	}
}