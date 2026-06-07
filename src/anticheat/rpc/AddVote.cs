using Hazel;
using InnerNet;
using System;

namespace HydraMenu.anticheat.rpc
{
    /// <summary>
    /// Validates AddVote RPC (RPC 26) for VoteBanSystem
    /// Checks that voting client IDs are valid and exist in the game
    /// </summary>
    internal class AddVote : RpcCheck
    {
        public override RpcCalls GetRpcCall()
        {
            return RpcCalls.AddVote;
        }

        public override Type GetExpectedNetObject()
        {
            return typeof(VoteBanSystem);
        }

        public override void Validate(PlayerControl player, MessageReader reader, ref bool blockRpc)
        {
            int srcClient = reader.ReadInt32();
            int targetClient = reader.ReadInt32();

            // Verify the source client actually exists (not a fabricated ID)
            ClientData srcClientData = AmongUsClient.Instance.GetClient(srcClient);
            if(srcClientData == null)
            {
                Anticheat.Flag(player, $"{player.Data.PlayerName} sent AddVote from non-existent client ID {srcClient} against client {targetClient}.");
                blockRpc = true;
                return;
            }

            // Verify the target client exists
            ClientData targetClientData = AmongUsClient.Instance.GetClient(targetClient);
            if(targetClientData == null)
            {
                Anticheat.Flag(player, $"{player.Data.PlayerName} sent AddVote against non-existent client ID {targetClient}.");
                blockRpc = true;
                return;
            }

            // Can't vote-kick yourself
            if(srcClient == targetClient)
            {
                Anticheat.Flag(player, $"{player.Data.PlayerName} attempted to vote-kick themselves.");
                blockRpc = true;
                return;
            }
        }
    }
}
