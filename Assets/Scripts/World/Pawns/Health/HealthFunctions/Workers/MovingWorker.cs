using System.Collections.Generic;
using Assets.Scripts.World.Pawns.BodyTemplates;
using UnityEngine;
using Utilities;
using World.Pawns.BodyPartTags;
using World.Pawns.Health.HealthModifiers;

namespace World.Pawns.Health.HealthFunctions.Workers
{
    /// <summary>
    /// The moving worker class
    /// </summary>
    /// <seealso cref="HealthFunctionWorker"/>
    public class MovingWorker : HealthFunctionWorker
    {
        /// <summary>
        /// Calculates the function level using the specified pawn
        /// </summary>
        /// <param name="pawn">The pawn</param>
        /// <param name="healthMods">The health mods</param>
        /// <returns>The float</returns>
        public override float CalculateFunctionLevel(Pawn pawn, List<HealthMod> healthMods)
        {
            var tagRepo = Object.FindObjectOfType<BodyPartTagRepo>();

            var functionLevel = HealthFunctionUtils.CalculateLimbEfficiency(pawn, healthMods, tagRepo.movingLimbCore,
                tagRepo.movingLimbSegment, tagRepo.movingLimbDigit, 0.4f, out var functionalPercentage);

            if (functionalPercentage < 0.5f)
            {
                return 0f;
            }

            functionLevel *= HealthFunctionUtils.CalculateTagEfficiency(pawn, healthMods, tagRepo.pelvis);
            
            functionLevel *= HealthFunctionUtils.CalculateTagEfficiency(pawn, healthMods, tagRepo.spine);

            var functionRepo = Object.FindObjectOfType<HealthFunctionRepo>();

            functionLevel = Mathf.Lerp(functionLevel, functionLevel * CalculateFunction(pawn, functionRepo.breathing),
                0.2f);
            
            functionLevel = Mathf.Lerp(functionLevel, functionLevel * CalculateFunction(pawn, functionRepo.bloodPumping),
                0.2f);

            return functionLevel * Mathf.Min(CalculateFunction(pawn, functionRepo.consciousness), 1f);
        }

        /// <summary>
        /// Describes whether this instance can have function
        /// </summary>
        /// <param name="body">The body</param>
        /// <returns>The bool</returns>
        public override bool CanHaveFunction(BodyTemplate body)
        {
            var tagRepo = Object.FindObjectOfType<BodyPartTagRepo>();
            
            return body.HasPartsWithTag(tagRepo.movingLimbCore);
        }
    }
}
