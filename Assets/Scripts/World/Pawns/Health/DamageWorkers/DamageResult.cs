using System.Collections.Generic;
using Assets.Scripts.World.Pawns;
using UnityEngine;
using World.Pawns.Health.DamageTemplates;
using World.Pawns.Health.HealthModifiers;
using World.Things;

namespace World.Pawns.Health.DamageWorkers
{
    public class DamageResult
    {
        public bool wounded;

        public bool headshot;

        public bool deflected;

        public bool stunned;

        public bool reduced;

        public Thing target;

        public List<BodyPart> parts;

        public List<HealthMod> healthMods;

        public float totalDamage;

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

        public void AddHealthMod(HealthMod healthMod)
        {
            healthMods ??= new List<HealthMod>();

            healthMods.Add(healthMod);
        }
    }
}
