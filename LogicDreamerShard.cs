using RandomizerCore;
using RandomizerCore.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoreEndings
{
    //Representation of a DreamerShard in the Rando logic (to keep dreamer count updated)
    internal record LogicDreamerShard : LogicItem
    {
        Term id;
        Term dreamer;
        public LogicDreamerShard(Term id, Term dreamer) : base("Dreamer Shard")
        {
            this.id = id;
            this.dreamer = dreamer;
        }

        public override void AddTo(ProgressionManager pm)
        {
            int currentDreamerCount = (int)DreamerShardRando.saveSettings.GetEquivalentDreamersDefeated((uint)pm.Get(id));
            int newDreamerCount = (int)DreamerShardRando.saveSettings.GetEquivalentDreamersDefeated((uint)pm.Get(id) + 1);


            pm.Incr(dreamer, newDreamerCount - currentDreamerCount);
            pm.Incr(id, 1);
        }

        public override IEnumerable<Term> GetAffectedTerms()
        {
            return new List<Term>(){ id, dreamer};
        }
    }
}
