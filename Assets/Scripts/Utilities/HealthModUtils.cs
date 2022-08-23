using World.Pawns.Health.HealthModifierComponents;
using World.Pawns.Health.HealthModifiers;

namespace Utilities
{
    /// <summary>
    /// The health mod utils class
    /// </summary>
    public static class HealthModUtils
    {
        /// <summary>
        /// Gets the comp using the specified health mod
        /// </summary>
        /// <typeparam name="T">The </typeparam>
        /// <param name="healthMod">The health mod</param>
        /// <returns>The</returns>
        public static T GetComp<T>(this HealthMod healthMod) where T : HealthModComp
        {
            if (!healthMod.HasComps)
            {
                return null;
            }

            foreach (var comp in healthMod.comps)
            {
                if (comp is T matchingComp)
                {
                    return matchingComp;
                }
            }

            return null;
        }

        /// <summary>
        /// Describes whether is tended
        /// </summary>
        /// <param name="healthMod">The health mod</param>
        /// <returns>The bool</returns>
        public static bool IsTended(this HealthMod healthMod)
        {
            return healthMod.GetComp<TendDuration>()?.IsTended ?? false;
        }
        
        /// <summary>
        /// Describes whether fully immune
        /// </summary>
        /// <param name="healthMod">The health mod</param>
        /// <returns>The bool</returns>
        public static bool FullyImmune(this HealthMod healthMod)
        {
            //todo check Immune component

            return false;
        }

        /// <summary>
        /// Describes whether is permanent
        /// </summary>
        /// <param name="healthMod">The health mod</param>
        /// <returns>The bool</returns>
        public static bool IsPermanent(this HealthMod healthMod)
        {
            //todo check Gets Permanent component

            return false;
        }
        
        /// <summary>
        /// Describes whether can heal from tending
        /// </summary>
        /// <param name="healthMod">The health mod</param>
        /// <returns>The bool</returns>
        public static bool CanHealFromTending(this HealthMod healthMod)
        {
            if (healthMod.IsTended())
            {
                return !healthMod.IsPermanent();
            }
            return false;
        }
        
        /// <summary>
        /// Describes whether can heal naturally
        /// </summary>
        /// <param name="healthMod">The health mod</param>
        /// <returns>The bool</returns>
        public static bool CanHealNaturally(this HealthMod healthMod)
        {
            return !healthMod.IsPermanent();
        }
    }
}
