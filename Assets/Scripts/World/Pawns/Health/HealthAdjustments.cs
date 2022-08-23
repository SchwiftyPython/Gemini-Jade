using UnityEngine;

namespace World.Pawns.Health
{
    /// <summary>
    /// The health adjustments class
    /// </summary>
    /// <seealso cref="MonoBehaviour"/>
    public class HealthAdjustments : MonoBehaviour
    {
        /// <summary>
        /// The pain level weighted
        /// </summary>
        public struct PainLevelWeighted
        {
            /// <summary>
            /// The level
            /// </summary>
            public PainLevel level;

            /// <summary>
            /// The weight
            /// </summary>
            public float weight;

            /// <summary>
            /// Initializes a new instance of the <see cref="PainLevelWeighted"/> class
            /// </summary>
            /// <param name="level">The level</param>
            /// <param name="weight">The weight</param>
            public PainLevelWeighted(PainLevel level, float weight)
            {
                this.level = level;
                this.weight = weight;
            }
        }

        /// <summary>
        /// The standard interval
        /// </summary>
        public const int StandardInterval = 60;

        /// <summary>
        /// The small pawn fragmented damage health scale threshold
        /// </summary>
        public const float SmallPawnFragmentedDamageHealthScaleThreshold = 0.5f;

        /// <summary>
        /// The small pawn fragmented damage minimum damage amount
        /// </summary>
        public const int SmallPawnFragmentedDamageMinimumDamageAmount = 10;

        /// <summary>
        /// The chance to additionally damage inner solid part
        /// </summary>
        public static float chanceToAdditionallyDamageInnerSolidPart = 0.2f;

        /// <summary>
        /// The min bleeding rate to bleed
        /// </summary>
        public const float MinBleedingRateToBleed = 0.1f;

        /// <summary>
        /// The bleed severity recovery per interval
        /// </summary>
        public const float BleedSeverityRecoveryPerInterval = 0.00033333333f;

        /// <summary>
        /// The blood filth drop chance factor standing
        /// </summary>
        public const float BloodFilthDropChanceFactorStanding = 0.004f;

        /// <summary>
        /// The blood filth drop chance factor laying
        /// </summary>
        public const float BloodFilthDropChanceFactorLaying = 0.0004f;

        /// <summary>
        /// The base ticks after injury to stop bleeding
        /// </summary>
        public const int BaseTicksAfterInjuryToStopBleeding = 90000;

        /// <summary>
        /// The ticks after missing body part to stop being fresh
        /// </summary>
        public const int TicksAfterMissingBodyPartToStopBeingFresh = 90000;

        /// <summary>
        /// The default pain shock threshold
        /// </summary>
        public const float DefaultPainShockThreshold = 0.8f;

        /// <summary>
        /// The injury heal interval
        /// </summary>
        public const int InjuryHealInterval = 600;

        /// <summary>
        /// The injury heal per day base
        /// </summary>
        public const float InjuryHealPerDayBase = 8f;

        /// <summary>
        /// The injury heal per day offset laying
        /// </summary>
        public const float InjuryHealPerDayOffsetLaying = 4f;

        /// <summary>
        /// The injury heal per day offset tended
        /// </summary>
        public const float InjuryHealPerDayOffsetTended = 8f;

        /// <summary>
        /// The injury severity tended per medicine
        /// </summary>
        public const int InjurySeverityTendedPerMedicine = 20;

        /// <summary>
        /// The base total damage lethal threshold
        /// </summary>
        public const float BaseTotalDamageLethalThreshold = 150f;

        /// <summary>
        /// The become permanent base chance
        /// </summary>
        public const float BecomePermanentBaseChance = 0.02f;

        /// <summary>
        /// The become permanent chance factor by severity curve
        /// </summary>
        public AnimationCurve BecomePermanentChanceFactorBySeverityCurve;

        /// <summary>
        /// The high
        /// </summary>
        public static readonly PainLevelWeighted[] InjuryPainLevels =
        {
            new PainLevelWeighted(PainLevel.None, 0.5f),
            new PainLevelWeighted(PainLevel.Low, 0.2f),
            new PainLevelWeighted(PainLevel.Medium, 0.2f),
            new PainLevelWeighted(PainLevel.High, 0.1f)
        };

        /// <summary>
        /// The min damage part pct for infection
        /// </summary>
        public const float MinDamagePartPctForInfection = 0.2f;

        /// <summary>
        /// The infection delay range
        /// </summary>
        public static readonly (int, int) InfectionDelayRange = new(15000, 45000);

        /// <summary>
        /// The animals infection chance factor
        /// </summary>
        public const float AnimalsInfectionChanceFactor = 0.1f;

        /// <summary>
        /// The hypothermia growth per degree under
        /// </summary>
        public const float HypothermiaGrowthPerDegreeUnder = 6.45E-05f;

        /// <summary>
        /// The heatstroke growth per degree over
        /// </summary>
        public const float HeatstrokeGrowthPerDegreeOver = 6.45E-05f;

        /// <summary>
        /// The min heatstroke progress per interval
        /// </summary>
        public const float MinHeatstrokeProgressPerInterval = 0.000375f;

        /// <summary>
        /// The min hypothermia progress
        /// </summary>
        public const float MinHypothermiaProgress = 0.00075f;

        /// <summary>
        /// The harmful temperature offset
        /// </summary>
        public const float HarmfulTemperatureOffset = 10f;

        /// <summary>
        /// The min temp over comfy max for burn
        /// </summary>
        public const float MinTempOverComfyMaxForBurn = 150f;

        /// <summary>
        /// The burn damage per temp overage
        /// </summary>
        public const float BurnDamagePerTempOverage = 0.06f;

        /// <summary>
        /// The min burn damage
        /// </summary>
        public const int MinBurnDamage = 3;

        /// <summary>
        /// The immunity gain random factor min
        /// </summary>
        public const float ImmunityGainRandomFactorMin = 0.8f;

        /// <summary>
        /// The immunity gain random factor max
        /// </summary>
        public const float ImmunityGainRandomFactorMax = 1.2f;

        /// <summary>
        /// The impossible to fall sick if above this immunity level
        /// </summary>
        public const float ImpossibleToFallSickIfAboveThisImmunityLevel = 0.6f;

        /// <summary>
        /// The health mod giver update interval
        /// </summary>
        public const int HealthModGiverUpdateInterval = 60;

        /// <summary>
        /// The vomit check interval
        /// </summary>
        public const int VomitCheckInterval = 600;

        /// <summary>
        /// The death check interval
        /// </summary>
        public const int DeathCheckInterval = 200;

        /// <summary>
        /// The forget random memory thought check interval
        /// </summary>
        public const int ForgetRandomMemoryThoughtCheckInterval = 400;

        /// <summary>
        /// The pawn base health for summary
        /// </summary>
        public const float PawnBaseHealthForSummary = 75f;

        /// <summary>
        /// The death on downed chance non colony animal
        /// </summary>
        public const float DeathOnDownedChanceNonColonyAnimal = 0.5f;

        /// <summary>
        /// The death on downed chance non colony machine
        /// </summary>
        public const float DeathOnDownedChanceNonColonyMachine = 1f;

        /// <summary>
        /// The death on downed chance non colony humanoid from population intent curve
        /// </summary>
        public AnimationCurve DeathOnDownedChanceNonColonyHumanoidFromPopulationIntentCurve;

        /// <summary>
        /// The tend priority life threatening disease
        /// </summary>
        public const float TendPriorityLifeThreateningDisease = 1f;

        /// <summary>
        /// The tend priority per bleed rate
        /// </summary>
        public const float TendPriorityPerBleedRate = 1.5f;

        /// <summary>
        /// The tend priority disease severity decreases when tended
        /// </summary>
        public const float TendPriorityDiseaseSeverityDecreasesWhenTended = 0.025f;
    }
}