using System;
using World.Pawns.Health.HealthModifiers;

namespace World.Pawns.Health.HealthModifierComponents.HealthModCompProperties
{
    /// <summary>
    /// The health mod comp props class
    /// </summary>
    public class HealthModCompProps
    {
        /// <summary>
        /// The comp
        /// </summary>
        public Type compClass;

        /// <summary>
        /// Posts the load
        /// </summary>
        public virtual void PostLoad()
        {
        }
        
        /// <summary>
        /// Resolves the references using the specified parent
        /// </summary>
        /// <param name="parent">The parent</param>
        public virtual void ResolveReferences(HealthModTemplate parent)
        {
        }
    }
}
