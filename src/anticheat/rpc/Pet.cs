using Hazel;

namespace HydraMenu.anticheat.rpc
{
    /// <summary>
    /// Validates Pet RPC (RPC 50) and CancelPet RPC (RPC 51)
    /// Ensures pet interactions are valid
    /// </summary>
    internal class Pet : RpcCheck
    {
        public override RpcCalls GetRpcCall()
        {
            return RpcCalls.Pet;
        }

        public override void Validate(PlayerControl player, MessageReader reader, ref bool blockRpc)
        {
            // Pet RPC just plays a pet animation - no major exploit risk
            // But we can still check basic validity
            if(player.Data == null || player.Data.IsDead)
            {
                Anticheat.Flag(player, $"{player.Data.PlayerName} attempted to use pet while dead.");
                blockRpc = true;
                return;
            }
        }
    }
}
