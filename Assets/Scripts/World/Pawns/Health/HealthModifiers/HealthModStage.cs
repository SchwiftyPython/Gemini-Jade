using System.Collections.Generic;
using World.Pawns.Health.HealthModifierAdders;

namespace World.Pawns.Health.HealthModifiers
{
    /// <summary>
    /// The health mod stage class
    /// </summary>
    public class HealthModStage
    {
        /// <summary>
        /// The min severity
        /// </summary>
        public float minSeverity;

        /// <summary>
        /// The label
        /// </summary>
        public string label;

        /// <summary>
        /// The visible
        /// </summary>
        public bool visible = true;

        /// <summary>
        /// The life threatening
        /// </summary>
        public bool lifeThreatening;

        /// <summary>
        /// The vomit chance
        /// </summary>
        public float vomitChance = 0f;

        /// <summary>
        /// The sudden death chance
        /// </summary>
        public float suddenDeathChance = 0f;

        /// <summary>
        /// The pain modifier
        /// </summary>
        public float painModifier = 1f;

        /// <summary>
        /// The pain offset
        /// </summary>
        public float painOffset;

        /// <summary>
        /// The bleed modifier
        /// </summary>
        public float bleedModifier = 1f;

        /// <summary>
        /// The natural healing modifier
        /// </summary>
        public float naturalHealingModifier = -1f;

        /// <summary>
        /// The opinion of others modifier
        /// </summary>
        public float opinionOfOthersModifier = 1f;

        /// <summary>
        /// The hunger rate modifier
        /// </summary>
        public float hungerRateModifier = 1f;

        /// <summary>
        /// The hunger rate modifier offset
        /// </summary>
        public float hungerRateModifierOffset;

        /// <summary>
        /// The rest modifier
        /// </summary>
        public float restModifier = 1f;

        /// <summary>
        /// The rest modifier offset
        /// </summary>
        public float restModifierOffset;

        /// <summary>
        /// The brawl chance
        /// </summary>
        public float brawlChance = 1f;

        /// <summary>
        /// The food poisoning chance
        /// </summary>
        public float foodPoisoningChance = 1f;

        /// <summary>
        /// The mental break chance
        /// </summary>
        public float mentalBreakChance = 0f;
    
        //todo MentalBreakIntensities

        /// <summary>
        /// The immunities
        /// </summary>
        public List<HealthModTemplate> immunities;
    
        //todo health function modifiers

        /// <summary>
        /// The health mod adders
        /// </summary>
        public List<HealthModAdder> healthModAdders;
    
        //todo mental state givers
    
        //todo stat offsets
    
        //todo stat defs
    
        //todo disabled work tags

        /// <summary>
        /// The part efficiency offset
        /// </summary>
        public float partEfficiencyOffset;

        /// <summary>
        /// The part ignore missing hp
        /// </summary>
        public bool partIgnoreMissingHp;

        /// <summary>
        /// The destroy part
        /// </summary>
        public bool destroyPart;

        //todo affects memory

        /// <summary>
        /// Gets the value of the affects social interactions
        /// </summary>
        public bool AffectsSocialInteractions => opinionOfOthersModifier != 1f;

    }
}
