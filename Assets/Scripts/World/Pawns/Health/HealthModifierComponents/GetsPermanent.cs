using UnityEngine;
using Utilities;

namespace World.Pawns.Health.HealthModifierComponents
{
    public class GetsPermanent : HealthModComp
    {
        private const float DefaultPermDamageThreshold = 9999f;

        private PainLevel _painLevel;
        
        public float permDamageThreshold = DefaultPermDamageThreshold;

        public bool isPermanent;

        public HealthModCompProperties.GetsPermanent Props => (HealthModCompProperties.GetsPermanent) props;

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

        public PainLevel PainLevel => _painLevel;

        public float PainModifier => (float) _painLevel;

        public void SetPainLevel(PainLevel level)
        {
            _painLevel = level;

            Pawn?.health.CheckForHealthStateChange(parent);
        }

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
        
        public override string DebugString()
        {
            return $"IsPermanent: {IsPermanent}\npermDamageThreshold: {permDamageThreshold}\npainLevel: {PainLevel}";
        }
    }
}
