using System.Collections.Generic;
using UnityEngine;
using World.Pawns.Health.DamageTemplates;
using World.Things;

namespace World.Pawns.Health.DamageWorkers
{
    public class DamageWorker
    {
        public DamageTemplate template;

        public virtual DamageResult Apply(DamageInfo damageInfo, Thing target)
        {
            var damageResult = new DamageResult();
            
            //todo check if target is spawned -- then play impact sound if true

            if (!target.template.useHitPoints)
            {
                return damageResult;
            }

            if (!damageInfo.Template.damagesHp)
            {
                return damageResult;
            }

            var damageAmount = damageInfo.Amount;
                
            //todo check if target is building or plant

            damageResult.totalDamage = Mathf.Min(target.HitPoints, Mathf.RoundToInt(damageAmount));

            target.HitPoints -= Mathf.RoundToInt(damageResult.totalDamage);

            if (target.HitPoints > 0)
            {
                return damageResult;
            }

            target.HitPoints = 0;
                    
            target.Kill(damageInfo);

            return damageResult;
        }
    }
}
