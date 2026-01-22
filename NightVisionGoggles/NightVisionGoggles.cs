using System.Collections.Generic;
using System.Linq;

using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Items;
using Exiled.API.Features.Spawn;
using Exiled.API.Features.Toys;
using Exiled.CustomItems.API.Features;

using MEC;

using Mirror;

using UnityEngine;

using Light = Exiled.API.Features.Toys.Light;

namespace NightVisionGoggles
{
    public class NightVisionGoggles : CustomGoggles
    {
        private readonly Dictionary<Player, CoroutineHandle> trackCameraCoroutines = [];

        internal static NightVisionGoggles NVG { get; private set; }

        public Dictionary<Player, Light> Lights { get; private set; } = [];

        public override uint Id { get; set; } = 757;

        public override float Weight { get; set; } = 1f;

        public override float WearingTime { get; set; } = 1;

        public override float RemovingTime { get; set; } = 1;

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

        protected override void OnWornGoggles(Player player, Scp1344 goggles)
        {
            Config config = Plugin.Instance.Config;

            if (config.PlaySoundOnUse)
            {
                Speaker speaker = Speaker.Create(player.Transform, true, false);
                speaker.ControllerId = (byte)player.Id;
                speaker.Play(config.SoundPath, false, true, false);
            }

            player.EnableEffect(EffectType.NightVision, intensity: config.NightVisionEffectInsentity);

            Light light = Light.Create(player.CameraTransform.position, player.Rotation.eulerAngles, null, spawn: true, color: config.LightSettings.Color);
            light.Transform.SetParent(player.Transform, true);

            light.SpotAngle = config.LightSettings.SpotAngle;
            light.InnerSpotAngle = config.LightSettings.InnerSpotAngle;

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

            if (config.LightSettings.TrackCameraRotation)
                trackCameraCoroutines[player] = Timing.RunCoroutine(TrackCameraRotation(player.CameraTransform, light.Transform, config.LightSettings.TrackCameraRotationInterval));
        }

        protected override void OnRemovedGoggles(Player player, Scp1344 goggles)
        {
            if (!Lights.ContainsKey(player))
                return;

            player.DisableEffect(EffectType.NightVision);

            if (Plugin.Instance.Config.LightSettings.TrackCameraRotation && trackCameraCoroutines.TryGetValue(player, out CoroutineHandle coroutine))
            {
                Timing.KillCoroutines(coroutine);
                trackCameraCoroutines.Remove(player);
            }

            if (Lights.TryGetValue(player, out Light lighObjectt))
            {
                GameObject lighObject = Lights[player]?.GameObject;
                Lights.Remove(player);
                NetworkServer.Destroy(lighObject);
            }

            foreach (Player ply in player.CurrentSpectatingPlayers)
            {
                Plugin.Instance.EventHandlers.DirtyPlayers.Remove(ply);
            }
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
