using Exiled.API.Features;

using Mirror;

namespace NightVisionGoggles
{
    internal static class Extensions
    {
        internal static void ShowHidedNetworkIdentity(this Player player, NetworkIdentity identity)
        {
            NetworkServer.SendSpawnMessage(identity, player.Connection);
        }

        internal static void HideNetworkIdentity(this Player player, NetworkIdentity identity)
        {
            player.Connection.Send(new ObjectHideMessage() { netId = identity.netId });
        }
    }
}
