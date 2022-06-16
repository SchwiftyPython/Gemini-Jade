namespace World.Pawns.Health.HealthModifierComponents.HealthModCompProperties
{
    public class SeverityPerDay : HealthModCompProps
    {
        public float severityPerDay;

        public bool showDaysToRecover;

        public bool showHoursToRecover;

        public SeverityPerDay()
        {
            compClass = typeof(HealthModifierComponents.SeverityPerDay);
        }
    }
}
