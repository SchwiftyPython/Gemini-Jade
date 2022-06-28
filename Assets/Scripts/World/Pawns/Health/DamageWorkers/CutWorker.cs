using Assets.Scripts.World.Pawns;
using UnityEngine;
using Utilities;

namespace World.Pawns.Health.DamageWorkers
{
    public class CutWorker : DamageWorker
    {
        protected override BodyPart ChooseHitPart(DamageInfo damageInfo, Pawn target)
        {
            var healthUtils = Object.FindObjectOfType<HealthUtils>();

            return target.health.GetRandomBodyPart(damageInfo.Height, healthUtils.outside);
        }
    }
}
