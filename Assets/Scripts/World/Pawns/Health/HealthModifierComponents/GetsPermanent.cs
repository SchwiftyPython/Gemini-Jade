using UnityEngine;
using Utilities;

namespace World.Pawns.Health.HealthModifierComponents
{
    /// <summary>
    /// The gets permanent class
    /// </summary>
    /// <seealso cref="HealthModComp"/>
    public class GetsPermanent : HealthModComp
    {
        /// <summary>
        /// The default perm damage threshold
        /// </summary>
        private const float DefaultPermDamageThreshold = 9999f;

        /// <summary>
        /// The pain level
        /// </summary>
        private PainLevel _painLevel;
        
        /// <summary>
        /// The default perm damage threshold
        /// </summary>
        public float permDamageThreshold = DefaultPermDamageThreshold;

        /// <summary>
        /// The is permanent
        /// </summary>
        public bool isPermanent;

        /// <summary>
        /// Gets the value of the props
        /// </summary>
        public HealthModCompProperties.GetsPermanent Props => (HealthModCompProperties.GetsPermanent) props;

        /// <summary>
        /// Gets or sets the value of the is permanent
        /// </summary>
        public bool IsPermanent
        {
            get => isPermanent;
            set
            {
                if (value == isPermanent)
                {
                    return;
                }

                isPermanent = value;

                if (!isPermanent)
                {
                    return;
                }

                permDamageThreshold = DefaultPermDamageThreshold;

                SetPainLevel(HealthAdjustments.InjuryPainLevels.RandomElementByWeight(painLevel => painLevel.weight)
                    .level);
            }
        }

        /// <summary>
        /// Gets the value of the pain level
        /// </summary>
        public PainLevel PainLevel => _painLevel;

        /// <summary>
        /// Gets the value of the pain modifier
        /// </summary>
        public float PainModifier => (float) _painLevel;

        /// <summary>
        /// Sets the pain level using the specified level
        /// </summary>
        /// <param name="level">The level</param>
        public void SetPainLevel(PainLevel level)
        {
            _painLevel = level;

            Pawn?.health.CheckForHealthStateChange(parent);
        }

        /// <summary>
        /// Pres the finalize injury
        /// </summary>
        public void PreFinalizeInjury()
        {
            //todo check for added parts

            var becomePermChance =
                0.02f * parent.Part.template.permanentInjuryChanceFactor * Props.permanentChanceModifier; //magic number

            if (!parent.Part.template.delicate)
            {
                var healthAdjustments = Object.FindObjectOfType<HealthAdjustments>();

                becomePermChance *=
                    healthAdjustments.BecomePermanentChanceFactorBySeverityCurve.Evaluate(parent.Severity);
            }

            if (Random.Range(0f, 1f) < becomePermChance)
            {
                if (parent.Part.template.delicate)
                {
                    IsPermanent = true;
                }
                else
                {
                    permDamageThreshold = Random.Range(1f, parent.Severity / 2f);
                }
            }
        }

        /// <summary>
        /// Posts the injury heal using the specified amount
        /// </summary>
        /// <param name="amount">The amount</param>
        public override void PostInjuryHeal(float amount)
        {
            if (permDamageThreshold >= DefaultPermDamageThreshold)
            {
                return;
            }

            if (IsPermanent)
            {
                return;
            }

            if (!(parent.Severity <= permDamageThreshold))
            {
                return;
            }

            if (parent.Severity >= permDamageThreshold - amount)
            {
                parent.Severity = permDamageThreshold;

                IsPermanent = true;
                
                Pawn.health.CheckForHealthStateChange(parent);
            }
        }
        
        /// <summary>
        /// Debugs the string
        /// </summary>
        /// <returns>The string</returns>
        public override string DebugString()
        {
            return $"IsPermanent: {IsPermanent}\npermDamageThreshold: {permDamageThreshold}\npainLevel: {PainLevel}";
        }
    }
}
