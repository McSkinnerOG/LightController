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
    public class LIGHT
    {
        public static ConfigEntry<KeyCode> RELOAD_KEY_LIGHTMAP;
        public static ConfigEntry<Color> LIGHT_COLOR;
        public static ConfigEntry<float> LIGHT_AMOUNT;
        public static ConfigEntry<float> LIGHT_ROT_X;
        public static ConfigEntry<string> LIGHT_NAME;
        public static ConfigEntry<bool> NIGHT;
        public static ConfigEntry<bool> USE_CUSTOM_COLOR;
        public static ConfigEntry<bool> IND_ACTIVE;
        public static ConfigEntry<float> IND_DIFF_LIGHT;
        public static ConfigEntry<float> IND_REF_LIGHT;
        public static ConfigEntry<float> IND_REF_PROBE_MULT;
        public static DayNightController DNC;
        public static GameObject SUN_GO;
        public static Light SUN; 
        public static IndirectLightingController IND_LIGHT; 
        public static void DO_LIGHT() 
        {
            SUN.color = LIGHT_COLOR.Value;
            SUN.intensity = LIGHT_AMOUNT.Value;
            SUN_GO.transform.eulerAngles = new Vector3(LIGHT_ROT_X.Value, 0f, 0f);
        }

        public static void DO_IND_LIGHT() 
        {
            IND_LIGHT.active = IND_ACTIVE.Value;
            IND_LIGHT.indirectDiffuseLightingMultiplier.value = IND_DIFF_LIGHT.Value;
            IND_LIGHT.reflectionLightingMultiplier.value = IND_REF_LIGHT.Value;
            IND_LIGHT.reflectionProbeIntensityMultiplier.value = IND_REF_PROBE_MULT.Value; 
        }
    }
}
