using System;
using System.Collections.Generic;
using System.Reflection.Emit;

using Exiled.API.Features;
using Exiled.API.Features.Pools;

using HarmonyLib;

using InventorySystem;
using InventorySystem.Items.Usables.Scp1344;

using static HarmonyLib.AccessTools;

namespace NightVisionGoggles.Patchs
{
    [HarmonyPatch(typeof(Scp1344Item), nameof(Scp1344Item.ServerUpdateDeactivating))]
    [HarmonyPriority(Priority.HigherThanNormal)]
    public class ServerUpdateDeactivatingPatch
    {
        internal static event Action<ReferenceHub> On1344WearOff;
        internal static void WearOffNightVision(ReferenceHub hub) => On1344WearOff?.Invoke(hub);

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> NewCodes = ListPool<CodeInstruction>.Pool.Get(instructions);

            List<Label> jumpLabels;
            Label Skip = generator.DefineLabel();

            const int offset = -1;
            int index = NewCodes.FindLastIndex(code => code.Calls(Method(typeof(Scp1344Item), nameof(Scp1344Item.ActivateFinalEffects)))) + offset;
            
            jumpLabels = NewCodes[index].ExtractLabels();
            NewCodes[index].labels.Add(Skip);

            NewCodes.InsertRange(index,
            [
               new CodeInstruction(OpCodes.Call,PropertyGetter(typeof(NightVisionGoggles), nameof(NightVisionGoggles.NVG))).WithLabels(jumpLabels),
                new(OpCodes.Callvirt,           PropertyGetter(typeof(NightVisionGoggles), nameof(NightVisionGoggles.TrackedSerials))),
                new(OpCodes.Ldarg_0),
                new(OpCodes.Callvirt,           PropertyGetter(typeof(Scp1344Item), nameof(Scp1344Item.ItemSerial))),
                new(OpCodes.Callvirt,           Method(typeof(HashSet<int>), nameof(HashSet<int>.Contains), [typeof(ushort)])),
                new(OpCodes.Brfalse_S,          Skip),
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldc_I4,             (int)Scp1344Status.Idle),
                new(OpCodes.Callvirt ,          PropertySetter(typeof(Scp1344Item), nameof(Scp1344Item.Status))),
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldc_R4, 0f),
                new(OpCodes.Stfld,              Field(typeof(Scp1344Item), nameof(Scp1344Item._useTime))),
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldc_R4, 0f),
                new(OpCodes.Stfld,              Field(typeof(Scp1344Item), nameof(Scp1344Item._cancelationTime))),
                new(OpCodes.Ldarg_0),
                new(OpCodes.Callvirt,           PropertyGetter(typeof(Scp1344Item), nameof(Scp1344Item.Owner))),
                new(OpCodes.Ldfld,              Field(typeof(ReferenceHub), nameof(ReferenceHub.inventory))),
                new(OpCodes.Ldc_I4_0),
                new(OpCodes.Callvirt,           Method(typeof(Inventory), nameof(Inventory.ServerSelectItem))),
                new(OpCodes.Ldarg_0),
                new(OpCodes.Callvirt,           PropertyGetter(typeof(Scp1344Item), nameof(Scp1344Item.Owner))),
                new(OpCodes.Call,               Method(typeof(ServerUpdateDeactivatingPatch), nameof(ServerUpdateDeactivatingPatch.WearOffNightVision))),
                new(OpCodes.Ret),
            ]);
            
            Label Skip2 = generator.DefineLabel();

            index = NewCodes.FindIndex(code => code.opcode == OpCodes.Ldc_R4 && (float)code.operand == Scp1344Item.DeactivationTime) + offset;

            Label targetLabel = (Label)NewCodes[index + 2].operand;
            Label targetLabel2 = (Label)NewCodes[index + 3].operand;

            NewCodes[index].labels.Add(Skip2);

            NewCodes.InsertRange(index,
            [
                new(OpCodes.Call,      PropertyGetter(typeof(NightVisionGoggles), nameof(NightVisionGoggles.NVG))),
                new(OpCodes.Callvirt,  PropertyGetter(typeof(NightVisionGoggles), nameof(NightVisionGoggles.TrackedSerials))),
                new(OpCodes.Ldarg_0),
                new(OpCodes.Callvirt,  PropertyGetter(typeof(Scp1344Item), nameof(Scp1344Item.ItemSerial))),
                new(OpCodes.Callvirt,  Method(typeof(HashSet<int>), nameof(HashSet<int>.Contains), [typeof(ushort)])),
                new(OpCodes.Brfalse_S, Skip2),
                new(OpCodes.Ldloc_0),
                new(OpCodes.Ldc_R4,    Plugin.Instance.Config.WearingOffTime),
                new(OpCodes.Blt_S,     targetLabel),
                new(OpCodes.Br_S,      targetLabel2)
            ]);
            
            for (int i = 0; i < NewCodes.Count; i++)
            {
                yield return NewCodes[i];
            }

            ListPool<CodeInstruction>.Pool.Return(NewCodes);
        }
    }
}
