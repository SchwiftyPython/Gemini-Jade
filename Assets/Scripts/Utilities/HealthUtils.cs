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
    /// <summary>
    /// The health utils class
    /// </summary>
    /// <seealso cref="MonoBehaviour"/>
    public class HealthUtils : MonoBehaviour
    {
        /// <summary>
        /// The inside
        /// </summary>
        public BodyPartDepth inside;
        /// <summary>
        /// The outside
        /// </summary>
        public BodyPartDepth outside;
        /// <summary>
        /// The depth undefined
        /// </summary>
        public BodyPartDepth depthUndefined;

        /// <summary>
        /// The top
        /// </summary>
        public BodyPartHeight top;
        /// <summary>
        /// The middle
        /// </summary>
        public BodyPartHeight middle;
        /// <summary>
        /// The bottom
        /// </summary>
        public BodyPartHeight bottom;
        /// <summary>
        /// The height undefined
        /// </summary>
        public BodyPartHeight heightUndefined;

        /// <summary>
        /// Gets the general destroyed part label using the specified part
        /// </summary>
        /// <param name="part">The part</param>
        /// <param name="solid">The solid</param>
        /// <returns>The string</returns>
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

        /// <summary>
        /// Adjusts the severity using the specified pawn
        /// </summary>
        /// <param name="pawn">The pawn</param>
        /// <param name="healthModTemplate">The health mod template</param>
        /// <param name="severityOffset">The severity offset</param>
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
