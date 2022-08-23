using Assets.Scripts.World.Pawns;
using UnityEngine;
using Utilities;

namespace World.Pawns.Health.DamageWorkers
{
    /// <summary>
    /// The blunt worker class
    /// </summary>
    /// <seealso cref="DamageWorker"/>
    public class BluntWorker : DamageWorker
    {
        /// <summary>
        /// The internal damage chance
        /// </summary>
        private const float InternalDamageChance = 0.25f;
        
        /// <summary>
        /// Chooses the hit part using the specified damage info
        /// </summary>
        /// <param name="damageInfo">The damage info</param>
        /// <param name="target">The target</param>
        /// <returns>The body part</returns>
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
