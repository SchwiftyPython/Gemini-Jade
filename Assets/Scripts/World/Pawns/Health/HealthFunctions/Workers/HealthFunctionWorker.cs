using System.Collections.Generic;
using Assets.Scripts.World.Pawns.BodyTemplates;
using World.Pawns.Health.HealthModifiers;

namespace World.Pawns.Health.HealthFunctions.Workers
{
    public class HealthFunctionWorker
    {
        public virtual float CalculateFunctionLevel(Pawn pawn, List<HealthMod> healthMods)
        {
            return 1f;
        }

        public virtual bool CanHaveFunction(BodyTemplate body)
        {
            return true;
        }
        
        protected float CalculateFunction(Pawn pawn, List<HealthMod> healthMods, HealthFunctionTemplate function)
        {
            return pawn.health.GetLevel(function);
        }
    }
}
