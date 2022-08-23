using System.Collections.Generic;
using Assets.Scripts.World.Pawns.BodyTemplates;
using UnityEngine;
using Utilities;
using World.Pawns.BodyPartTags;
using World.Pawns.Health.HealthModifiers;

namespace World.Pawns.Health.HealthFunctions.Workers
{
    /// <summary>
    /// The talking worker class
    /// </summary>
    /// <seealso cref="HealthFunctionWorker"/>
    public class TalkingWorker : HealthFunctionWorker
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

            var functionRepo = Object.FindObjectOfType<HealthFunctionRepo>();

            return HealthFunctionUtils.CalculateTagEfficiency(pawn, healthMods, tagRepo.talkingSource) *
                   HealthFunctionUtils.CalculateTagEfficiency(pawn, healthMods, tagRepo.talkingPathway) *
                   HealthFunctionUtils.CalculateTagEfficiency(pawn, healthMods, tagRepo.tongue) *
                   HealthFunctionUtils.CalculateTagEfficiency(pawn, healthMods, tagRepo.talkingSource) *
                   CalculateFunction(pawn, functionRepo.consciousness);
        }

        /// <summary>
        /// Describes whether this instance can have function
        /// </summary>
        /// <param name="body">The body</param>
        /// <returns>The bool</returns>
        public override bool CanHaveFunction(BodyTemplate body)
        {
            var tagRepo = Object.FindObjectOfType<BodyPartTagRepo>();
            
            return body.HasPartsWithTag(tagRepo.talkingSource);
        }
    }
}
