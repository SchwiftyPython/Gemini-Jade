using System;
using System.Collections.Generic;
using Assets.Scripts.World.Pawns.BodyPartTemplates;
using Utilities;
using World.Pawns.Health.HealthModifiers;

namespace World.Pawns.Health.HealthModifierAdders
{
    /// <summary>
    /// The health mod adder class
    /// </summary>
    public class HealthModAdder
    {
        /// <summary>
        /// The health mod template
        /// </summary>
        public HealthModTemplate healthModTemplate;

        /// <summary>
        /// The affected parts
        /// </summary>
        public List<BodyPartTemplate> affectedParts;

        /// <summary>
        /// The affects any live part
        /// </summary>
        public bool affectsAnyLivePart;

        /// <summary>
        /// The num parts to affect
        /// </summary>
        public int numPartsToAffect = 1;

        /// <summary>
        /// Ons the interval passed using the specified pawn
        /// </summary>
        /// <param name="pawn">The pawn</param>
        /// <param name="cause">The cause</param>
        public virtual void OnIntervalPassed(Pawn pawn, HealthMod cause)
        {
        }

        /// <summary>
        /// Describes whether this instance on health mod added
        /// </summary>
        /// <param name="pawn">The pawn</param>
        /// <param name="healthMod">The health mod</param>
        /// <returns>The bool</returns>
        public virtual bool OnHealthModAdded(Pawn pawn, HealthMod healthMod)
        {
            return false;
        }

        /// <summary>
        /// Describes whether this instance try apply
        /// </summary>
        /// <param name="pawn">The pawn</param>
        /// <param name="outAddedHealthMods">The out added health mods</param>
        /// <returns>The bool</returns>
        public bool TryApply(Pawn pawn, List<HealthMod> outAddedHealthMods = null)
        {
            return HealthModAdderUtils.TryApply(pawn, healthModTemplate, affectedParts, affectsAnyLivePart,
                numPartsToAffect, outAddedHealthMods);
        }

        /// <summary>
        /// Sends the alert
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        protected void SendAlert()
        {
            throw new NotImplementedException();
        }
    }
}
