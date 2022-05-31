using System.Collections.Generic;
using Assets.Scripts.World.Pawns.BodyTemplates;
using UnityEngine;
using Utilities;
using World.Pawns.BodyPartTags;
using World.Pawns.Health.HealthModifiers;

namespace World.Pawns.Health.HealthFunctions.Workers
{
    public class BloodFiltrationWorker : HealthFunctionWorker
    {
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
