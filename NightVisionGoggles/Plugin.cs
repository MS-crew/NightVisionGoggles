using System;

using Exiled.API.Features;
using Exiled.CustomItems.API;

namespace NightVisionGoggles
{
    public class Plugin : Plugin<Config>
    {
        public static Plugin Instance { get; private set; }

        public EventHandlers EventHandlers { get; private set; }

        public override string Author { get; } = "MS";

        public override string Name { get; } = "NightVisionGoggles";

        public override string Prefix { get; } = "NightVisionGoggles";

        public override Version Version { get; } = new Version(1, 3, 0);

        public override Version RequiredExiledVersion { get; } = new Version(9, 13, 0);

        public override void OnEnabled()
        {
            Instance = this;
            EventHandlers = new EventHandlers();

            Config.NVG.Register();
            EventHandlers.Subscribe();

            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            EventHandlers.Unsubscribe();
            Config.NVG.Unregister();
            
            EventHandlers = null;
            Instance = null;

            base.OnDisabled();
        }
    }
}
