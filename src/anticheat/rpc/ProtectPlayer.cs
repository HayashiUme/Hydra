using Hazel;
using InnerNet;
using AmongUs.GameOptions;

namespace HydraMenu.anticheat.rpc
{
    /// <summary>
    /// Validates ProtectPlayer RPC (RPC 45)
    /// Only GuardianAngel role should be able to protect players
    /// </summary>
    internal class ProtectPlayer : RpcCheck
    {
        public override RpcCalls GetRpcCall()
        {
            return RpcCalls.ProtectPlayer;
        }

        public override void Validate(PlayerControl player, MessageReader reader, ref bool blockRpc)
        {
            PlayerControl target = reader.ReadNetObject<PlayerControl>();
            int colorId = (int)reader.ReadByte();

            // Only Guardian Angels can protect
            if(player.Data.RoleType != RoleTypes.GuardianAngel)
            {
                Anticheat.Flag(player, $"{player.Data.PlayerName} attempted to protect {target.Data.PlayerName} without being GuardianAngel (actual role: {player.Data.RoleType}).");
                blockRpc = true;
                return;
            }

            if(player.Data.IsDead)
            {
                Anticheat.Flag(player, $"{player.Data.PlayerName} attempted to protect while dead.");
                blockRpc = true;
                return;
            }
        }
    }
}
