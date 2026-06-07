using Hazel;
using InnerNet;
using AmongUs.GameOptions;

namespace HydraMenu.anticheat.rpc
{
    /// <summary>
    /// Validates Shapeshift RPC (RPC 46)
    /// Only Shapeshifter roles should be able to shapeshift
    /// Validates that sender has the Shapeshifter role
    /// </summary>
    internal class Shapeshift : RpcCheck
    {
        public override RpcCalls GetRpcCall()
        {
            return RpcCalls.Shapeshift;
        }

        public override void Validate(PlayerControl player, MessageReader reader, ref bool blockRpc)
        {
            PlayerControl targetPlayer = reader.ReadNetObject<PlayerControl>();
            bool shouldAnimate = reader.ReadBoolean();

            // Only Shapeshifters should be able to use this RPC
            if(player.Data.RoleType != RoleTypes.Shapeshifter)
            {
                Anticheat.Flag(player, $"{player.Data.PlayerName} attempted to shapeshift into {targetPlayer.Data.PlayerName} without being a Shapeshifter (actual role: {player.Data.RoleType}).");
                blockRpc = true;
                return;
            }

            if(player.Data.IsDead)
            {
                Anticheat.Flag(player, $"{player.Data.PlayerName} attempted to shapeshift while dead.");
                blockRpc = true;
                return;
            }
        }
    }
}
