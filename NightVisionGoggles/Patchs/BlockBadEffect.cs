using System;

using HarmonyLib;

using InventorySystem.Items.Usables.Scp1344;

using static NightVisionGoggles.NightVisionGoggles;

namespace NightVisionGoggles.Patchs
{
    [HarmonyPatch(typeof(Scp1344Item), nameof(Scp1344Item.ActivateFinalEffects))]
    public class BlockBadEffect
    {
        internal static event Action<ReferenceHub> On1344WearOff;

        public static bool Prefix(Scp1344Item __instance)
        {
            if (!NVG.TrackedSerials.Contains(__instance.ItemSerial))
                return true;

            On1344WearOff?.Invoke(__instance.Owner);
            return false;
        }
    }
}
