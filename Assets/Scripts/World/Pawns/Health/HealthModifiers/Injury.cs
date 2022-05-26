using UnityEngine;

namespace World.Pawns.Health.HealthModifiers
{
    public class Injury : HealthMod
    {
        //todo permanent injury color
        
        //todo override label base checking for permanent injury health mod comp
        
        //todo all the override label stuff

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

        private int DurationTicksToStopBleeding
        {
            get
            {
                //magic numbers

                var ticks = Mathf.Clamp(Mathf.InverseLerp(1f, 30f, Severity), 0f, 1f);

                return 90000 + Mathf.RoundToInt(Mathf.Lerp(0f, 90000f, ticks));
            }
        }

        private bool BleedingStoppedDueToDuration => durationTicks >= DurationTicksToStopBleeding;

        public override void Tick()
        {
            var bleedingStoppedDueToDurationPre = BleedingStoppedDueToDuration;

            base.Tick();

            var bleedingStoppedDueToDurationPost = BleedingStoppedDueToDuration;

            if (bleedingStoppedDueToDurationPre != bleedingStoppedDueToDurationPost)
            {
                pawn.health.CheckForHealthStateChange(this);
            }
        }
        
        //todo override heal

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

            if (otherInjury.Part != Part) //todo or is tended, perm, or can't be merged
            {
                return false;
            }

            return base.TryMergeWith(otherHealthMod);
        }

        public override void PostRemove()
        {
            base.PostRemove();
            
            //todo removing injuries on sprites if applicable
        }
    }
}
