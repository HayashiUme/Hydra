using UnityEngine;

namespace HydraMenu.routines
{
	public class AutoTriggerSporesRoutine : IRoutine
	{
		public AutoTriggerSporesRoutine() : base("AutoTriggerSpores") { }

		public readonly float SPORE_TRIGGER_LENGTH = 5.0f;
		private float timeElapsed = 0f;

		public override void Run()
		{
			if(ShipStatus.Instance == null) return;

			timeElapsed += Time.deltaTime;
			if(timeElapsed < SPORE_TRIGGER_LENGTH) return;
			timeElapsed = 0f;

			FungleShipStatus shipStatus = ShipStatus.Instance.Cast<FungleShipStatus>();
			foreach(Mushroom mushroom in shipStatus.sporeMushrooms.Values)
			{
				PlayerControl.LocalPlayer.RpcTriggerSpores(mushroom);
			}
		}

		public override void OnEnable()
		{
			if(ShipStatus.Instance == null)
			{
				Hydra.notifications.Send("触发孢子", "自动触发孢子需要游戏已开始。", 10);
				Enabled = false;
				return;
			}

			if(Utilities.GetCurrentMap() != MapNames.Fungle)
			{
				Hydra.notifications.Send("触发孢子", "自动触发孢子只能在 Fungle 地图使用。", 10);
				Enabled = false;
				return;
			}
		}

		public override void OnDisconnect()
		{
			Hydra.notifications.Send("触发孢子", "离开游戏，自动触发孢子已禁用。", 10);
			Enabled = false;
		}
	}
}