using World.Pawns.Health.HealthModifierComponents;
using World.Pawns.Health.HealthModifiers;

namespace Utilities
{
    public static class HealthModUtils
    {
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

        public static bool IsTended(this HealthMod healthMod)
        {
            return healthMod.GetComp<TendDuration>()?.IsTended ?? false;
        }
        
        public static bool FullyImmune(this HealthMod healthMod)
        {
            //todo check Immune component

            return false;
        }

        public static bool IsPermanent(this HealthMod healthMod)
        {
            //todo check Gets Permanent component

            return false;
        }
        
        public static bool CanHealFromTending(this HealthMod healthMod)
        {
            if (healthMod.IsTended())
            {
                return !healthMod.IsPermanent();
            }
            return false;
        }
        
        public static bool CanHealNaturally(this HealthMod healthMod)
        {
            return !healthMod.IsPermanent();
        }
    }
}
