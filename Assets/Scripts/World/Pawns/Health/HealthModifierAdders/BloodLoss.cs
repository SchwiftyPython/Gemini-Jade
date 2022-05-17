using World.Pawns.Health.HealthModifiers;

namespace World.Pawns.Health.HealthModifierAdders
{
    public class BloodLoss : HealthModAdder
    {
        public override void OnIntervalPassed(Pawn pawn, HealthMod cause)
        {
            var bleedRate = pawn.health.GetBleedRateTotal();

            if (bleedRate >= 0.1f)
            {
                //todo health utils adjust severity
            }
        }
    }
}
