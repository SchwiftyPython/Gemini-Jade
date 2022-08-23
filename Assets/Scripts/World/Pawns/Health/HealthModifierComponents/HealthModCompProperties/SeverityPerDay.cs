namespace World.Pawns.Health.HealthModifierComponents.HealthModCompProperties
{
    /// <summary>
    /// The severity per day class
    /// </summary>
    /// <seealso cref="HealthModCompProps"/>
    public class SeverityPerDay : HealthModCompProps
    {
        /// <summary>
        /// The severity per day
        /// </summary>
        public float severityPerDay;

        /// <summary>
        /// The show days to recover
        /// </summary>
        public bool showDaysToRecover;

        /// <summary>
        /// The show hours to recover
        /// </summary>
        public bool showHoursToRecover;

        /// <summary>
        /// Initializes a new instance of the <see cref="SeverityPerDay"/> class
        /// </summary>
        public SeverityPerDay()
        {
            compClass = typeof(HealthModifierComponents.SeverityPerDay);
        }
    }
}
