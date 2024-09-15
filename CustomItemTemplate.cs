using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using LobbyCompatibility.Attributes;
using LobbyCompatibility.Enums;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace CustomItemTemplate;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency(LethalLib.Plugin.ModGUID)]
[BepInDependency("BMX.LobbyCompatibility", BepInDependency.DependencyFlags.HardDependency)]
[LobbyCompatibility(CompatibilityLevel.ClientOnly, VersionStrictness.None)]
public class CustomItemTemplate : BaseUnityPlugin{
    public static CustomItemTemplate Instance { get; private set; } = null!;
    internal new static ManualLogSource Logger { get; private set; } = null!;
    internal static Harmony? Harmony { get; set; }


    public static AssetBundle CustomAssets;

    private void Awake(){
    // Called once when both the game and plugin are loaded
    // This is where we will put all of our custom code
        Logger = base.Logger;
        Instance = this;

        // ---- ITEM LOADING CODE ----
        string asset_bundle_name = "ASSET_BUNDLE_NAME_HERE"; // Name of asset file. 
        string item_data_name = "directory/to/itemdata.asset"; // ItemData directory in Unity


        string assembly_location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        CustomAssets = AssetBundle.LoadFromFile(Path.Combine(assembly_location, asset_bundle_name));
        if (CustomAssets == null){
            Logger.LogError("Failed to load custom assets.");
            return;
        }


        int iRarity = 30; // Item rarity, 1-100
        Item MY_ITEM_NAME = CustomAssets.LoadAsset<Item>(item_data_name);
        if (MY_ITEM_NAME == null){
            Logger.LogError("Failed to load item " + item_data_name + " from " + asset_bundle_name);
        }
        LethalLib.Modules.NetworkPrefabs.RegisterNetworkPrefab(MY_ITEM_NAME.spawnPrefab);
        LethalLib.Modules.Items.RegisterScrap(MY_ITEM_NAME, iRarity, LethalLib.Modules.Levels.LevelTypes.All);


        // Method called to add, or patch, this plugin into the game
        Patch();

        // Lets user know the plugin has loaded successfully
        Logger.LogInfo($"{MyPluginInfo.PLUGIN_GUID} v{MyPluginInfo.PLUGIN_VERSION} has loaded!");
    }

    internal static void Patch(){
    // Custom method we create to "patch" our plugin into the game

        // Checks if an instance of Harmony exists. If not, creates an instance. 
        Harmony ??= new Harmony(MyPluginInfo.PLUGIN_GUID);

        Logger.LogDebug("Patching...");

        // Calls Harmonys built in "PatchAll" method.
        // This  yoaddsur methods into the game whenever the game is run.
        Harmony.PatchAll();

        Logger.LogDebug("Finished patching!");
    }

    internal static void Unpatch(){
    // Used to unload/remove the mod. 
        Logger.LogDebug("Unpatching...");

        Harmony?.UnpatchSelf();

        Logger.LogDebug("Finished unpatching!");
    }
}
