using UnityEngine;

namespace World.Pawns.Health
{
    public class HealthAdjustments : MonoBehaviour
    {
        public struct PainLevelWeighted
        {
            public PainLevel level;

            public float weight;

            public PainLevelWeighted(PainLevel level, float weight)
            {
                this.level = level;
                this.weight = weight;
            }
        }

        public const int StandardInterval = 60;

        public const float SmallPawnFragmentedDamageHealthScaleThreshold = 0.5f;

        public const int SmallPawnFragmentedDamageMinimumDamageAmount = 10;

        public static float chanceToAdditionallyDamageInnerSolidPart = 0.2f;

        public const float MinBleedingRateToBleed = 0.1f;

        public const float BleedSeverityRecoveryPerInterval = 0.00033333333f;

        public const float BloodFilthDropChanceFactorStanding = 0.004f;

        public const float BloodFilthDropChanceFactorLaying = 0.0004f;

        public const int BaseTicksAfterInjuryToStopBleeding = 90000;

        public const int TicksAfterMissingBodyPartToStopBeingFresh = 90000;

        public const float DefaultPainShockThreshold = 0.8f;

        public const int InjuryHealInterval = 600;

        public const float InjuryHealPerDayBase = 8f;

        public const float InjuryHealPerDayOffsetLaying = 4f;

        public const float InjuryHealPerDayOffsetTended = 8f;

        public const int InjurySeverityTendedPerMedicine = 20;

        public const float BaseTotalDamageLethalThreshold = 150f;

        public const float BecomePermanentBaseChance = 0.02f;

        public AnimationCurve BecomePermanentChanceFactorBySeverityCurve;

        public static readonly PainLevelWeighted[] InjuryPainLevels =
        {
            new PainLevelWeighted(PainLevel.None, 0.5f),
            new PainLevelWeighted(PainLevel.Low, 0.2f),
            new PainLevelWeighted(PainLevel.Medium, 0.2f),
            new PainLevelWeighted(PainLevel.High, 0.1f)
        };

        public const float MinDamagePartPctForInfection = 0.2f;

        public static readonly (int, int) InfectionDelayRange = new(15000, 45000);

        public const float AnimalsInfectionChanceFactor = 0.1f;

        public const float HypothermiaGrowthPerDegreeUnder = 6.45E-05f;

        public const float HeatstrokeGrowthPerDegreeOver = 6.45E-05f;

        public const float MinHeatstrokeProgressPerInterval = 0.000375f;

        public const float MinHypothermiaProgress = 0.00075f;

        public const float HarmfulTemperatureOffset = 10f;

        public const float MinTempOverComfyMaxForBurn = 150f;

        public const float BurnDamagePerTempOverage = 0.06f;

        public const int MinBurnDamage = 3;

        public const float ImmunityGainRandomFactorMin = 0.8f;

        public const float ImmunityGainRandomFactorMax = 1.2f;

        public const float ImpossibleToFallSickIfAboveThisImmunityLevel = 0.6f;

        public const int HealthModGiverUpdateInterval = 60;

        public const int VomitCheckInterval = 600;

        public const int DeathCheckInterval = 200;

        public const int ForgetRandomMemoryThoughtCheckInterval = 400;

        public const float PawnBaseHealthForSummary = 75f;

        public const float DeathOnDownedChanceNonColonyAnimal = 0.5f;

        public const float DeathOnDownedChanceNonColonyMachine = 1f;

        public AnimationCurve DeathOnDownedChanceNonColonyHumanoidFromPopulationIntentCurve;

        public const float TendPriorityLifeThreateningDisease = 1f;

        public const float TendPriorityPerBleedRate = 1.5f;

        public const float TendPriorityDiseaseSeverityDecreasesWhenTended = 0.025f;
    }
}