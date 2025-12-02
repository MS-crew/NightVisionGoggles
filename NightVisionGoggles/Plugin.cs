using System;

using Exiled.API.Features;
using Exiled.CustomItems.API;

using HarmonyLib;

using NightVisionGoggles.Patchs;

namespace NightVisionGoggles
{
    public class Plugin : Plugin<Config>
    {
        private Harmony harmony;

        public static Plugin Instance { get; private set; }

        public EventHandlers EventHandlers { get; private set; }

        public override string Author { get; } = "ZurnaSever";

        public override string Name { get; } = "NightVisionGoggles";

        public override string Prefix { get; } = "NightVisionGoggles";

        public override Version Version { get; } = new Version(1, 0, 0);

        public override Version RequiredExiledVersion { get; } = new Version(9, 10, 0);

        public override void OnEnabled()
        {
            Instance = this;
            EventHandlers = new EventHandlers();

            Config.NVG.Register();
            EventHandlers.Subscribe();

            harmony = new Harmony(Prefix + DateTime.Now.Ticks);
            DoDynamicPatchs();

            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            harmony.UnpatchAll(harmony.Id);
            Config.NVG.Unregister();
            EventHandlers.Unsubscribe();

            EventHandlers = null;
            Instance = null;

            base.OnDisabled();
        }

        private void DoDynamicPatchs()
        {
            if (Config.OverrideWearingTime)
            {
                harmony.PatchSingleType(typeof(SetWearingTime));
            }

            if (Config.OverrideWearingOffTime)
            {
                harmony.PatchSingleType(typeof(SetWearOffTime));
            }

            harmony.PatchSingleType(typeof(BlockBadEffect));
            harmony.PatchSingleType(typeof(BlockForceDrop));
        }
    }
}
