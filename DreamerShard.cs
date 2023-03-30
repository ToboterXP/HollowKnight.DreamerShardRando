using ItemChanger;
using ItemChanger.Items;
using ItemChanger.Tags;
using ItemChanger.UIDefs;
using ConnectionMetadataInjector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RandomizerMod.RandomizerData;

namespace MoreEndings
{
    //The ItemChanger implementation of the Dreamer Shard item
    internal class DreamerShard : DreamerItem
    {
        static System.Random random = new();

        static string[] shopDescriptions = new string[]
        {
            "Not fit for human consumption",
            "Now available in pill form",
            "Fresh import from the Glimmering Realm",
            "Would you like a drink with this?",
            "You'll never guess where I got this",
            "Yeah. Lurien fell down the stairs, y'know. Pretty bad affair",
            "Being a locksmith in Hallownest must be a nightmare",
            "No, this isn't a mask shard. No, you're not allowed to eat it.",
            "Sometimes, you just have to buy pieces of someones face from some shady guy, y'know?",
            "Highly intoxicating",
            "Can cause internal hemmorage and being chased by the Fucking Sun",
            "<Insert rude word here>",
            "Heh. Dreamer Shart.",
            "A little souvenir for your travels, maybe?",
            "Seems awfully familar... probably family resemblance", 
            "If the Knight finds a Mask Shard, is that cannibalism?",
            "I'd make a Dream joke here, but the probability of that is astronomically low.",
            "Fresh from an illegal ivory trader",
            "Works great against insomnia",
            "You know, murdering ghosts is all fine and well, but when it comes to murdering shop keepers, that's just illegal",
            "Like, where does the Knight keep all these? Is he like a gelatinous cube of void, filled with random crap floating inside him?",
            "Hornet will be so jealous if you buy this",
            "Wtf did the Pale King do this time?!",
            "Honestly, this is a way better plan than what Pale King did in Vanilla.",
            "Fresh from the Dreamer factory. You don't wanna know what they make these from.",
            "Completely useless, move along."
        };

        //ItemChanger string that dynamically displays the amount of shards collected so far
        public class NameString : IString
        {

            public string Value { 
                get {
                    return "Dreamer Shard (" + DreamerShardRando.saveSettings.shardsCollected + "/" + DreamerShardRando.saveSettings.shardsRequired + ")";
                } 
            }


            public IString Clone()
            {
                return (IString)MemberwiseClone();
            }
        }

        //ItemChanger string that chooses a random shop description to display
        public class ShopString : IString
        {

            public ShopString()
            {
                if (Value == null)
                {
                    Value = shopDescriptions[random.Next(shopDescriptions.Length)];
                }
            }

            public string Value { get; }


            public IString Clone()
            {
                return (IString)MemberwiseClone();
            }
        }

        public DreamerShard()
        { 
            UIDef = new SplitUIDef()
            {
                preview = new BoxedString("Dreamer Shard"),
                name = new NameString(),
                shopDesc = new ShopString(),
                sprite = new ItemChangerSprite("Prompts.Monomon")
            };

            name = "Dreamer Shard";

            var tag = AddTag<InteropTag>();
            tag.Message = SupplementalMetadata.InteropTagMessage;
            tag.Properties["ModSource"] = DreamerShardRando.Instance.Name;
            tag.Properties["PoolGroup"] = PoolNames.Dreamer;
            tag.Properties["PinSprite"] = new ItemChangerSprite("Prompts.Monomon"); 
        }

        public override void GiveImmediate(GiveInfo info)
        {
            DreamerShardRando.saveSettings.shardsCollected++;

            int newDreamerCount = (int)DreamerShardRando.saveSettings.GetEquivalentDreamersDefeated(DreamerShardRando.saveSettings.shardsCollected);
            PlayerData.instance.guardiansDefeated = newDreamerCount-1;

            if (newDreamerCount == 1) dreamer = DreamerType.Lurien;
            if (newDreamerCount == 2) dreamer = DreamerType.Monomon;
            if (newDreamerCount == 3) dreamer = DreamerType.Herrah;

            if (newDreamerCount > 0)
            {
                base.GiveImmediate(info);
            }

            PlayerData.instance.guardiansDefeated = newDreamerCount;

            if (DreamerShardRando.saveSettings.shardsCollected > DreamerShardRando.saveSettings.shardsRequired)
            {
                HeroController.instance.AddGeo(50);
            }
        }
    }
}
