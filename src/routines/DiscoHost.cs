using UnityEngine;

namespace HydraMenu.routines
{
	public class DiscoHostRoutine : IRoutine
	{
		public DiscoHostRoutine() : base("DiscoHost") { }

		public float randomizationDelay = 0.5f;
		private float timeElapsed = 0f;

		public override void Run()
		{
			timeElapsed += Time.deltaTime;
			if(timeElapsed < randomizationDelay) return;

			System.Random rnd = new System.Random();
			foreach(PlayerControl player in PlayerControl.AllPlayerControls)
			{
				player.RpcSetColor((byte)rnd.Next(0, 17));
			}

			timeElapsed = 0f;
		}

		public override void OnEnable()
		{
			if(PlayerControl.LocalPlayer == null)
			{
				Hydra.notifications.Send("迪斯科派对", "迪斯科派对只能在游戏内使用。", 10);
				Enabled = false;
				return;
			}

			if(Utilities.IsAnticheatPresent() && !AmongUsClient.Instance.AmHost)
			{
				Hydra.notifications.Send("迪斯科派对", "迪斯科派对需要你是房主。", 10);
				Enabled = false;
				return;
			}
		}

		public override void OnDisconnect()
		{
			Hydra.notifications.Send("迪斯科派对", "离开游戏，迪斯科派对已禁用。", 10);
			Enabled = false;
		}
	}
}