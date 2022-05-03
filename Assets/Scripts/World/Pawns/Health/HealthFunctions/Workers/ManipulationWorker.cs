using System.Collections.Generic;
using Assets.Scripts.World.Pawns.BodyTemplates;
using UnityEngine;
using Utilities;
using World.Pawns.BodyPartTags;
using World.Pawns.Health.HealthModifiers;

namespace World.Pawns.Health.HealthFunctions.Workers
{
    public class ManipulationWorker : HealthFunctionWorker
    {
        public override float CalculateFunctionLevel(Pawn pawn, List<HealthMod> healthMods)
        {
            var tagRepo = Object.FindObjectOfType<BodyPartTagRepo>();

            var functionRepo = Object.FindObjectOfType<HealthFunctionRepo>();

            return HealthFunctionUtils.CalculateLimbEfficiency(pawn, healthMods, tagRepo.manipulationLimbCore,
                       tagRepo.manipulationLimbSegment, tagRepo.manipulationLimbDigit, 0.8f,
                       out _) *
                   CalculateFunction(pawn, functionRepo.consciousness);
        }

        public override bool CanHaveFunction(BodyTemplate body)
        {
            var tagRepo = Object.FindObjectOfType<BodyPartTagRepo>();
            
            return body.HasPartsWithTag(tagRepo.manipulationLimbCore);
        }
    }
}
