using System.Collections.Generic;

namespace World.Pawns.Health.HealthModifiers
{
    public class HealthModStage
    {
        public float minSeverity;

        public string label;

        public bool visible = true;

        public bool lifeThreatening;

        public float vomitChance = 0f;

        public float suddenDeathChance = 0f;

        public float painModifier = 1f;

        public float painOffset;

        public float bleedModifier = 1f;

        public float naturalHealingModifier = -1f;

        public float opinionOfOthersModifier = 1f;

        public float hungerRateModifier = 1f;

        public float hungerRateModifierOffset;

        public float restModifier = 1f;

        public float restModifierOffset;

        public float brawlChance = 1f;

        public float foodPoisoningChance = 1f;

        public float mentalBreakChance = 0f;
    
        //todo MentalBreakIntensities

        public List<HealthModTemplate> immunities;
    
        //todo health function modifiers
    
        //todo health mod givers
    
        //todo mental state givers
    
        //todo stat offsets
    
        //todo stat defs
    
        //todo disabled work tags

        public float partEfficiencyOffset;

        public bool partIgnoreMissingHp;

        public bool destroyPart;

        //todo affects memory

        public bool AffectsSocialInteractions => opinionOfOthersModifier != 1f;

    }
}
