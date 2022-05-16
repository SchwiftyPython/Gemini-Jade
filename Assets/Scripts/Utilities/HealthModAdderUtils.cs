using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.World.Pawns;
using Assets.Scripts.World.Pawns.BodyPartTemplates;
using Assets.Scripts.World.Pawns.Health.HealthModifiers;
using World.Pawns;
using World.Pawns.Health.HealthModifiers;

namespace Utilities
{
   public static class HealthModAdderUtils
   {
      public static bool TryApply(Pawn pawn, HealthModTemplate healthModTemplate, List<BodyPartTemplate> affectedParts,
         bool affectsAnyLivePart = false, int numPartsToAffect = 1, List<HealthMod> outAddedHealthMods = null)
      {
         if (affectsAnyLivePart || affectedParts != null)
         {
            var success = false;

            for (var i = 0; i < numPartsToAffect; i++)
            {
               IEnumerable<BodyPart> validPartsToAffect = pawn.health.GetExistingParts();

               validPartsToAffect = validPartsToAffect?.Where(p => affectedParts.Contains(p.template));

               if (affectsAnyLivePart)
               {
                  validPartsToAffect = validPartsToAffect?.Where(p => p.template.alive);
               }

               if (validPartsToAffect != null)
               {
                  validPartsToAffect =
                     validPartsToAffect.Where(p =>
                        !pawn.health.HasHealthMod(healthModTemplate, p)); //&& PartOrAnyAncestorHasDirectlyAddedParts(p)

                  if (!validPartsToAffect.Any())
                  {
                     break;
                  }

                  var partToAffect = validPartsToAffect.RandomElementByWeight(part => part.coverage);

                  var healthMod = HealthModMaker.MakeHealthMod(healthModTemplate, pawn, partToAffect);

                  partToAffect.AddHealthMod(healthMod);

                  outAddedHealthMods?.Add(healthMod);
               }

               success = true;
            }

            return success;
         }

         if (!pawn.health.HasHealthMod(healthModTemplate))
         {
            var healthMod = HealthModMaker.MakeHealthMod(healthModTemplate, pawn);

            var corePart = pawn.health.GetCoreBodyPart(); //basically assuming here that if no parts to affect then it applies to core. May not be true.
            
            corePart.AddHealthMod(healthMod);
            
            outAddedHealthMods?.Add(healthMod);

            return true;
         }

         return false;
      }
   }
}
