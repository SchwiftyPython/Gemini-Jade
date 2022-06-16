namespace World.Pawns.Health.HealthModifierComponents.HealthModCompProperties
{
    public class GetsPermanent : HealthModCompProps
    {
        public float permanentChanceModifier = 1f;

        public string permanentLabel;

        public string instantlyPermanentLabel;

        public GetsPermanent()
        {
            compClass = typeof(HealthModifierComponents.GetsPermanent);
        }
    }
}