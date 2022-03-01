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
    public class SKY
    {
        public static ConfigEntry<bool> SKY_MOVE;
        public static ConfigEntry<float> SKY_ROTATION;
        public static ConfigEntry<float> SKY_ROTATION_SPEED;
        public static ConfigEntry<float> SKY_EXPOSURE;
        public static HDRISky sky;
        public static void DO_SKY()
        {
            sky.rotation.Override(SKY_ROTATION.Value);
            sky.exposure.Override(SKY_EXPOSURE.Value);
        }
    }
}
