using System;

using Exiled.API.Features;

using HarmonyLib;

using Mirror;

namespace NightVisionGoggles
{
    internal static class Extensions
    {
        internal static void PatchSingleType(this Harmony harmony, Type patchClass)
        {
            PatchClassProcessor processor = new(harmony, patchClass);
            processor.Patch();
        }

        internal static void ShowHidedNetworkIdentity(this Player player, NetworkIdentity identity)
        {
            Server.SendSpawnMessage.Invoke(null, [identity, player.Connection]);
        }

        internal static void HideNetworkIdentity(this Player player, NetworkIdentity identity)
        {
            player.Connection.Send(new ObjectHideMessage() { netId = identity.netId });
        }
    }
}
