using Hazel;
using System;

namespace HydraMenu.anticheat.rpc
{
    /// <summary>
    /// Validates CloseMeeting RPC (RPC 22)
    /// Should only be sent by the host during an active meeting
    /// </summary>
    internal class CloseMeeting : RpcCheck
    {
        public override RpcCalls GetRpcCall()
        {
            return RpcCalls.CloseMeeting;
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
            // CloseMeeting should only be sent by the host
            if(MeetingHud.Instance == null)
            {
                Anticheat.Flag(player, $"{player.Data.PlayerName} attempted to close meeting when no meeting is active.");
                blockRpc = true;
                return;
            }
        }
    }
}
