using System.Collections.Generic;
using Assets.Scripts.World.Pawns;
using UnityEngine;
using World.Pawns.Health.DamageTemplates;
using World.Pawns.Health.HealthModifiers;
using World.Things;

namespace World.Pawns.Health.DamageWorkers
{
    /// <summary>
    /// The damage result class
    /// </summary>
    public class DamageResult
    {
        /// <summary>
        /// The wounded
        /// </summary>
        public bool wounded;

        /// <summary>
        /// The headshot
        /// </summary>
        public bool headshot;

        /// <summary>
        /// The deflected
        /// </summary>
        public bool deflected;

        /// <summary>
        /// The stunned
        /// </summary>
        public bool stunned;

        /// <summary>
        /// The reduced
        /// </summary>
        public bool reduced;

        /// <summary>
        /// The target
        /// </summary>
        public Thing target;

        /// <summary>
        /// The parts
        /// </summary>
        public List<BodyPart> parts;

        /// <summary>
        /// The health mods
        /// </summary>
        public List<HealthMod> healthMods;

        /// <summary>
        /// The total damage
        /// </summary>
        public float totalDamage;

        /// <summary>
        /// Adds the body part using the specified hit target
        /// </summary>
        /// <param name="hitTarget">The hit target</param>
        /// <param name="partToAdd">The part to add</param>
        public void AddBodyPart(Thing hitTarget, BodyPart partToAdd)
        {
            if (target != null)
            {
                if (target != hitTarget)
                {
                    Debug.LogError("Damage Result used for more than one target!");
                }
            }

            target = hitTarget;

            parts ??= new List<BodyPart>();
            
            parts.Add(partToAdd);
        }

        /// <summary>
        /// Adds the health mod using the specified health mod
        /// </summary>
        /// <param name="healthMod">The health mod</param>
        public void AddHealthMod(HealthMod healthMod)
        {
            healthMods ??= new List<HealthMod>();

            healthMods.Add(healthMod);
        }
    }
}
