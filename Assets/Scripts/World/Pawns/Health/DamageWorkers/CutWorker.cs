using Assets.Scripts.World.Pawns;
using UnityEngine;
using Utilities;

namespace World.Pawns.Health.DamageWorkers
{
    /// <summary>
    /// The cut worker class
    /// </summary>
    /// <seealso cref="DamageWorker"/>
    public class CutWorker : DamageWorker
    {
        /// <summary>
        /// Chooses the hit part using the specified damage info
        /// </summary>
        /// <param name="damageInfo">The damage info</param>
        /// <param name="target">The target</param>
        /// <returns>The body part</returns>
        protected override BodyPart ChooseHitPart(DamageInfo damageInfo, Pawn target)
        {
            var healthUtils = Object.FindObjectOfType<HealthUtils>();

            return target.health.GetRandomBodyPart(damageInfo.Height, healthUtils.outside);
        }
    }
}
