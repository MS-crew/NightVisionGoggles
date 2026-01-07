using System.ComponentModel;

using Exiled.API.Interfaces;

using UnityEngine;

namespace NightVisionGoggles
{
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;

        public bool Debug { get; set; } = false;

        public byte NightVisionEffectInsentity { get; set; } = 1;

        [Description("Simulate the temporary darkness when wearing the glasses")]
        public bool SimulateTemporaryDarkness { get; set; } =true;

        [Description("Wearing time (default 5)")]
        public float WearingTime { get; set; } = 1f;

        [Description("Removal time (default 5.1 set less than 5.1)")]
        public float WearingOffTime { get; set; } = 1f;

        public NightVisionGoggles NVG { get; set; } = new NightVisionGoggles();

        public LightSetting LightSettings { get; set; } = new LightSetting();

        public class LightSetting
        {
            public float Range { get; set; } = 50f;

            public float Intensity { get; set; } = 70f;

            public float SpotAngle { get; set; } = 90f;

            public float InnerSpotAngle { get; set; } = 0f;

            public byte MovementSmoothing { get; set; } = 60;

            public Color Color { get; set; } = Color.green;

            public bool TrackCameraRotation { get; set; } = true;

            public float TrackCameraRotationInterval { get; set; } = 0.1f;

            [Description("You can use this types `Spot, Point, Directional`")]
            public LightType LightType { get; set; } = LightType.Spot;

            public LightShadows ShadowType { get; set; } = LightShadows.None;
        }
    }
}
