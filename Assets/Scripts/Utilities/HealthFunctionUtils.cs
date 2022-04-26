using System.Collections.Generic;
using World.Pawns;
using World.Pawns.Health.HealthFunctions;
using World.Pawns.Health.HealthModifiers;

namespace Utilities
{
    public static class HealthFunctionUtils
    {
        public static float CalculateFunctionLevel(Pawn pawn, List<HealthMod> healthMods, HealthFunctionTemplate function)
        {
            if (function.zeroIfCannotWakeUp && !pawn.CanWakeUp)
            {
                return 0f;
            }

            float level = function.Worker.CalculateFunctionLevel(healthMods);

            if (level > 0f)
            {
                foreach (var healthMod in healthMods)
                {
                    //todo get function mods and loop through them
                    
                    //a lot of voodoo going on in this loop. See what we end up needing
                    //will probably make more sense once we start seeing the numbers
                }
            }

            return level;
        }
    }
}
