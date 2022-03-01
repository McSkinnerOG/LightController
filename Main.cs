using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.SceneManagement; 

namespace LightController
{
    [BepInPlugin(GUID, MODNAME, VERSION)]
    public class Main : BaseUnityPlugin
    {
        #region[Declarations] 
        public const string MODNAME = "LightController", AUTHOR = "ValidUser", GUID = AUTHOR + "_" + MODNAME, VERSION = "1.0.0.0"; 
        internal readonly ManualLogSource log;
        internal readonly Harmony harmony;
        internal readonly Assembly assembly;
        public readonly string modFolder;
        public Main()
        {
            log = Logger;
            harmony = new Harmony(GUID);
            assembly = Assembly.GetExecutingAssembly();
            modFolder = Path.GetDirectoryName(assembly.Location);
            InitConfig();
        }
        public void Start()
        {
            harmony.PatchAll(assembly);
            harmony.PatchAll(typeof(Patches));
        }
        #endregion

        // VOLUMES
        public Volume VOLUMES;  
        public VisualEnvironment ENV; 
        public Exposure EXP;
        public static ConfigEntry<bool> REFLECTION_PROBES;
        public GameObject[] reflectionProbes; 
        public AmbientOcclusion AMB; 
        public void InitConfig()
        {
            //LIGHT
            LIGHT.USE_CUSTOM_COLOR = Config.Bind("LIGHT", "CUSTOMS TOGGLE", false, "Custom Color Use toggle.");
            LIGHT.NIGHT = Config.Bind("LIGHT", "NIGHT TOGGLE", false, "Night toggle.");
            LIGHT.LIGHT_NAME = Config.Bind(new ConfigDefinition("LIGHT", "Light Scene"), "vp_airfield", new ConfigDescription("Lighting Scene to use!", new AcceptableValueList<string>("vp_airfield", "vp_atron", "vp_bathurst", "vp_ebisu", "vp_fiorano", "vp_irwindale", "vp_japan", "vp_losAngeles", "vp_pacifichills", "vp_parking", "vp_petersburg", "vp_redring", "vp_redrock", "vp_showroom", "vp_silverstone", "vp_winterfell", "vp_nring", "vp_mondello")));
            LIGHT.LIGHT_AMOUNT = Config.Bind("LIGHT", "Light Amount", 1f, new ConfigDescription("Light Amount.", new AcceptableValueRange<float>(1, 1000f)));
            LIGHT.LIGHT_ROT_X = Config.Bind("LIGHT", "Light Rotation", 45f, new ConfigDescription("Light Rotation.", new AcceptableValueRange<float>(1, 359f)));
            LIGHT.LIGHT_COLOR = Config.Bind("LIGHT", "Light Color", new Color(1,1,1,1), new ConfigDescription("Color/Hue emmited by the 'sun'."));
            REFLECTION_PROBES = Config.Bind("LIGHT", "PROBES TOGGLE", false, "Probes toggle.");
            LIGHT.RELOAD_KEY_LIGHTMAP = Config.Bind("KEYBINDS", "RELOAD Keybind", KeyCode.N, "RELOAD Key.");
            
            LIGHT.IND_ACTIVE = Config.Bind("IND_LIGHT", "INDIRECT/WORLD LIGHT TOGGLE", true, "Night toggle.");
            LIGHT.IND_DIFF_LIGHT = Config.Bind("IND_LIGHT", "Indirect Light Multiplier", 2f, new ConfigDescription("Indirect Light Multiplier.", new AcceptableValueRange<float>(0f, 100f)));
            LIGHT.IND_REF_LIGHT = Config.Bind("IND_LIGHT", "Indirect Reflection Multiplier", 1f, new ConfigDescription("Indirect Reflection Multiplier.", new AcceptableValueRange<float>(0f, 100f)));
            LIGHT.IND_REF_PROBE_MULT = Config.Bind("IND_LIGHT", "Indirect Reflection Probe Multiplier", 1.5f, new ConfigDescription("Indirect Reflection Probe Multiplier.", new AcceptableValueRange<float>(0f, 100f))); 

            // SKY
            SKY.SKY_MOVE = Config.Bind("SKY", "Sky Mover", false, "Sky Moving.");
            SKY.SKY_ROTATION = Config.Bind("SKY", "Sky Rotation", 0f, new ConfigDescription("Skybox Rotation.", new AcceptableValueRange<float>(0f, 360f)));
            SKY.SKY_ROTATION_SPEED = Config.Bind("SKY", "Sky Rotation Speed", 1f, new ConfigDescription("Skybox Rotation.", new AcceptableValueRange<float>(0.001f, 0.1f)));
            SKY.SKY_EXPOSURE = Config.Bind("SKY", "Sky Exposure", 1.75f, new ConfigDescription("Skybox Exposure.", new AcceptableValueRange<float>(0f, 10f))); 
            //FOG
            FOG.FOG_ACTIVE = Config.Bind("FOG", "FOG TOGGLE", true, "Fog toggle."); 
            FOG.FOG_COLOR = Config.Bind("FOG", "Fog Color", new Color(1,1,1,1), new ConfigDescription("Color/Hue emmited by the 'fog'.")); 
            FOG.FOG_DISTANCE = Config.Bind("FOG", "Fog Distance", 5000f, new ConfigDescription("Fog Distance.", new AcceptableValueRange<float>(0f, 10000f)));
            FOG.FOG_FREEPATH = Config.Bind("FOG", "Fog Freepath", 500f, new ConfigDescription("Fog Freepath 'Density???' Lower = Lower heavier fog.", new AcceptableValueRange<float>(0f, 1000f))); 
        } 
        void FindVolumes()
        {
            LIGHT.DNC = GameObject.Find("lighting_controller").GetComponent<DayNightController>();
            LIGHT.SUN_GO = GameObject.Find("sunlight");
            LIGHT.SUN = GameObject.Find("sunlight").GetComponent<Light>();
            VOLUMES = GameObject.Find("sunlight").GetComponentInParent<Volume>();
            SKY.sky = (HDRISky)VOLUMES.profile.components[0];
            ENV = (VisualEnvironment)VOLUMES.profile.components[1];
            FOG.fog = (Fog)VOLUMES.profile.components[2];
            if (LIGHT.SUN_GO.transform.parent.transform.gameObject.name.ToLowerInvariant().Contains("night"))
            {
                AMB = (AmbientOcclusion)VOLUMES.profile.components[3];
                LIGHT.IND_LIGHT = (IndirectLightingController)VOLUMES.profile.components[4];
                EXP = (Exposure)VOLUMES.profile.components[5];
            } else { EXP = (Exposure)VOLUMES.profile.components[3]; } 
            reflectionProbes = (from x in UnityEngine.Object.FindObjectsOfType<ReflectionProbe>(true) select x.gameObject).ToArray<GameObject>();
            if (reflectionProbes.ToList().Count > 0) { foreach (GameObject gameObject in reflectionProbes) { gameObject.SetActive(REFLECTION_PROBES.Value); } }
        }

        void Update()
        { 
            if (!SceneManager.GetActiveScene().name.ToLowerInvariant().Contains("empty"))
            {
                if (SKY.SKY_MOVE.Value == true) { SKY.sky.rotation.Override(SKY.sky.rotation.value += SKY.SKY_ROTATION_SPEED.Value); } 
                if (Input.GetKeyDown(LIGHT.RELOAD_KEY_LIGHTMAP.Value))
                {
                    FindVolumes();
                    if (LIGHT.NIGHT.Value == true)
                    {
                        LIGHT.DNC.TryApplyMode(LIGHT.LIGHT_NAME.Value + "_night", !LIGHT.NIGHT.Value);
                        if (LIGHT.USE_CUSTOM_COLOR.Value == true)
                        {
                            FindVolumes();
                            FOG.DO_FOG();
                            LIGHT.DO_LIGHT();
                            LIGHT.DO_IND_LIGHT();
                            SKY.DO_SKY();
                        } 
                    }
                    else if (LIGHT.NIGHT.Value == false)
                    {
                        LIGHT.DNC.TryApplyMode(LIGHT.LIGHT_NAME.Value, !LIGHT.NIGHT.Value);
                        if (LIGHT.USE_CUSTOM_COLOR.Value == true)
                        {
                            FindVolumes();
                            FOG.DO_FOG();
                            LIGHT.DO_LIGHT();
                            LIGHT.DO_IND_LIGHT();
                            SKY.DO_SKY();
                        }
                    }
                }
            }
        }
    }
}