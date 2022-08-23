using System;
using Assets.Scripts.World.Pawns;
using UnityEngine;

namespace World.Pawns.Health.HealthModifiers
{
    /// <summary>
    /// The health mod maker class
    /// </summary>
    public static class HealthModMaker 
    {
        /// <summary>
        /// Makes the health mod using the specified template
        /// </summary>
        /// <param name="template">The template</param>
        /// <param name="pawn">The pawn</param>
        /// <param name="part">The part</param>
        /// <returns>The health mod</returns>
        public static HealthMod MakeHealthMod(HealthModTemplate template, Pawn pawn, BodyPart part = null)
        {
            if (pawn == null)
            {
                Debug.LogError(string.Concat("Cannot make health mod ", template, " for null pawn."));
                return null;
            }

            var healthMod = (HealthMod) Activator.CreateInstance(template.healthModClass);
            healthMod.template = template;
            healthMod.pawn = pawn;
            healthMod.Part = part;

            return healthMod;
        }
    }
}
