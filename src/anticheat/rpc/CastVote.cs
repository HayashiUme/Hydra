using Hazel;
using System;

namespace HydraMenu.anticheat.rpc
{
    /// <summary>
    /// Validates CastVote RPC (RPC 24)
    /// Ensures vote casting happens during an active meeting
    /// and the voting player exists and is alive
    /// </summary>
    internal class CastVote : RpcCheck
    {
        public override RpcCalls GetRpcCall()
        {
            return RpcCalls.CastVote;
        }

        public override Type GetExpectedNetObject()
        {
            return typeof(MeetingHud);
        }

        public override void Validate(PlayerControl player, MessageReader reader, ref bool blockRpc)
        {
            byte srcPlayerId = reader.ReadByte();
            byte suspectPlayerId = reader.ReadByte();

            // Check that a meeting is actually active
            if(MeetingHud.Instance == null)
            {
                Anticheat.Flag(player, $"{player.Data.PlayerName} attempted to cast vote without an active meeting.");
                blockRpc = true;
                return;
            }

            // The voting player should not be dead
            NetworkedPlayerInfo voterInfo = GameData.Instance.GetPlayerById(srcPlayerId);
            if(voterInfo != null && voterInfo.IsDead)
            {
                Anticheat.Flag(player, $"{player.Data.PlayerName} attempted to cast vote for dead player {voterInfo.PlayerName}.");
                blockRpc = true;
                return;
            }

            // Validate suspect index is in valid range
            if(suspectPlayerId != byte.MaxValue && suspectPlayerId >= PlayerControl.AllPlayerControls.Count)
            {
                Anticheat.Flag(player, $"{player.Data.PlayerName} attempted to vote for invalid suspect index {suspectPlayerId}.");
                blockRpc = true;
                return;
            }
        }
    }
}
