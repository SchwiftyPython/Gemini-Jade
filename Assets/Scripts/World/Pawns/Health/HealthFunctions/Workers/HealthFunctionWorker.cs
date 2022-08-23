using System.Collections.Generic;
using Assets.Scripts.World.Pawns.BodyTemplates;
using World.Pawns.Health.HealthModifiers;

namespace World.Pawns.Health.HealthFunctions.Workers
{
    /// <summary>
    /// The health function worker class
    /// </summary>
    public class HealthFunctionWorker
    {
        /// <summary>
        /// Calculates the function level using the specified pawn
        /// </summary>
        /// <param name="pawn">The pawn</param>
        /// <param name="healthMods">The health mods</param>
        /// <returns>The float</returns>
        public virtual float CalculateFunctionLevel(Pawn pawn, List<HealthMod> healthMods)
        {
            return 1f;
        }

        /// <summary>
        /// Describes whether this instance can have function
        /// </summary>
        /// <param name="body">The body</param>
        /// <returns>The bool</returns>
        public virtual bool CanHaveFunction(BodyTemplate body)
        {
            return true;
        }
        
        /// <summary>
        /// Calculates the function using the specified pawn
        /// </summary>
        /// <param name="pawn">The pawn</param>
        /// <param name="function">The function</param>
        /// <returns>The float</returns>
        protected static float CalculateFunction(Pawn pawn, HealthFunctionTemplate function)
        {
            return pawn.health.GetLevel(function);
        }
    }
}
