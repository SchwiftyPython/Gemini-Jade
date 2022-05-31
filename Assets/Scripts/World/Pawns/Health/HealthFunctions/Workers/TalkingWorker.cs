using System.Collections.Generic;
using Assets.Scripts.World.Pawns.BodyTemplates;
using UnityEngine;
using Utilities;
using World.Pawns.BodyPartTags;
using World.Pawns.Health.HealthModifiers;

namespace World.Pawns.Health.HealthFunctions.Workers
{
    public class TalkingWorker : HealthFunctionWorker
    {
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

        public override bool CanHaveFunction(BodyTemplate body)
        {
            var tagRepo = Object.FindObjectOfType<BodyPartTagRepo>();
            
            return body.HasPartsWithTag(tagRepo.talkingSource);
        }
    }
}
