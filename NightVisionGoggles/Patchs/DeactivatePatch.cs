using HarmonyLib;

using InventorySystem.Items.Pickups;
using InventorySystem.Items.Usables.Scp1344;

using MEC;

using static NightVisionGoggles.NightVisionGoggles;

namespace NightVisionGoggles.Patchs
{
    [HarmonyPatch(typeof(Scp1344Item), nameof(Scp1344Item.ActivateFinalEffects))]
    public class OnPlayerDisarmedPatch
    {
        public static bool Prefix(Scp1344Item __instance)
        {
            if (!NVG.TrackedSerials.Contains(__instance.ItemSerial))
                return true;

            if (__instance.Owner.IsHost)
                return true;

            WearOffNightVision(__instance.Owner);
            return false;
        }
    }

    [HarmonyPatch(typeof(Scp1344Item), nameof(Scp1344Item.ServerDropItem))]
    public static class Prevent3114DropPatch
    {
        public static bool Prefix(Scp1344Item __instance, bool spawn, ref ItemPickupBase __result)
        {
            if (!spawn)
                return true;

            if (__instance.Owner.IsHost)
                return true;

            if (!NVG.TrackedSerials.Contains(__instance.ItemSerial))
                return true;

            if (!__instance.IsWorn)
                return true;

            __instance.ServerSetStatus(Scp1344Status.Idle);
            WearOffNightVision(__instance.Owner);
            Timing.CallDelayed(Timing.WaitForOneFrame, () => __instance.OwnerInventory.ServerSelectItem(__instance.ItemSerial));
            return false;
        }
    }
}
