// These are your imports, mostly you'll be needing these 5 for every plugin. Some will need more.

using BepInEx;
using BepInEx.Logging;
using BepInEx.Configuration;
using HarmonyLib;
// using static Obeliskial_Essentials.Essentials;
using System;


// The Plugin csharp file is used to 


// Make sure all your files have the same namespace and this namespace matches the RootNamespace in the .csproj file
namespace MoreScarabs{
    // These are used to create the actual plugin. If you don't need Obeliskial Essentials for your mod, 
    // delete the BepInDependency and the associated code "RegisterMod()" below.

    // If you have other dependencies, such as obeliskial content, make sure to include them here.
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    // [BepInDependency("com.stiffmeds.obeliskialessentials")] // this is the name of the .dll in the !libs folder.
    [BepInProcess("AcrossTheObelisk.exe")] //Don't change this
    [BepInIncompatibility("com.stiffmeds.obeliskialessentials")]

    // If PluginInfo isn't working, you are either:
    // 1. Using BepInEx v6
    // 2. Have an issue with your csproj file (not loading the analyzer or BepInEx appropriately)
    // 3. You have an issue with your solution file (not referencing the correct csproj file)


    public class Plugin : BaseUnityPlugin
    {
        
        // If desired, you can create configs for users by creating a ConfigEntry object here, 
        // and then use config = Config.Bind() to set the title, default value, and description of the config.
        // It automatically creates the appropriate configs.
        
        public static ConfigEntry<bool> GuaranteedSpawn { get; set; }
        public static ConfigEntry<bool> OnlySpawnJades { get; set; }
        public static ConfigEntry<int> PercentChanceToSpawn { get; set; }

        internal int ModDate = 20241024; //int.Parse(DateTime.Today.ToString("yyyyMMdd"));
        private readonly Harmony harmony = new(PluginInfo.PLUGIN_GUID);
        internal static ManualLogSource Log;
        private void Awake()
        {

            Log = Logger;
            Log.LogInfo($"{PluginInfo.PLUGIN_GUID} {PluginInfo.PLUGIN_VERSION} has loaded!");
            

            // Log.LogInfo($"{PluginInfo.PLUGIN_GUID} second test (pre-register)");

            GuaranteedSpawn = Config.Bind(new ConfigDefinition("Debug", "Guaranteed Spawn"), true, new ConfigDescription("It true, will guarantee 1 Scarab to spawn at the start of round 2."));
            OnlySpawnJades = Config.Bind(new ConfigDefinition("Debug", "Only Spawn Jades"), true, new ConfigDescription("If true, will force all scarabs spawned to be Jade"));
            PercentChanceToSpawn = Config.Bind(new ConfigDefinition("Debug", "Percent Chance to Spawn"), 7, new ConfigDescription("Set the percent chance for a scarab to spawn, overwritten by Guaranteed Spawn)"));
            
            // Log.LogInfo($"{PluginInfo.PLUGIN_GUID} Config Values. Spawn: " + GuaranteedSpawn.Value + " Jade: " + OnlySpawnJades.Value + " Percent: " + PercentChanceToSpawn.Value);

            // Register with Obeliskial Essentials
            // RegisterMod(
            //     _name: PluginInfo.PLUGIN_NAME,
            //     _author: "binbin",
            //     _description: "More Scarabs",
            //     _version: PluginInfo.PLUGIN_VERSION,
            //     _date: ModDate,
            //     _link: @"https://github.com/binbinmods/MoreScarabs"
            // );

            // Log.LogInfo($"{PluginInfo.PLUGIN_GUID} third test (pre patch)");

            // apply patches
            harmony.PatchAll();

            // Log.LogInfo($"{PluginInfo.PLUGIN_GUID} fourth test(post patch)");
            
        }
    }
}