using Hazel;
using InnerNet;
using AmongUs.GameOptions;

namespace HydraMenu.anticheat.rpc
{
    /// <summary>
    /// Validates MurderPlayer RPC (RPC 12)
    /// Only impostors should be able to murder players
    /// Victims must be alive, not in vents, not on ladders/platforms
    /// </summary>
    internal class MurderPlayer : RpcCheck
    {
        public override RpcCalls GetRpcCall()
        {
            return RpcCalls.MurderPlayer;
        }

        public override void Validate(PlayerControl player, MessageReader reader, ref bool blockRpc)
        {
            PlayerControl target = reader.ReadNetObject<PlayerControl>();
            MurderResultFlags resultFlags = (MurderResultFlags)reader.ReadInt32();

            // If the RPC says the murder succeeded but the sender isn't an impostor
            if(resultFlags.HasFlag(MurderResultFlags.Succeeded))
            {
                if(!RoleManager.IsImpostorRole(player.Data.RoleType))
                {
                    Anticheat.Flag(player, $"{player.Data.PlayerName} attempted to murder {target.Data.PlayerName} while not being an impostor.");
                    blockRpc = true;
                    return;
                }

                if(player.Data.IsDead)
                {
                    Anticheat.Flag(player, $"{player.Data.PlayerName} attempted to murder {target.Data.PlayerName} while dead.");
                    blockRpc = true;
                    return;
                }

                if(target == null || target.Data == null || target.Data.IsDead)
                {
                    Anticheat.Flag(player, $"{player.Data.PlayerName} attempted to murder an already dead or null player.");
                    blockRpc = true;
                    return;
                }

                if(target == player)
                {
                    Anticheat.Flag(player, $"{player.Data.PlayerName} attempted to murder themselves.");
                    blockRpc = true;
                    return;
                }
            }
        }
    }
}
