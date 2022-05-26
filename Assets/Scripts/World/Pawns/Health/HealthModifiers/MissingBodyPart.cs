using Sirenix.Utilities;
using UnityEngine;
using Utilities;

namespace World.Pawns.Health.HealthModifiers
{
    public class MissingBodyPart : HealthMod
    {
        public override float SummaryHealthPercentImpact
        {
            get
            {
                if (Part.template.tags.IsNullOrEmpty() && Part.GetAllChildren().IsNullOrEmpty())
                {
                    return 0f;
                }

                if (!Bleeding)
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

        public bool ParentMissing => pawn.health.BodyPartIsMissing(Part.parent);

        public override void PostAdd()
        {
            if (!part.HasChildParts())
            {
                return;
            }

            foreach (var child in part.GetAllChildren())
            {
                var missingPartMod = HealthModMaker.MakeHealthMod(template, pawn, child);

                pawn.health.AddHealthMod(missingPartMod);
            }
        }

        //todo is fresh

        //todo tendable now
    }
}
