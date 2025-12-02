using System.ComponentModel;

using Exiled.API.Interfaces;

using UnityEngine;

namespace NightVisionGoggles
{
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;

        public bool Debug { get; set; } = false;

        public byte NightVisionInsentity { get; set; } = 5;

        [Description("Simulate the temporary darkness when wearing the glasses")]
        public bool SimulateTemporaryDarkness { get; set; } = true;

        [Description("Wearing time (default 5)")]
        public float WearingTime { get; set; } = 1f;

        [Description("Removal time (default 5.1 set less than 5.1)")]
        public float WearingOffTime { get; set; } = 1f;

        public NightVisionGoggles NVG { get; set; } = new NightVisionGoggles();

        public FakeLightSetting FakeLightSettings { get; set; } = new FakeLightSetting();

        public class FakeLightSetting
        {
            public float Range { get; set; } = 50f;

            public float Intensity { get; set; } = 70f;

            public Color Color { get; set; } = Color.green;

            public LightShadows ShadowType { get; set; } = LightShadows.None;
        }
    }
}
