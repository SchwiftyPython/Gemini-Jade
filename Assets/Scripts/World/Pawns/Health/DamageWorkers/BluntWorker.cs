using Assets.Scripts.World.Pawns;
using UnityEngine;
using Utilities;

namespace World.Pawns.Health.DamageWorkers
{
    public class BluntWorker : DamageWorker
    {
        private const float InternalDamageChance = 0.25f;
        
        protected override BodyPart ChooseHitPart(DamageInfo damageInfo, Pawn target)
        {
            var healthUtils = Object.FindObjectOfType<HealthUtils>();

            if(Random.Range(0f, 1f) < InternalDamageChance)
            {
                target.health.GetRandomBodyPart(damageInfo.Height, healthUtils.inside);
            }

            return target.health.GetRandomBodyPart(damageInfo.Height, healthUtils.outside);
        }
    }
}
