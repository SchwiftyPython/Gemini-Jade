namespace World.Pawns.Health.HealthModifierComponents
{
    /// <summary>
    /// The severity per day class
    /// </summary>
    /// <seealso cref="HealthModComp"/>
    public class SeverityPerDay : HealthModComp
    {
        /// <summary>
        /// The severity update interval
        /// </summary>
        protected const int SeverityUpdateInterval = 200;
        
        /// <summary>
        /// The interval check counter
        /// </summary>
        private int _intervalCheckCounter = 0;

        /// <summary>
        /// Gets the value of the props
        /// </summary>
        private HealthModCompProperties.SeverityPerDay Props => (HealthModCompProperties.SeverityPerDay) props;
        
        //todo override label stuff

        /// <summary>
        /// Posts the tick using the specified severity adjustment
        /// </summary>
        /// <param name="severityAdjustment">The severity adjustment</param>
        public override void PostTick(ref float severityAdjustment)
        {
            base.PostTick(ref severityAdjustment);

            if (_intervalCheckCounter >= SeverityUpdateInterval)
            {
                _intervalCheckCounter = 0;
                
                var severityChangePerDay = SeverityChangePerDay();

                severityChangePerDay *= 0.0033f; //some magic number

                severityAdjustment += severityChangePerDay;
            }
            else
            {
                _intervalCheckCounter++;
            }
        }

        /// <summary>
        /// Severities the change per day
        /// </summary>
        /// <returns>The float</returns>
        public virtual float SeverityChangePerDay()
        {
            return Props.severityPerDay;
        }
    }
}
