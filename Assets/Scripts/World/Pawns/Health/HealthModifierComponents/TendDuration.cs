using System.Text;
using Assets.Scripts.Utilities;
using UnityEngine;

namespace World.Pawns.Health.HealthModifierComponents
{
    public class TendDuration : SeverityPerDay
    {
        public const float TendQualityVariance = 0.25f;
        
        private float _totalTendQuality;
        
        public int tendTicksLeft = -1;

        public float tendQuality;
        
        //todo icons and colors for tend quality

        private HealthModCompProperties.TendDuration Props => (HealthModCompProperties.TendDuration) props;
        
        public bool IsTended
        {
            get
            {
                //todo check if game is playing
                
                return tendTicksLeft > 0;
            }
        }

        public bool AllowTend
        {
            get
            {
                if (Props.TendIsPermanent)
                {
                    return !IsTended;
                }
                
                return true;
            }
        }

        public override bool ShouldRemove
        {
            get
            {
                if (base.ShouldRemove)
                {
                    return true;
                }

                if (Props.disappearsAtTotalTendQuality >= 0)
                {
                    return _totalTendQuality >= Props.disappearsAtTotalTendQuality;
                }

                return false;
            }
        }

        //todo tooltip override
        
        //todo comp state icon override

        public override float SeverityChangePerDay()
        {
            if (IsTended)
            {
                return Props.severityPerDayTended * tendQuality;
            }

            return 0f;
        }

        public override void PostTick(ref float severityAdjustment)
        {
            base.PostTick(ref severityAdjustment);

            if (tendTicksLeft > 0 && Props.TendIsPermanent) 
            {
                tendTicksLeft--;
            }
        }
        
        public override void Tend(float quality, float maxQuality)
        {
            tendQuality = Mathf.Clamp(quality + Random.Range(-0.25f, 0.25f), 0f, maxQuality);
            _totalTendQuality += tendQuality;
            if (Props.TendIsPermanent)
            {
                tendTicksLeft = 1;
            }
            else
            {
                tendTicksLeft = Mathf.Max(0, tendTicksLeft) + Props.TendTicksFull;
            }
            
            //todo floating tend quality indicator

            base.Pawn.health.CheckForHealthStateChange(parent);
        }

        public override string DebugString()
        {
            var stringBuilder = new StringBuilder();
            if (IsTended)
            {
                stringBuilder.AppendLine("tendQuality: " + tendQuality.ToStringPercent());
                if (!Props.TendIsPermanent)
                {
                    stringBuilder.AppendLine("tendTicksLeft: " + tendTicksLeft);
                }
            }
            else
            {
                stringBuilder.AppendLine("untended");
            }

            stringBuilder.AppendLine("severity/day: " + SeverityChangePerDay());
            if (Props.disappearsAtTotalTendQuality >= 0)
            {
                stringBuilder.AppendLine("totalTendQuality: " + _totalTendQuality.ToString("F2") + " / " +
                                         Props.disappearsAtTotalTendQuality);
            }

            return stringBuilder.ToString().Trim();
        }
    }
}
