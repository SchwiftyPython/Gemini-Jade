using UnityEngine;
using Utilities;
using World.Pawns.Health.HealthModifiers;

namespace World.Pawns.Health.HealthModifierAdders
{
    /// <summary>
    /// The blood loss class
    /// </summary>
    /// <seealso cref="HealthModAdder"/>
    public class BloodLoss : HealthModAdder
    {
        /// <summary>
        /// Ons the interval passed using the specified pawn
        /// </summary>
        /// <param name="pawn">The pawn</param>
        /// <param name="cause">The cause</param>
        public override void OnIntervalPassed(Pawn pawn, HealthMod cause)
        {
            var bleedRate = pawn.health.GetBleedRateTotal();

            var healthUtils = Object.FindObjectOfType<HealthUtils>();

            if (bleedRate >= 0.1f)
            {
                healthUtils.AdjustSeverity(pawn, healthModTemplate, bleedRate * 0.001f); //magic number stuff happening here
            }
            else
            {
                healthUtils.AdjustSeverity(pawn, healthModTemplate, -0.00033333333f); //magic number stuff happening here
            }
        }
    }
}
