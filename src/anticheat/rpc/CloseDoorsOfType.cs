using Hazel;
using System;

namespace HydraMenu.anticheat.rpc
{
	internal class CloseDoorsOfType : RpcCheck
	{
		// Player is null here as we cannot determine who sent this RPC
		public override void Validate(PlayerControl player, MessageReader reader, ref bool blockRpc)
		{
			// It would be nice if we could also add additional checks such as someone closing doors without being imposter
			// however we are not able to determine who sent the CloseDoorsOfType RPC
			if(GameManager.Instance.IsHideAndSeek())
			{
				Hydra.notifications.Send("反作弊", "有人在躲猫猫模式中试图关门。", Anticheat.NotificationDuration);
				blockRpc = true;
			}
		}

		public override RpcCalls GetRpcCall()
		{
			return RpcCalls.CloseDoorsOfType;
		}

		public override Type GetExpectedNetObject()
		{
			return typeof(ShipStatus);
		}
	}
}