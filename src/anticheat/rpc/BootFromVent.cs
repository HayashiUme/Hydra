using Hazel;
using InnerNet;
using System;

namespace HydraMenu.anticheat.rpc
{
    /// <summary>
    /// Validates BootFromVent RPC (RPC 34)
    /// Only the host should be able to boot players from vents
    /// </summary>
    internal class BootFromVent : RpcCheck
    {
        public override RpcCalls GetRpcCall()
        {
            return RpcCalls.BootFromVent;
        }

        public override Type GetExpectedNetObject()
        {
            return typeof(PlayerPhysics);
        }

        public override bool IsHostOnly()
        {
            return true;
        }

        public override void Validate(PlayerControl player, MessageReader reader, ref bool blockRpc)
        {
            int ventId = reader.ReadPackedInt32();
            PlayerControl targetPlayer = reader.ReadNetObject<PlayerControl>();

            // Check vent exists
            if(ShipStatus.Instance != null && ventId >= ShipStatus.Instance.AllVents.Count)
            {
                Anticheat.Flag(player, $"{player.Data.PlayerName} attempted to boot from non-existent vent {ventId}.");
                blockRpc = true;
                return;
            }
        }
    }
}
