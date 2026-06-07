using Hazel;
using AmongUs.GameOptions;

namespace HydraMenu.anticheat.rpc
{
    /// <summary>
    /// Validates SetRole RPC (RPC 44)
    /// This RPC should only be sent by the host during role assignment
    /// Any non-host sending this is an exploit attempt
    /// </summary>
    internal class SetRole : RpcCheck
    {
        public override RpcCalls GetRpcCall()
        {
            return RpcCalls.SetRole;
        }

        public override bool IsHostOnly()
        {
            return true;
        }

        public override void Validate(PlayerControl player, MessageReader reader, ref bool blockRpc)
        {
            RoleTypes role = (RoleTypes)reader.ReadUInt16();
            bool canOverride = reader.ReadBoolean();

            // Only valid role types should be settable
            if(!System.Enum.IsDefined(typeof(RoleTypes), role))
            {
                Anticheat.Flag(player, $"{player.Data.PlayerName} attempted to set invalid role: {role}.");
                blockRpc = true;
                return;
            }

            // Dead players should only become ghost roles
            if(player.Data.IsDead)
            {
                bool isValidGhostRole = role == RoleTypes.CrewmateGhost || role == RoleTypes.ImpostorGhost;
                if(!isValidGhostRole)
                {
                    Anticheat.Flag(player, $"{player.Data.PlayerName} attempted to set living role {role} while dead.");
                    blockRpc = true;
                    return;
                }
            }
        }
    }
}
