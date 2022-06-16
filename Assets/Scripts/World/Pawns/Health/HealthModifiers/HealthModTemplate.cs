using System;
using System.Collections.Generic;
using Assets.Scripts.World;
using UnityEngine;
using World.Pawns.Health.HealthModifierAdders;
using World.Pawns.Health.HealthModifierComponents.HealthModCompProperties;

namespace World.Pawns.Health.HealthModifiers
{
    [CreateAssetMenu(menuName = "Templates/HealthModTemplate")]
    public class HealthModTemplate : Template
    {
        public Type healthModClass;
        
        public List<HealthModCompProps> comps;

        public float initialSeverity = 0.001f;

        public float lethalSeverity = -1f;

        public List<HealthModStage> stages;

        public bool tendable;

        public float chanceToCauseNoPain;

        public float minSeverity;

        public float maxSeverity = float.MaxValue;

        public float painPerSeverity = 1f;
        
        public float averagePainPerSeverityPermanent = 0.5f;

        public float bleedRate = 1f;
        
        public bool canMerge;
        
        public string destroyedLabel;

        public string destroyedOutLabel;

        public bool useRemovedLabel;

        public List<HealthModAdder> healthModAdders;
        
        public bool makesAlert = true;
        
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
