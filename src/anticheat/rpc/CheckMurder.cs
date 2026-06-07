using Hazel;
using InnerNet;
using AmongUs.GameOptions;

namespace HydraMenu.anticheat.rpc
{
    /// <summary>
    /// Validates CheckMurder RPC (RPC 47)
    /// This RPC is sent to the server to request authorization for a kill
    /// Only impostors should send this, and targets must be valid
    /// </summary>
    internal class CheckMurder : RpcCheck
    {
        public override RpcCalls GetRpcCall()
        {
            return RpcCalls.CheckMurder;
        }

        public override void Validate(PlayerControl player, MessageReader reader, ref bool blockRpc)
        {
            PlayerControl target = reader.ReadNetObject<PlayerControl>();

            // Only impostors can request kills
            if(!RoleManager.IsImpostorRole(player.Data.RoleType))
            {
                Anticheat.Flag(player, $"{player.Data.PlayerName} attempted to check murder {target.Data.PlayerName} without being an impostor.");
                blockRpc = true;
                return;
            }

            if(player.Data.IsDead)
            {
                Anticheat.Flag(player, $"{player.Data.PlayerName} attempted to check murder while dead.");
                blockRpc = true;
                return;
            }

            if(target == player)
            {
                Anticheat.Flag(player, $"{player.Data.PlayerName} attempted to check murder on themselves.");
                blockRpc = true;
                return;
            }

            // Can't kill during meetings
            if(MeetingHud.Instance != null)
            {
                Anticheat.Flag(player, $"{player.Data.PlayerName} attempted to check murder during a meeting.");
                blockRpc = true;
                return;
            }
        }
    }
}
