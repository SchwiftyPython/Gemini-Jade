using System.Text;
using Assets.Scripts.Utilities;
using UnityEngine;

namespace World.Pawns.Health.HealthModifierComponents
{
    /// <summary>
    /// The tend duration class
    /// </summary>
    /// <seealso cref="SeverityPerDay"/>
    public class TendDuration : SeverityPerDay
    {
        /// <summary>
        /// The tend quality variance
        /// </summary>
        public const float TendQualityVariance = 0.25f;
        
        /// <summary>
        /// The total tend quality
        /// </summary>
        private float _totalTendQuality;
        
        /// <summary>
        /// The tend ticks left
        /// </summary>
        public int tendTicksLeft = -1;

        /// <summary>
        /// The tend quality
        /// </summary>
        public float tendQuality;
        
        //todo icons and colors for tend quality

        /// <summary>
        /// Gets the value of the props
        /// </summary>
        private HealthModCompProperties.TendDuration Props => (HealthModCompProperties.TendDuration) props;
        
        /// <summary>
        /// Gets the value of the is tended
        /// </summary>
        public bool IsTended
        {
            get
            {
                //todo check if game is playing
                
                return tendTicksLeft > 0;
            }
        }

        /// <summary>
        /// Gets the value of the allow tend
        /// </summary>
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

        /// <summary>
        /// Gets the value of the should remove
        /// </summary>
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

        /// <summary>
        /// Severities the change per day
        /// </summary>
        /// <returns>The float</returns>
        public override float SeverityChangePerDay()
        {
            if (IsTended)
            {
                return Props.severityPerDayTended * tendQuality;
            }

            return 0f;
        }

        /// <summary>
        /// Posts the tick using the specified severity adjustment
        /// </summary>
        /// <param name="severityAdjustment">The severity adjustment</param>
        public override void PostTick(ref float severityAdjustment)
        {
            base.PostTick(ref severityAdjustment);

            if (tendTicksLeft > 0 && Props.TendIsPermanent) 
            {
                tendTicksLeft--;
            }
        }
        
        /// <summary>
        /// Tends the quality
        /// </summary>
        /// <param name="quality">The quality</param>
        /// <param name="maxQuality">The max quality</param>
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

            Pawn.health.CheckForHealthStateChange(parent);
        }

        /// <summary>
        /// Debugs the string
        /// </summary>
        /// <returns>The string</returns>
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
