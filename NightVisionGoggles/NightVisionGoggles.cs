using System.Collections.Generic;
using System.Linq;

using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Scp1344;

using InventorySystem.Items.Usables.Scp1344;

using Mirror;

using NightVisionGoggles.Patchs;

using PlayerRoles.FirstPersonControl.Thirdperson.Subcontrollers.Wearables;

using UnityEngine;

using YamlDotNet.Serialization;

using Light = Exiled.API.Features.Toys.Light;
using Scp1344Event = Exiled.Events.Handlers.Scp1344;

namespace NightVisionGoggles
{
    [CustomItem(ItemType.SCP1344)]
    public class NightVisionGoggles : CustomItem
    {
        internal static NightVisionGoggles NVG { get; private set; }

        [YamlIgnore]
        public Dictionary<Player, Light> Lights { get; private set; } = [];

        public override uint Id { get; set; } = 757;

        public override float Weight { get; set; } = 1f;

        [YamlIgnore]
        public override ItemType Type { get; set; } = ItemType.SCP1344;

        public override string Name { get; set; } = "Night Vision Goggles";

        public override string Description { get; set; } = "A night-vision device (NVD), also known as a Night-Vision goggle (NVG), is an optoelectronic device that allows visualization of images in low levels of light, improving the user's night vision.";

        public override SpawnProperties SpawnProperties { get; set; } = new SpawnProperties();

        public override void Init()
        {
            base.Init();
            NVG = this;
        }

        public override void Destroy()
        {
            NVG = null;
            base.Destroy();
        }

        protected override void SubscribeEvents()
        {
            BlockBadEffect.On1344WearOff += DisableNVG;
            Scp1344Event.ChangedStatus += OnChangedStatus;
            base.SubscribeEvents();
        }

        protected override void UnsubscribeEvents()
        {
            BlockBadEffect.On1344WearOff -= DisableNVG;
            Scp1344Event.ChangedStatus -= OnChangedStatus;
            base.UnsubscribeEvents();
        }

        protected override void OnWaitingForPlayers()
        {
            Lights.Clear();
            base.OnWaitingForPlayers();
        }

        private void OnChangedStatus(ChangedStatusEventArgs ev)
        {
            if (!Check(ev.Item))
                return;

            if (ev.Scp1344Status == Scp1344Status.Active)
                ActivateNVG(ev.Player);
        }

        private void ActivateNVG(Player player)
        {
            Config config = Plugin.Instance.Config;

            player.DisableEffect(EffectType.Scp1344);
            player.EnableEffect(EffectType.NightVision, intensity: config.NightVisionInsentity);
            player.ReferenceHub.EnableWearables(WearableElements.Scp1344Goggles);

            if (config.SimulateTemporaryDarkness)
                player.EnableEffect(EffectType.Blinded, 255, float.MaxValue, true);

            Light light = Light.Create(null, null, null, spawn: true, color: config.FakeLightSettings.Color);
            light.Transform.SetParent(player.Transform, false);

            light.Range = config.FakeLightSettings.Range;
            light.Intensity = config.FakeLightSettings.Intensity;
            light.ShadowType = config.FakeLightSettings.ShadowType;

            Lights[player] = light;

            foreach (Player ply in Player.List)
            {
                if (ply == player)
                    continue;

                if (player.CurrentSpectatingPlayers.Contains(ply))
                {
                    Plugin.Instance.EventHandlers.DirtyPlayers.Add(ply);//next update fix
                    continue;
                }
                    

                ply.HideNetworkIdentity(light.Base.netIdentity);
            }

            Log.Debug($"{player.Nickname} : Activated NVG");
        }

        public void DisableNVG(ReferenceHub hub)
        {
            Player player = Player.Get(hub);

            player.DisableEffect(EffectType.NightVision);
            player.ReferenceHub.DisableWearables(WearableElements.Scp1344Goggles);

            if (Plugin.Instance.Config.SimulateTemporaryDarkness)
                player.DisableEffect(EffectType.Blinded);

            GameObject lighObject = Lights[player]?.GameObject;
            Lights.Remove(player);
            NetworkServer.Destroy(lighObject);

            foreach (Player ply in player.CurrentSpectatingPlayers)
            {
                Plugin.Instance.EventHandlers.DirtyPlayers.Remove(ply);
            }

            Log.Debug($"{player.Nickname} : Deactivated NVG");
        }
    }
}
