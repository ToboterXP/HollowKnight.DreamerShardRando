using ItemChanger;
using Modding;
using RandomizerCore;
using RandomizerCore.Logic;
using RandomizerCore.LogicItems;
using RandomizerMod.Logging;
using RandomizerMod.RandomizerData;
using RandomizerMod.RC;
using RandomizerMod.Settings;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UObject = UnityEngine.Object;

namespace MoreEndings
{
    public class SaveData
    {
        public bool enabled = false;

        public uint totalShardCount = 15;
        public uint shardsRequired = 10;

        public uint shardsCollected = 0;

        //returns what internal count of dreamers collected equates to the given count of dreamer shards
        public uint GetEquivalentDreamersDefeated(uint shardCount)
        {
            uint ret = 3 * shardCount / shardsRequired;
            return ret > 3 ? 3 : ret;
        }

        //returns how many shards equate to this internal count of dreamers
        public uint GetNDreamerEquivalent(uint n)
        {
            if (shardsRequired % 3 == 0) return shardsRequired / 3 * n;
            return shardsRequired / 3 * n + 1;
        }

    }
    //the mods main class

    public class DreamerShardRando : Mod, ILocalSettings<SaveData>
    {
        internal static DreamerShardRando Instance;
        public static SaveData saveSettings { get; set; } = new SaveData();
        public void OnLoadLocal(SaveData s) => saveSettings = s;
        public SaveData OnSaveLocal() => saveSettings;

        public static string ModName = "Dreamer Shard Rando";

        public DreamerShardRando() : base(ModName)
        {
            Instance = this;
        }

        public override string GetVersion()
        {
            return "v1.0.0.0";
        }

        public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
        {
            Log("Initializing");

            Instance = this;

            Menu.Hook();

            var item = new DreamerShard();
            Finder.DefineCustomItem(item);

            RCData.RuntimeLogicOverride.Subscribe(100000f, AddItems);
            RequestBuilder.OnUpdate.Subscribe(200f, PlaceDreamerShards);

            RequestBuilder.OnUpdate.Subscribe(-499f, AddGroup);

            if (ModHooks.GetMod("RandoSettingsManager") is Mod) {
                RandomizerSettingsInterop.Hook();
            }

            SettingsLog.AfterLogSettings += AddDreamerShardSettings;

            CondensedSpoilerLogger.AddCategory("Dreamer Shards", (args) => true, new List<string>() { "Dreamer Shard" });

            Log("Initialized");
        }

        //Adds the mods settings to the Settings Log
        public void AddDreamerShardSettings(LogArguments args, System.IO.TextWriter tw)
        {
            tw.WriteLine("Logging Dreamer Shards Settings:");
            using Newtonsoft.Json.JsonTextWriter jtw = new(tw) { CloseOutput = false, };
            JsonUtil._js.Serialize(jtw, saveSettings);
            tw.WriteLine();
        }

        //Runs early in OnUpdate, adds the group designation for the Dreamer Shards
        public void AddGroup(RequestBuilder rb) {

            rb.OnGetGroupFor.Subscribe(0, MatchDreamerShardGroup);


            bool MatchDreamerShardGroup(RequestBuilder rb, string item, RequestBuilder.ElementType type, out GroupBuilder gb)
            {
                if (item == "Dreamer Shard" && (type == RequestBuilder.ElementType.Unknown || type == RequestBuilder.ElementType.Item))
                {
                    gb = rb.GetGroupFor(ItemNames.Dreamer);
                    return true;
                }
                gb = default;
                return false;
            }
        }

        //Adds the logical dreamer item
        public void AddItems(GenerationSettings settings, LogicManagerBuilder logic)
        {
            if (saveSettings.enabled)
            {
                if (saveSettings.shardsRequired > saveSettings.totalShardCount) saveSettings.totalShardCount = saveSettings.shardsRequired;

                Term term = logic.GetOrAddTerm("DREAMERSHARD");
                logic.AddItem(new LogicDreamerShard(term, logic.GetTerm("DREAMER")));
            }

        }

        //Runs late in OnUpdate, removes the dreamers, adds the appropriate number of shards
        public void PlaceDreamerShards(RequestBuilder rb)
        {
            if (saveSettings.enabled)
            {
                rb.RemoveItemByName(PlaceholderItem.Prefix+ItemNames.Dreamer);
                rb.RemoveItemByName(ItemNames.Lurien);
                rb.RemoveItemByName(ItemNames.Monomon);
                rb.RemoveItemByName(ItemNames.Herrah);
                for (int i = 0; i < saveSettings.totalShardCount; i++)
                {
                    rb.AddItemByName("Dreamer Shard");
                }
            }
        }
    }
}