using UnityEngine;

namespace World.Pawns.Health.HealthModifiers
{
    /// <summary>
    /// The injury class
    /// </summary>
    /// <seealso cref="HealthMod"/>
    public class Injury : HealthMod
    {
        //todo permanent injury color
        
        //todo override label base checking for permanent injury health mod comp
        
        //todo all the override label stuff

        /// <summary>
        /// Gets the value of the summary health percent impact
        /// </summary>
        public override float SummaryHealthPercentImpact
        {
            get
            {
                if (!visible) //todo or not permanent
                {
                    return 0f;
                }

                return Severity / 75f; //todo times pawn health scale
            }
        }

        /// <summary>
        /// Gets the value of the pain offset
        /// </summary>
        public override float PainOffset
        {
            get
            {
                if (pawn.Dead || painless) //todo or has added parts
                {
                    return 0f;
                }
                
                //todo fetch get permanent component

                var painOffset = Severity * template.painPerSeverity; //todo if perm than use the perm pain numbers

                return painOffset; //todo divided by pawn health scale
            }
        }

        /// <summary>
        /// Gets the value of the bleed rate
        /// </summary>
        public override float BleedRate
        {
            get
            {
                if (pawn.Dead)
                {
                    return 0f;
                }

                if (BleedingStoppedDueToDuration)
                {
                    return 0f;
                }

                if (Part != null && Part.template.solid) //todo or is tended or is perm
                {
                    return 0f;
                }
                
                //todo if any added parts

                var bleedRate = Severity * template.bleedRate;

                if (Part != null)
                {
                    bleedRate *= Part.template.bleedRate;
                }

                return bleedRate;
            }
        }

        /// <summary>
        /// Gets the value of the duration ticks to stop bleeding
        /// </summary>
        private int DurationTicksToStopBleeding
        {
            get
            {
                //magic numbers

                var ticks = Mathf.Clamp(Mathf.InverseLerp(1f, 30f, Severity), 0f, 1f);

                return 90000 + Mathf.RoundToInt(Mathf.Lerp(0f, 90000f, ticks));
            }
        }

        /// <summary>
        /// Gets the value of the bleeding stopped due to duration
        /// </summary>
        private bool BleedingStoppedDueToDuration => durationTicks >= DurationTicksToStopBleeding;

        /// <summary>
        /// Ticks this instance
        /// </summary>
        public override void Tick()
        {
            var bleedingStoppedDueToDurationPre = BleedingStoppedDueToDuration;

            base.Tick();

            var bleedingStoppedDueToDurationPost = BleedingStoppedDueToDuration;

            if (bleedingStoppedDueToDurationPre != bleedingStoppedDueToDurationPost)
            {
                Debug.Log($"Bleeding stopped due to duration. Duration Ticks: {durationTicks}");
                
                pawn.health.CheckForHealthStateChange(this);
            }
        }
        
        //todo override heal

        /// <summary>
        /// Describes whether this instance try merge with
        /// </summary>
        /// <param name="otherHealthMod">The other health mod</param>
        /// <returns>The bool</returns>
        public override bool TryMergeWith(HealthMod otherHealthMod)
        {
            if (otherHealthMod is not Injury otherInjury) 
            {
                return false;
            }

            if (otherInjury.template != template) 
            {
                return false;
            }

            if (otherInjury.Part != Part) //todo or is tended or perm
            {
                return false;
            }

            if (!template.canMerge) 
            {
                return false;
            }

            return base.TryMergeWith(otherHealthMod);
        }

        /// <summary>
        /// Posts the remove
        /// </summary>
        public override void PostRemove()
        {
            base.PostRemove();
            
            //todo removing injuries on sprites if applicable
        }
    }
}
