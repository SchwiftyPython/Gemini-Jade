using System.Collections.Generic;
using World.Pawns.Health.DamageTemplates;
using World.Things;

namespace World.Pawns.Health.DamageWorkers
{
    public class DamageWorker
    {
        public DamageTemplate template;

        public virtual DamageResult Apply(DamageInfo damageInfo, Thing target)
        {
            return null;
        }
    }
}
