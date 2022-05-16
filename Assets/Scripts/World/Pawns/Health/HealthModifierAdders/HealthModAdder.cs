using System;
using System.Collections.Generic;
using Assets.Scripts.World.Pawns.BodyPartTemplates;
using Utilities;
using World.Pawns.Health.HealthModifiers;

namespace World.Pawns.Health.HealthModifierAdders
{
    public class HealthModAdder
    {
        public HealthModTemplate healthModTemplate;

        public List<BodyPartTemplate> affectedParts;

        public bool affectsAnyLivePart;

        public int numPartsToAffect = 1;

        public virtual void OnIntervalPassed(Pawn pawn, HealthMod cause)
        {
        }

        public virtual bool OnHealthModAdded(Pawn pawn, HealthMod healthMod)
        {
            return false;
        }

        public bool TryApply(Pawn pawn, List<HealthMod> outAddedHealthMods = null)
        {
            return HealthModAdderUtils.TryApply(pawn, healthModTemplate, affectedParts, affectsAnyLivePart,
                numPartsToAffect, outAddedHealthMods);
        }

        protected void SendAlert()
        {
            throw new NotImplementedException();
        }
    }
}
