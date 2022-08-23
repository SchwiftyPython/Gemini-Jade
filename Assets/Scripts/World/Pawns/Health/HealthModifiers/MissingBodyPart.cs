using Sirenix.Utilities;
using UnityEngine;
using Utilities;

namespace World.Pawns.Health.HealthModifiers
{
    /// <summary>
    /// The missing body part class
    /// </summary>
    /// <seealso cref="HealthMod"/>
    public class MissingBodyPart : HealthMod
    {
        /// <summary>
        /// Gets the value of the summary health percent impact
        /// </summary>
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

        /// <summary>
        /// Gets the value of the should remove
        /// </summary>
        public override bool ShouldRemove => false;

        /// <summary>
        /// Gets the value of the label base
        /// </summary>
        public override string LabelBase
        {
            get
            {
                var healthUtils = Object.FindObjectOfType<HealthUtils>();

                if (Part.depth == healthUtils.inside)
                {
                    return healthUtils.GetGeneralDestroyedPartLabel(Part, Part.template.solid);
                }

                return "Missing"; //todo label from injury source. Like explosion would say "shredded", sword "cutoff", etc.
            }
        }

        /// <summary>
        /// Gets the value of the bleed rate
        /// </summary>
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

        /// <summary>
        /// Gets the value of the pain offset
        /// </summary>
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

        /// <summary>
        /// Gets the value of the parent missing
        /// </summary>
        public bool ParentMissing => pawn.health.BodyPartIsMissing(Part.parent);

        /// <summary>
        /// Posts the add
        /// </summary>
        public override void PostAdd()
        {
            if (!Part.HasChildParts())
            {
                return;
            }

            foreach (var child in Part.GetAllChildren())
            {
                var missingPartMod = HealthModMaker.MakeHealthMod(template, pawn, child);

                pawn.health.AddHealthMod(missingPartMod);
            }
        }

        //todo is fresh

        //todo tendable now
    }
}
