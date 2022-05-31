using System;
using UnityEngine;
using World.Pawns.Health.HealthModifiers;

namespace World.Pawns.Health.HealthModifierComponents.HealthModCompProperties
{
    public class HealthModCompProps : MonoBehaviour
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
