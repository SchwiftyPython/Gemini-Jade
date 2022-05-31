namespace World.Pawns.Health.HealthModifierComponents
{
    public class SeverityPerDay : HealthModComp
    {
        protected const int SeverityUpdateInterval = 200;
        
        private int _intervalCheckCounter = 0;

        private HealthModCompProperties.SeverityPerDay Props => (HealthModCompProperties.SeverityPerDay) props;
        
        //todo override label stuff

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

        public virtual float SeverityChangePerDay()
        {
            return Props.severityPerDay;
        }
    }
}
