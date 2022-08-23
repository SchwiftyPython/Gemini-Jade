using System.Collections.Generic;
using Assets.Scripts.World.Pawns.BodyTemplates;
using UnityEngine;
using Utilities;
using World.Pawns.BodyPartTags;
using World.Pawns.Health.HealthModifiers;

namespace World.Pawns.Health.HealthFunctions.Workers
{
    /// <summary>
    /// The blood filtration worker class
    /// </summary>
    /// <seealso cref="HealthFunctionWorker"/>
    public class BloodFiltrationWorker : HealthFunctionWorker
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

            if (pawn.health.HasPartsWithTag(tagRepo.bloodFiltrationKidney))
            {
                return HealthFunctionUtils.CalculateTagEfficiency(pawn, healthMods, tagRepo.bloodFiltrationKidney) *
                       HealthFunctionUtils.CalculateTagEfficiency(pawn, healthMods, tagRepo.bloodFiltrationLiver);
            }

            return HealthFunctionUtils.CalculateTagEfficiency(pawn, healthMods, tagRepo.bloodFiltrationSource);
        }

        /// <summary>
        /// Describes whether this instance can have function
        /// </summary>
        /// <param name="body">The body</param>
        /// <returns>The bool</returns>
        public override bool CanHaveFunction(BodyTemplate body)
        {
            var tagRepo = Object.FindObjectOfType<BodyPartTagRepo>();

            if (!body.HasPartsWithTag(tagRepo.bloodFiltrationKidney) ||
                !body.HasPartsWithTag(tagRepo.bloodFiltrationLiver))
            {
                return body.HasPartsWithTag(tagRepo.bloodFiltrationSource);
            }

            return true;
        }
    }
}
