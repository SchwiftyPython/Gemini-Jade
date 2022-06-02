using World.Pawns.Health.HealthModifierComponents.HealthModCompProperties;
using World.Pawns.Health.HealthModifiers;
using World.Things;

namespace World.Pawns.Health.HealthModifierComponents
{
    public class HealthModComp
    {
        public HealthMod parent;

        public HealthModCompProps props;

        public Pawn Pawn => parent.pawn;

        public HealthModTemplate HealthModTemplate => parent.template;
        
        //todo some label and tool tip stuff

        public virtual bool ShouldRemove => false;

        public virtual void PostMake()
        {
        }

        public virtual void PostTick(ref float severityAdjustment)
        {
        }

        public virtual void PostAdd() //todo takes DamageInfo? as arg
        {
        }

        public virtual void PostRemove()
        {
        }

        public virtual void PostMerge(HealthMod otherHealthMod)
        {
        }

        public virtual bool DisallowVisible()
        {
            return false;
        }

        public virtual void ModifyChemicalEffect() //todo chemical template and effect amount
        {
        }

        public virtual void PostInjuryHeal()
        {
        }

        public virtual void Tend(float quality, float maxQuality)
        {
        }
        
        public virtual void Alert_ImplantUsed(string violationSourceName, float detectionChance, int violationSourceLevel = -1)
        {
        }

        public virtual void Alert_EntropyGained(float baseAmount, float finalAmount, Thing source = null)
        {
        }

        public virtual void Alert_PawnUsedVerb() //todo args Verb verb, LocalTargetInfo target
        {
        }

        public virtual void Alert_PawnDied()
        {
        }

        public virtual void Alert_PawnKilled()
        {
        }

        public virtual void Alert_PawnPostApplyDamage() //todo args DamageInfo dinfo, float totalDamageDealt
        {
        }

        public virtual string DebugString()
        {
            return null;
        }
    }
}
