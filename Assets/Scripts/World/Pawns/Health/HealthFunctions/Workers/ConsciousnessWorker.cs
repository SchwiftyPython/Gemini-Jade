using System.Collections.Generic;
using Assets.Scripts.World.Pawns.BodyTemplates;
using UnityEngine;
using Utilities;
using World.Pawns.BodyPartTags;
using World.Pawns.Health.HealthModifiers;

namespace World.Pawns.Health.HealthFunctions.Workers
{
    public class ConsciousnessWorker : HealthFunctionWorker
    {
        public override float CalculateFunctionLevel(Pawn pawn, List<HealthMod> healthMods)
        {
            var tagRepo = Object.FindObjectOfType<BodyPartTagRepo>();

            var tagEfficiency =
                HealthFunctionUtils.CalculateTagEfficiency(pawn, healthMods, tagRepo.consciousnessSource);
            
            //todo get pain total
            
            //todo if pain > 0 then subtract it from tag efficiency and add pain impactor
            
            //not sure what the Lerps are going to give us, but I'm curious

            var functionRepo = Object.FindObjectOfType<HealthFunctionRepo>();

            tagEfficiency = Mathf.Lerp(tagEfficiency,
                tagEfficiency * Mathf.Min(CalculateFunction(pawn, healthMods, functionRepo.bloodPumping), 1f), 0.2f); //todo need worker
            
            tagEfficiency = Mathf.Lerp(tagEfficiency,
                tagEfficiency * Mathf.Min(CalculateFunction(pawn, healthMods, functionRepo.breathing), 1f), 0.2f); //todo need worker
            
            return Mathf.Lerp(tagEfficiency,
                tagEfficiency * Mathf.Min(CalculateFunction(pawn, healthMods, functionRepo.bloodFiltration), 1f), 0.2f); //todo need worker
        }

        public override bool CanHaveFunction(BodyTemplate body)
        {
            var tagRepo = Object.FindObjectOfType<BodyPartTagRepo>();

            return body.HasPartsWithTag(tagRepo.consciousnessSource);
        }
    }
}
