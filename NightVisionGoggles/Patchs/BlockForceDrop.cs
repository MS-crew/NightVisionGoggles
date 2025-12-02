using Exiled.API.Features.Items;

using HarmonyLib;

using InventorySystem;
using InventorySystem.Items;
using InventorySystem.Items.Pickups;
using InventorySystem.Items.Usables.Scp1344;

using static NightVisionGoggles.NightVisionGoggles;

namespace NightVisionGoggles.Patchs
{
    [HarmonyPatch(typeof(ItemBase), nameof(ItemBase.ServerDropItem), typeof(bool))]
    public static class BlockForceDrop
    {
        public static void Postfix(ItemBase __instance, ref ItemPickupBase __result)
        {
            if (__result == null)
                return;

            if (!NVG.TrackedSerials.Contains(__result.Info.Serial))
                return;

            if (!Item.Get(__instance).Is(out Scp1344 scp1344Item))
                return;

            if (scp1344Item.Status != Scp1344Status.Deactivating)
                return;

            __result.PreviousOwner.Hub.inventory.ServerAddItem(__result.ItemId.TypeId, ItemAddReason.Undefined, __result.Info.Serial, __result);
            __result.DestroySelf();
        }
    }
}
