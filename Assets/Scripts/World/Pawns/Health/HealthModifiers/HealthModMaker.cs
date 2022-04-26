using System;
using UnityEngine;
using World.Pawns;
using World.Pawns.Health.HealthModifiers;

namespace Assets.Scripts.World.Pawns.Health.HealthModifiers
{
    public static class HealthModMaker 
    {
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
            healthMod.part = part;

            return healthMod;
        }
    }
}
