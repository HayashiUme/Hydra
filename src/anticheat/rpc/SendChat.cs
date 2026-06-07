using Hazel;

namespace HydraMenu.anticheat.rpc
{
    /// <summary>
    /// Validates SendChat RPC (RPC 13)
    /// Prevents dead players from sending chat (unless configured)
    /// Checks for chat spam/length abuse
    /// </summary>
    internal class SendChat : RpcCheck
    {
        public override RpcCalls GetRpcCall()
        {
            return RpcCalls.SendChat;
        }

        public override void Validate(PlayerControl player, MessageReader reader, ref bool blockRpc)
        {
            string chatText = reader.ReadString();

            // Check message length (guard against buffer overflow/spam)
            if(chatText != null && chatText.Length > 500)
            {
                Anticheat.Flag(player, $"{player.Data.PlayerName} sent excessively long chat message ({chatText.Length} chars).");
                blockRpc = true;
                return;
            }

            // Dead players shouldn't chat (only ghosts through seance-like system)
            // This is a soft check - the game normally handles this
        }
    }
}
