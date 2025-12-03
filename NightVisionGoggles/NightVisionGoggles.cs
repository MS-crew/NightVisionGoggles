using System.Collections.Generic;
using System.Linq;

using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.API.Features.Toys;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Scp1344;

using InventorySystem.Items.Usables.Scp1344;

using MEC;

using Mirror;

using NightVisionGoggles.Patchs;

using PlayerRoles.FirstPersonControl.Thirdperson.Subcontrollers.Wearables;

using UnityEngine;

using YamlDotNet.Core;
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

        [YamlIgnore]
        private Dictionary<Player, CoroutineHandle> trackCameraCoroutines = new Dictionary<Player, CoroutineHandle>();

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

            foreach (CoroutineHandle handle in trackCameraCoroutines.Values)
            {
                Timing.KillCoroutines(handle);
            }

            trackCameraCoroutines.Clear();

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
            player.EnableEffect(EffectType.NightVision, intensity: config.NightVisionEffectInsentity);
            player.ReferenceHub.EnableWearables(WearableElements.Scp1344Goggles);

            if (config.SimulateTemporaryDarkness)
                player.EnableEffect(EffectType.Blinded, 255, float.MaxValue, true);

            Light light = Light.Create(null, null, null, spawn: true, color: config.LightSettings.Color);
            light.Transform.SetParent(player.Transform, false);

            light.SpotAngle = config.LightSettings.SpotAngle;
            light.InnerSpotAngle = config.LightSettings.InnerSpotAngle;

            light.Rotation = player.Rotation;
            light.Position = player.CameraTransform.position;

            light.Range = config.LightSettings.Range;
            light.Intensity = config.LightSettings.Intensity;

            light.LightType = config.LightSettings.LightType;
            light.ShadowType = config.LightSettings.ShadowType;
            light.MovementSmoothing = config.LightSettings.MovementSmoothing;

            Lights[player] = light;

            foreach (Player ply in Player.List)
            {
                if (ply == player)
                    continue;

                if (player.CurrentSpectatingPlayers.Contains(ply))
                {
                    Plugin.Instance.EventHandlers.DirtyPlayers.Add(ply);
                    continue;
                }     

                ply.HideNetworkIdentity(light.Base.netIdentity);
            }

            Log.Debug($"{player.Nickname} : Activated NVG");

            if (config.LightSettings.TrackCameraRotation)
                trackCameraCoroutines[player] = Timing.RunCoroutine(TrackCameraRotation(player.CameraTransform, light.Transform, config.LightSettings.TrackCameraRotationInterval));
        }

        public void DisableNVG(ReferenceHub hub)
        {
            Player player = Player.Get(hub);

            player.DisableEffect(EffectType.NightVision);
            player.ReferenceHub.DisableWearables(WearableElements.Scp1344Goggles);

            if (Plugin.Instance.Config.SimulateTemporaryDarkness)
                player.DisableEffect(EffectType.Blinded);

            if (Plugin.Instance.Config.LightSettings.TrackCameraRotation && trackCameraCoroutines.TryGetValue(player, out CoroutineHandle coroutine))
            {
                Timing.KillCoroutines(coroutine);
                trackCameraCoroutines.Remove(player);
            }

            GameObject lighObject = Lights[player]?.GameObject;

            Lights.Remove(player);
            NetworkServer.Destroy(lighObject);

            foreach (Player ply in player.CurrentSpectatingPlayers)
            {
                Plugin.Instance.EventHandlers.DirtyPlayers.Remove(ply);
            }

            Log.Debug($"{player.Nickname} : Deactivated NVG");
        }

        private IEnumerator<float> TrackCameraRotation(Transform camera, Transform light, float syncInterval)
        {
            while (camera != null && light != null)
            {
                float pitch = camera.localRotation.eulerAngles.x;
                Quaternion targetRotation = Quaternion.AngleAxis(pitch, Vector3.right);

                if (light.localRotation != targetRotation)
                    light.localRotation = targetRotation;

                yield return syncInterval;
            }
        }
    }
}
