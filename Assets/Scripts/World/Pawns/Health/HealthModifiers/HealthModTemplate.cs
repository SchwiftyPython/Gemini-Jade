using System;
using System.Collections.Generic;
using Assets.Scripts.World;
using UnityEngine;
using World.Pawns.Health.HealthModifierAdders;
using World.Pawns.Health.HealthModifierComponents.HealthModCompProperties;

namespace World.Pawns.Health.HealthModifiers
{
    /// <summary>
    /// The health mod template class
    /// </summary>
    /// <seealso cref="Template"/>
    [CreateAssetMenu(menuName = "Templates/HealthModTemplate")]
    public class HealthModTemplate : Template
    {
        /// <summary>
        /// The health mod
        /// </summary>
        public Type healthModClass;
        
        /// <summary>
        /// The comps
        /// </summary>
        public List<HealthModCompProps> comps;

        /// <summary>
        /// The initial severity
        /// </summary>
        public float initialSeverity = 0.001f;

        /// <summary>
        /// The lethal severity
        /// </summary>
        public float lethalSeverity = -1f;

        /// <summary>
        /// The stages
        /// </summary>
        public List<HealthModStage> stages;

        /// <summary>
        /// The tendable
        /// </summary>
        public bool tendable;

        /// <summary>
        /// The chance to cause no pain
        /// </summary>
        public float chanceToCauseNoPain;

        /// <summary>
        /// The min severity
        /// </summary>
        public float minSeverity;

        /// <summary>
        /// The max value
        /// </summary>
        public float maxSeverity = float.MaxValue;

        /// <summary>
        /// The pain per severity
        /// </summary>
        public float painPerSeverity = 1f;
        
        /// <summary>
        /// The average pain per severity permanent
        /// </summary>
        public float averagePainPerSeverityPermanent = 0.5f;

        /// <summary>
        /// The bleed rate
        /// </summary>
        public float bleedRate = 1f;
        
        /// <summary>
        /// The can merge
        /// </summary>
        public bool canMerge;
        
        /// <summary>
        /// The destroyed label
        /// </summary>
        public string destroyedLabel;

        /// <summary>
        /// The destroyed out label
        /// </summary>
        public string destroyedOutLabel;

        /// <summary>
        /// The use removed label
        /// </summary>
        public bool useRemovedLabel;

        /// <summary>
        /// The health mod adders
        /// </summary>
        public List<HealthModAdder> healthModAdders;
        
        /// <summary>
        /// The makes alert
        /// </summary>
        public bool makesAlert = true;
        
        /// <summary>
        /// Describes whether this instance has comp
        /// </summary>
        /// <param name="compClass">The comp</param>
        /// <returns>The bool</returns>
        public bool HasComp(Type compClass)
        {
            if (comps == null)
            {
                return false;
            }

            foreach (var comp in comps)
            {
                if (comp.compClass == compClass)
                {
                    return true;
                }
            }
            
            return false;
        }
        
        /// <summary>
        /// Gets the comp props for using the specified comp class
        /// </summary>
        /// <param name="compClass">The comp</param>
        /// <returns>The health mod comp props</returns>
        public HealthModCompProps GetCompPropsFor(Type compClass)
        {
            if (comps == null)
            {
                return null;
            }

            foreach (var comp in comps)
            {
                if (comp.compClass == compClass)
                {
                    return comp;
                }
            }
            
            return null;
        }
        
        /// <summary>
        /// Comps the props
        /// </summary>
        /// <typeparam name="T">The </typeparam>
        /// <returns>The</returns>
        public T CompProps<T>() where T : HealthModCompProps
        {
            if (comps == null)
            {
                return null;
            }

            foreach (var comp in comps)
            {
                if (comp is T result)
                {
                    return result;
                }
            }

            return null;
        }
        
        /// <summary>
        /// Describes whether this instance can develop immunity naturally
        /// </summary>
        /// <returns>The bool</returns>
        public bool CanDevelopImmunityNaturally()
        {
            // todo HediffCompProperties_Immunizable hediffCompProperties_Immunizable = CompProps<HediffCompProperties_Immunizable>();
            // if (hediffCompProperties_Immunizable != null && (hediffCompProperties_Immunizable.immunityPerDayNotSick > 0f || hediffCompProperties_Immunizable.immunityPerDaySick > 0f))
            // {
            //     return true;
            // }
            return false;
        }
    }
}
