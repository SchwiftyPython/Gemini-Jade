using Assets.Scripts.Utilities;
using Sirenix.Utilities;
using UnityEngine;

namespace Assets.Scripts.World.Pawns.Health.HealthModifiers
{
    public class MissingBodyPart : HealthMod
    {
        public override float SummaryHealthPercentImpact
        {
            get
            {
                if (Part.template.tags.IsNullOrEmpty() && Part.GetAllChildren().IsNullOrEmpty() && !Bleeding)
                {
                    return 0f;
                }

                return Part.template.hitPoints / (75f * 1); //todo uses pawn health scale. 75 is some magic number.
            }
        }

        public override bool ShouldRemove => false;

        public override string LabelBase
        {
            get
            {
                var healthUtils = Object.FindObjectOfType<HealthUtils>();

                if (Part.depth == healthUtils.inside)
                {
                    return healthUtils.GetGeneralDestroyedPartLabel(Part, part.template.solid);
                }

                return "Missing"; //todo label from injury source. Like explosion would say "shredded", sword "cutoff", etc.
            }
        }

        public override float BleedRate
        {
            get
            {
                if (pawn.Dead || ParentMissing)
                {
                    return 0f;
                }

                return Part.template.GetMaxHealth(pawn) * template.bleedRate * Part.template.bleedRate;
            }
        }

        public override float PainOffset
        {
            get
            {
                if (pawn.Dead || painless || ParentMissing)
                {
                    return 0f;
                }

                return Part.template.GetMaxHealth(pawn) * template.painPerSeverity / 1f; //todo divide by health scale
            }
        }

        private bool ParentMissing => Part.parent.IsMissing();

        //todo is fresh

        //todo tendable now
    }
}
