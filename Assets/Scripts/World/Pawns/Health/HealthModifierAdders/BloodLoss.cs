using UnityEngine;
using Utilities;
using World.Pawns.Health.HealthModifiers;

namespace World.Pawns.Health.HealthModifierAdders
{
    public class BloodLoss : HealthModAdder
    {
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
