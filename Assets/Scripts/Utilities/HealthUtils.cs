using Assets.Scripts.Utilities;
using Assets.Scripts.World.Pawns;
using Assets.Scripts.World.Pawns.BodyPartDepth;
using Assets.Scripts.World.Pawns.BodyPartHeight;
using UnityEngine;
using World.Pawns;
using World.Pawns.Health.HealthModifierComponents;
using World.Pawns.Health.HealthModifiers;

namespace Utilities
{
    public class HealthUtils : MonoBehaviour
    {
        public BodyPartDepth inside;
        public BodyPartDepth outside;
        public BodyPartDepth depthUndefined;

        public BodyPartHeight top;
        public BodyPartHeight middle;
        public BodyPartHeight bottom;
        public BodyPartHeight heightUndefined;

        public string GetGeneralDestroyedPartLabel(BodyPart part, bool solid)
        {
            if (part.parent == null)
            {
                return "Seriously Impaired";
            }

            if (part.depth != inside) //todo && !fresh
            {
                return "Missing";
            }

            return solid ? "Shattered" : "Destroyed";
        }

        public void AdjustSeverity(Pawn pawn, HealthModTemplate healthModTemplate, float severityOffset)
        {
            if (severityOffset != 0f)
            {
                var firstHealthMod = pawn.health.GetFirstHealthModOf(healthModTemplate);

                if (firstHealthMod != null)
                {
                    firstHealthMod.Severity += severityOffset;
                }
                else if (severityOffset > 0f)
                {
                    firstHealthMod = HealthModMaker.MakeHealthMod(healthModTemplate, pawn);
                    firstHealthMod.Severity = severityOffset;
                    pawn.health.AddHealthMod(firstHealthMod);
                }
                
                //todo notify change in health
                
                HealthDebug.NotifyBodyChanged();
            }
        }
    }
}
