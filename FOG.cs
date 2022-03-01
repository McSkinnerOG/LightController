using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

namespace LightController
{
    public class FOG
    {
        public static ConfigEntry<bool> FOG_ACTIVE;
        public static ConfigEntry<Color> FOG_COLOR;
        public static ConfigEntry<Color> FOG_ALBEDO;
        public static ConfigEntry<float> FOG_BASE_HEIGHT;
        public static ConfigEntry<float> FOG_MAX_HEIGHT;
        public static ConfigEntry<float> FOG_DISTANCE;
        public static ConfigEntry<float> FOG_FREEPATH;
        public static Fog fog;
        public static void DO_FOG()
        {
            fog.active = FOG_ACTIVE.Value;
            fog.color.Override(FOG_COLOR.Value);
            fog.maxFogDistance.Override(FOG_DISTANCE.Value);
            fog.meanFreePath.Override(FOG_FREEPATH.Value);
            fog.tint.Override(FOG_COLOR.Value);
        }
    }
}
