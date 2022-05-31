using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.World.Pawns;
using Assets.Scripts.World.Pawns.BodyPartTags;
using UnityEngine;
using World.Pawns;
using World.Pawns.Health.HealthFunctions;
using World.Pawns.Health.HealthModifiers;

namespace Utilities
{
    public static class HealthFunctionUtils
    {
        //todo looks like impactors are used to list reason for function efficiency when hovering over function in health summary
        
        public static float CalculateFunctionLevel(Pawn pawn, List<HealthMod> healthMods, HealthFunctionTemplate function)
        {
            if (function.zeroIfCannotWakeUp && !pawn.CanWakeUp)
            {
                return 0f;
            }

            var level = function.Worker.CalculateFunctionLevel(pawn, healthMods);
            
            if (level <= 0f)
            {
                return level;
            }

            if (healthMods == null)
            {
                return level;
            }

            if (!healthMods.Any())
            {
                return level;
            }

            foreach (var healthMod in healthMods)
            {
                //todo get function mods and loop through them
                    
                //a lot of voodoo going on in this loop. See what we end up needing
                //will probably make more sense once we start seeing the numbers
            }

            return level;
        }

        public static float CalculatePartEfficiency(BodyPart bodyPart, Pawn pawn, List<HealthMod> healthMods)
        {
            //todo there are checks here for added parts as an impact. 

            if (bodyPart.parent != null && pawn.health.BodyPartIsMissing(bodyPart.parent))
            {
                return 0f;
            }
            
            //todo efficiency based on current stage of health mod

            var efficiency = pawn.health.GetPartHealth(bodyPart) / bodyPart.template.GetMaxHealth(pawn);
            
            //todo if efficiency != 1 add impactors

            return Mathf.Max(efficiency, 0f);
        }

        public static float CalculateTagEfficiency(Pawn pawn, List<HealthMod> healthMods, BodyPartTagTemplate tag)
        {
            var partsWithTag = pawn.health.GetBodyPartsWithTag(tag);

            if (partsWithTag == null)
            {
                return 1f;
            }
            
            if (!partsWithTag.Any())
            {
                return 1f;
            }

            var tagEfficiency = 0f;

            foreach (var bodyPart in partsWithTag)
            {
                var partEfficiency = CalculatePartEfficiency(bodyPart, pawn, healthMods);

                tagEfficiency += partEfficiency;
            }

            //todo lots of mystery numbers. Not clear on what some of them do.
            
            tagEfficiency /= partsWithTag.Count;
            
            //todo add impactors
            
            //todo enforce a min max

            return tagEfficiency;
        }

        public static float CalculateLimbEfficiency(Pawn pawn, List<HealthMod> healthMods,
            BodyPartTagTemplate limbCoreTag, BodyPartTagTemplate limbSegmentTag, BodyPartTagTemplate limbDigitTag,
            float appendageWeight, out float functionalPercentage)
        {
            var partsWithTag = pawn.health.GetBodyPartsWithTag(limbCoreTag);
            
            if (partsWithTag == null)
            {
                functionalPercentage = 0f;
                return 1f;
            }
            
            if (!partsWithTag.Any())
            {
                functionalPercentage = 0f;
                return 1f;
            }
            
            var limbEfficiency = 0f;
            var numFunctional = 0;

            foreach (var bodyPart in partsWithTag)
            {
                var partEfficiency = CalculatePartEfficiency(bodyPart, pawn, healthMods);

                var connectedParts = bodyPart.GetConnectedParts(limbSegmentTag);

                foreach (var connectedPart in connectedParts)
                {
                    partEfficiency *= CalculatePartEfficiency(connectedPart, pawn, healthMods);
                }
                
                var digitEfficiency = 0f;

                if (bodyPart.HasChildParts(limbDigitTag))
                {
                    var childParts = bodyPart.GetChildParts(limbDigitTag).ToList();

                    foreach (var childPart in childParts)
                    {
                        digitEfficiency += CalculatePartEfficiency(childPart, pawn, healthMods);
                    }

                    partEfficiency = Mathf.Lerp(partEfficiency, digitEfficiency / childParts.Count, appendageWeight);
                }

                limbEfficiency += partEfficiency;

                if (digitEfficiency > 0f)
                {
                    numFunctional++;
                }
            }

            functionalPercentage = (float)numFunctional / partsWithTag.Count;

            return limbEfficiency / partsWithTag.Count;
        }
    }
}
