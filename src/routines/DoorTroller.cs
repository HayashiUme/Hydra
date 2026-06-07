using UnityEngine;

namespace HydraMenu.routines
{
	public class DoorTrollerRoutine : IRoutine
	{
		public DoorTrollerRoutine() : base("Door Troller") { }

		public float lockAndUnlockDelay = 0.5f;
		private float timeElapsed = 0f;
		private bool doorsLocked = false;

		public override void Run()
		{
			if(ShipStatus.Instance == null) return;

			timeElapsed += Time.deltaTime;
			if(timeElapsed < lockAndUnlockDelay) return;

			if(doorsLocked)
			{
				Sabotage.UnlockAll();
			}
			else
			{
				Sabotage.LockAll();
			}

			doorsLocked = !doorsLocked;
			timeElapsed = 0;
		}

		public override void OnEnable()
		{
			if(PlayerControl.LocalPlayer == null || ShipStatus.Instance == null)
			{
				Hydra.notifications.Send("门板恶搞", "门板恶搞需要游戏已开始。", 10);
				Enabled = false;
				return;
			}

			if(ShipStatus.Instance.AllDoors.Count == 0)
			{
				Hydra.notifications.Send("门板恶搞", "当前地图没有门，无法使用门板恶搞。", 10);
				Enabled = false;
				return;
			}

			if(!Sabotage.CanUnlockDoors())
			{
				Hydra.notifications.Send("门板恶搞", "门板恶搞需要你是房主，或者当前地图支持解锁门。", 10);
				Enabled = false;
				return;
			}
		}

		public override void OnDisconnect()
		{
			Hydra.notifications.Send("门板恶搞", "离开游戏，门板恶搞已禁用。", 10);
			Enabled = false;
		}
	}
}