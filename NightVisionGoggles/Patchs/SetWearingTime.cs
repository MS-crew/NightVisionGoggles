using System.Collections.Generic;
using System.Reflection.Emit;
using Exiled.API.Features;
using Exiled.API.Features.Pools;

using HarmonyLib;

using InventorySystem.Items.Usables.Scp1344;

using static HarmonyLib.AccessTools;

namespace NightVisionGoggles.Patchs
{
    [HarmonyPatch(typeof(Scp1344Item), nameof(Scp1344Item.Update))]
    [HarmonyPriority(Priority.HigherThanNormal)]
    public static class SetWearingTime
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newCodes = ListPool<CodeInstruction>.Pool.Get(instructions);
    
            Label skip = generator.DefineLabel();

            const int offset = -1;
            int index = newCodes.FindIndex(code => code.opcode == OpCodes.Ldc_R4 && (float)code.operand == Scp1344Item.ActivationTime) + offset;

            List<Label> jumpLabels = newCodes[index].ExtractLabels();
            newCodes[index].WithLabels(skip);

            newCodes.InsertRange(index,
            [
                new CodeInstruction(OpCodes.Call, PropertyGetter(typeof(NightVisionGoggles), nameof(NightVisionGoggles.NVG))).WithLabels(jumpLabels),
                new(OpCodes.Callvirt, PropertyGetter(typeof(NightVisionGoggles), nameof(NightVisionGoggles.TrackedSerials))),
                new(OpCodes.Ldarg_0),
                new(OpCodes.Callvirt, PropertyGetter(typeof(Scp1344Item), nameof(Scp1344Item.ItemSerial))),
                new(OpCodes.Callvirt, Method(typeof(HashSet<int>), nameof(HashSet<int>.Contains), [typeof(ushort)])),
                new(OpCodes.Brfalse, skip),

                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldc_R4, Plugin.Instance.Config.WearingTime),
                new(OpCodes.Ldc_I4_3),
                new(OpCodes.Call, Method(typeof(Scp1344Item), nameof(Scp1344Item.ServerUpdateTimedStatus))),
            ]);

            for (int i = 0; i < newCodes.Count; i++)
            {
                yield return newCodes[i];
            }

            ListPool<CodeInstruction>.Pool.Return(newCodes);
        }
    }
}
