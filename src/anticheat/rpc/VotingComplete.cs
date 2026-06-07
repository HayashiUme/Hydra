using Hazel;
using System;

namespace HydraMenu.anticheat.rpc
{
    /// <summary>
    /// Validates VotingComplete RPC (RPC 23)
    /// This should only be sent by the host to complete voting
    /// </summary>
    internal class VotingComplete : RpcCheck
    {
        public override RpcCalls GetRpcCall()
        {
            return RpcCalls.VotingComplete;
        }

        public override Type GetExpectedNetObject()
        {
            return typeof(MeetingHud);
        }

        public override bool IsHostOnly()
        {
            return true;
        }

        public override void Validate(PlayerControl player, MessageReader reader, ref bool blockRpc)
        {
            // This RPC should ONLY come from the host
            // If we're the host and someone else sends it, flag them
            // The host-only check in Anticheat.cs already handles this case
        }
    }
}
