using System;
using World.Pawns.Health.HealthModifiers;

namespace World.Pawns.Health.HealthModifierComponents.HealthModCompProperties
{
    public class HealthModCompProps
    {
        public Type compClass;

        public virtual void PostLoad()
        {
        }
        
        public virtual void ResolveReferences(HealthModTemplate parent)
        {
        }
    }
}
