using System.Collections.Generic;
using Assets.Scripts.World.Pawns.BodyTemplates;
using UnityEngine;
using Utilities;
using World.Pawns.BodyPartTags;
using World.Pawns.Health.HealthModifiers;

namespace World.Pawns.Health.HealthFunctions.Workers
{
    public class BreathingWorker : HealthFunctionWorker
    {
        public override float CalculateFunctionLevel(Pawn pawn, List<HealthMod> healthMods)
        {
            var tagRepo = Object.FindObjectOfType<BodyPartTagRepo>();

            return HealthFunctionUtils.CalculateTagEfficiency(pawn, healthMods, tagRepo.breathingSource) *
                   HealthFunctionUtils.CalculateTagEfficiency(pawn, healthMods, tagRepo.breathingPathway) *
                   HealthFunctionUtils.CalculateTagEfficiency(pawn, healthMods, tagRepo.breathingSourceCage);
        }

        public override bool CanHaveFunction(BodyTemplate body)
        {
            var tagRepo = Object.FindObjectOfType<BodyPartTagRepo>();

            return body.HasPartsWithTag(tagRepo.breathingSource);
        }
    }
}
