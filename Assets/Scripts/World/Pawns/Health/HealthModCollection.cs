using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.World.Pawns;
using UnityEngine;
using Utilities;
using World.Pawns.Health.HealthModifiers;

namespace World.Pawns.Health
{
    public class HealthModCollection
    {
        public Pawn pawn;

        public List<HealthMod> healthMods = new List<HealthMod>();
        
        //there are some caches and other collections here. Might be needed.

        public float TotalPain => CalculatePain();

        public float TotalBleedRate => CalculateBleedRate();

        public float HungerRateModifier => GetHungerRateModifier();

        public float RestModifier
        {
            get
            {
                var restModifier = 1f;

                foreach (var healthMod in healthMods)
                {
                    var currentStage = healthMod.CurrentStage;

                    if (currentStage == null)
                    {
                        continue;
                    }

                    restModifier *= currentStage.restModifier;
                }
            
                foreach (var healthMod in healthMods)
                {
                    var currentStage = healthMod.CurrentStage;

                    if (currentStage == null)
                    {
                        continue;
                    }

                    restModifier += currentStage.restModifierOffset;
                }

                return Mathf.Max(restModifier, 0f);
            }
        }

        public HealthModCollection(Pawn pawn)
        {
            this.pawn = pawn;
        }

        public void AddHealthMod(HealthMod healthModToAdd, BodyPart bodyPart = null)
        {
            //todo there's a lot going on here including merging with existing health mods. Add additional stuff as needed.

            if (bodyPart != null)
            {
                healthModToAdd.Part = bodyPart;
                
                //bodyPart.AddHealthMod(healthMod);
            }
            // else
            // {
            //     var corePart = pawn.health.GetCoreBodyPart();
            //
            //     healthMod.Part = corePart;
            //     
            //     corePart.AddHealthMod(healthMod);
            // }

            healthModToAdd.durationTicks = 0;
            
            healthModToAdd.pawn = pawn;

            var healthModMerged = false;

            foreach (var healthMod in healthMods)
            {
                if (healthMod.TryMergeWith(healthModToAdd))
                {
                    healthModMerged = true;
                }
            }

            if (!healthModMerged)
            {
                healthMods.Add(healthModToAdd);
                healthModToAdd.PostAdd();
            }

        }

        public int GetNumberOf(HealthModTemplate template)
        {
            return healthMods.ToArray().Count(healthMod => healthMod.template == template);
        }
        
        public HealthMod GetFirstHealthModOf(HealthModTemplate healthModTemplate, bool mustBeVisible = false)
        {
            return healthMods.ToArray().FirstOrDefault(healthMod =>
                healthMod.template == healthModTemplate && (!mustBeVisible || healthMod.visible));
        }

        public bool BodyPartIsMissing(BodyPart part)
        {
            return healthMods.ToArray().Any(healthMod => healthMod.Part == part && healthMod is MissingBodyPart);
        }

        public float GetBodyPartHealth(BodyPart part)
        {
            if (part == null)
            {
                return 0f;
            }

            var partHealth = part.template.GetMaxHealth(pawn);

            foreach (var healthMod in healthMods)
            {
                //todo check if injury and subtract severity
            }

            partHealth = Mathf.Max(partHealth, 0f);

            if (!part.template.destroyableByDamage)
            {
                partHealth = Mathf.Max(partHealth, 1f);
            }

            return Mathf.RoundToInt(partHealth);
        }
        
        public bool HasHealthMod(HealthModTemplate healthModTemplate, BodyPart bodyPart, bool mustBeVisible = false)
        {
            foreach (var healthMod in healthMods)
            {
                if (healthMod.template == healthModTemplate)
                {
                    return true;
                }

                if (healthMod.Part == bodyPart)
                {
                    return true;
                }
                
                if (!mustBeVisible || healthMod.visible)
                {
                    return true;
                }
            }

            return false;
        }
        
        public bool HasHealthMod(HealthModTemplate healthModTemplate, bool mustBeVisible = false)
        {
            foreach (var healthMod in healthMods)
            {
                if (healthMod.template == healthModTemplate)
                {
                    return true;
                }
                
                if (!mustBeVisible || healthMod.visible)
                {
                    return true;
                }
            }

            return false;
        }

        public List<HealthMod> GetTendableHealthMods()
        {
            return healthMods.ToArray().Where(healthMod => healthMod.NeedsTending()).ToList();
        }

        public bool HasTendableHealthMod(bool forAlert = false)
        {
            return healthMods.ToArray().Any(healthMod =>
                (!forAlert || healthMod.template.makesAlert) && healthMod.NeedsTending());
        }
        
        //todo GetAllComps
        
        //todo get tendable injuries
        
        //todo more injury methods

        public List<MissingBodyPart> GetMissingBodyParts()
        {
            var missingBodyParts = new List<MissingBodyPart>();

            var missingPartsQueue = new Queue<BodyPart>();

            missingPartsQueue.Enqueue(pawn.health.GetCoreBodyPart());

            while (missingPartsQueue.Any())
            {
                var currentPart = missingPartsQueue.Dequeue();
                
                //todo check for added parts

                MissingBodyPart missingBodyPart = null;

                foreach (var partMod in GetHealthMods<MissingBodyPart>())
                {
                    if (partMod.Part != currentPart)
                    {
                        continue;
                    }

                    missingBodyPart = partMod;
                    break;
                }

                if (missingBodyPart != null)
                {
                    missingBodyParts.Add(missingBodyPart);
                    continue;
                }

                foreach (var childPart in currentPart.GetAllChildren().ToArray())
                {
                    missingPartsQueue.Enqueue(childPart);
                }
            }

            return missingBodyParts;
        }
        
        public List<BodyPart> GetExistingParts()
        {
            var existingParts = new List<BodyPart>();

            foreach (var bodyPart in pawn.GetBody())
            {
                if (!BodyPartIsMissing(bodyPart))
                {
                    existingParts.Add(bodyPart);
                }
            }

            return existingParts;
        }

        public BodyPart GetRandomExistingPart() //todo damage def
        {
            var existingParts = GetExistingParts();

            return !existingParts.Any() ? null : existingParts.RandomElementByWeight(part => part.coverage); //todo * hit chance mod from damage def
        }

        public List<HealthMod> GetTendableNonInjuryNonMissingHealthMods()
        {
            //todo

            throw new NotImplementedException();
        }

        public bool HasTendableNonInjuryNonMissingHealthMod(bool forAlert = false)
        {
            //todo 

            throw new NotImplementedException();
        }

        public bool HasImmunizableNotImmuneHealthMod()
        {
            //todo 

            throw new NotImplementedException();
        }

        public List<T> GetHealthMods<T>() where T : HealthMod
        {
            var filteredMods = new List<T>();
            
            foreach (var healthMod in healthMods.ToArray())
            {
                if (healthMod is T mod)
                {
                    filteredMods.Add(mod);
                }
            }

            return filteredMods;
        }

        public void Clear()
        {
            healthMods.Clear();
        }

        private float GetHungerRateModifier()
        {
            var hungerRateModifier = 1f;

            foreach (var healthMod in healthMods)
            {
                var currentStage = healthMod.CurrentStage;

                if (currentStage == null)
                {
                    continue;
                }

                hungerRateModifier *= currentStage.hungerRateModifier;
            }
            
            foreach (var healthMod in healthMods)
            {
                var currentStage = healthMod.CurrentStage;

                if (currentStage == null)
                {
                    continue;
                }

                hungerRateModifier += currentStage.hungerRateModifierOffset;
            }

            return Mathf.Max(hungerRateModifier, 0f);
        }

        private float CalculatePain()
        {
            if (!pawn.IsOrganic || pawn.Dead)
            {
                return 0f;
            }

            if (healthMods == null)
            {
                return 0f;
            }

            if (!healthMods.Any())
            {
                return 0f;
            }

            var pain = 0f;

            foreach (var healthMod in healthMods)
            {
                pain += healthMod.PainOffset;
            }
            
            foreach (var healthMod in healthMods)
            {
                pain *= healthMod.PainFactor;
            }

            return Mathf.Clamp(pain, 0f, 1f);
        }
        
        private float CalculateBleedRate()
        {
            if (!pawn.IsOrganic || pawn.Dead)
            {
                return 0f;
            }

            var bleedModifier = 1f;

            var bleedRateTotal = 0f;

            foreach (var healthMod in healthMods.ToArray())
            {
                var currentStage = healthMod.CurrentStage;

                if (currentStage != null)
                {
                    bleedModifier *= currentStage.bleedModifier;
                }

                bleedRateTotal += healthMod.BleedRate;
            }

            return bleedRateTotal * bleedModifier; //todo divided by pawn health scale
        }
    }
}
