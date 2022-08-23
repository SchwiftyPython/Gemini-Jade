using World.Pawns.Health.HealthModifierComponents.HealthModCompProperties;
using World.Pawns.Health.HealthModifiers;
using World.Things;

namespace World.Pawns.Health.HealthModifierComponents
{
    /// <summary>
    /// The health mod comp class
    /// </summary>
    public class HealthModComp
    {
        /// <summary>
        /// The parent
        /// </summary>
        public HealthMod parent;

        /// <summary>
        /// The props
        /// </summary>
        public HealthModCompProps props;

        /// <summary>
        /// Gets the value of the pawn
        /// </summary>
        public Pawn Pawn => parent.pawn;

        /// <summary>
        /// Gets the value of the health mod template
        /// </summary>
        public HealthModTemplate HealthModTemplate => parent.template;
        
        //todo some label and tool tip stuff

        /// <summary>
        /// Gets the value of the should remove
        /// </summary>
        public virtual bool ShouldRemove => false;

        /// <summary>
        /// Posts the make
        /// </summary>
        public virtual void PostMake()
        {
        }

        /// <summary>
        /// Posts the tick using the specified severity adjustment
        /// </summary>
        /// <param name="severityAdjustment">The severity adjustment</param>
        public virtual void PostTick(ref float severityAdjustment)
        {
        }

        /// <summary>
        /// Posts the add
        /// </summary>
        public virtual void PostAdd() //todo takes DamageInfo? as arg
        {
        }

        /// <summary>
        /// Posts the remove
        /// </summary>
        public virtual void PostRemove()
        {
        }

        /// <summary>
        /// Posts the merge using the specified other health mod
        /// </summary>
        /// <param name="otherHealthMod">The other health mod</param>
        public virtual void PostMerge(HealthMod otherHealthMod)
        {
        }

        /// <summary>
        /// Describes whether this instance disallow visible
        /// </summary>
        /// <returns>The bool</returns>
        public virtual bool DisallowVisible()
        {
            return false;
        }

        /// <summary>
        /// Modifies the chemical effect
        /// </summary>
        public virtual void ModifyChemicalEffect() //todo chemical template and effect amount
        {
        }

        /// <summary>
        /// Posts the injury heal using the specified amount
        /// </summary>
        /// <param name="amount">The amount</param>
        public virtual void PostInjuryHeal(float amount)
        {
        }

        /// <summary>
        /// Tends the quality
        /// </summary>
        /// <param name="quality">The quality</param>
        /// <param name="maxQuality">The max quality</param>
        public virtual void Tend(float quality, float maxQuality)
        {
        }
        
        /// <summary>
        /// Alerts the implant used using the specified violation source name
        /// </summary>
        /// <param name="violationSourceName">The violation source name</param>
        /// <param name="detectionChance">The detection chance</param>
        /// <param name="violationSourceLevel">The violation source level</param>
        public virtual void Alert_ImplantUsed(string violationSourceName, float detectionChance, int violationSourceLevel = -1)
        {
        }

        /// <summary>
        /// Alerts the entropy gained using the specified base amount
        /// </summary>
        /// <param name="baseAmount">The base amount</param>
        /// <param name="finalAmount">The final amount</param>
        /// <param name="source">The source</param>
        public virtual void Alert_EntropyGained(float baseAmount, float finalAmount, Thing source = null)
        {
        }

        /// <summary>
        /// Alerts the pawn used verb
        /// </summary>
        public virtual void Alert_PawnUsedVerb() //todo args Verb verb, LocalTargetInfo target
        {
        }

        /// <summary>
        /// Alerts the pawn died
        /// </summary>
        public virtual void Alert_PawnDied()
        {
        }

        /// <summary>
        /// Alerts the pawn killed
        /// </summary>
        public virtual void Alert_PawnKilled()
        {
        }

        /// <summary>
        /// Alerts the pawn post apply damage
        /// </summary>
        public virtual void Alert_PawnPostApplyDamage() //todo args DamageInfo dinfo, float totalDamageDealt
        {
        }

        /// <summary>
        /// Debugs the string
        /// </summary>
        /// <returns>The string</returns>
        public virtual string DebugString()
        {
            return null;
        }
    }
}
