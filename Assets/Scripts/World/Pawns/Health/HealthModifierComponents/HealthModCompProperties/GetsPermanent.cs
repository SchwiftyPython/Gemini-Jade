namespace World.Pawns.Health.HealthModifierComponents.HealthModCompProperties
{
    /// <summary>
    /// The gets permanent class
    /// </summary>
    /// <seealso cref="HealthModCompProps"/>
    public class GetsPermanent : HealthModCompProps
    {
        /// <summary>
        /// The permanent chance modifier
        /// </summary>
        public float permanentChanceModifier = 1f;

        /// <summary>
        /// The permanent label
        /// </summary>
        public string permanentLabel;

        /// <summary>
        /// The instantly permanent label
        /// </summary>
        public string instantlyPermanentLabel;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetsPermanent"/> class
        /// </summary>
        public GetsPermanent()
        {
            compClass = typeof(HealthModifierComponents.GetsPermanent);
        }
    }
}