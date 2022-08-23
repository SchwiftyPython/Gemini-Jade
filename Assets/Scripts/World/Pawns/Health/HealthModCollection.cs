using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.World.Pawns;
using Assets.Scripts.World.Pawns.BodyPartDepth;
using Assets.Scripts.World.Pawns.BodyPartHeight;
using Assets.Scripts.World.Pawns.BodyPartTags;
using UnityEngine;
using Utilities;
using World.Pawns.Health.DamageTemplates;
using World.Pawns.Health.HealthModifierComponents;
using World.Pawns.Health.HealthModifiers;
using Object = UnityEngine.Object;

namespace World.Pawns.Health
{
    /// <summary>
    /// The health mod collection class
    /// </summary>
    public class HealthModCollection
    {
        /// <summary>
        /// The pawn
        /// </summary>
        public Pawn pawn;

        /// <summary>
        /// The health mods
        /// </summary>
        public List<HealthMod> healthMods;
        
        //there are some caches and other collections here. Might be needed.

        /// <summary>
        /// Gets the value of the total pain
        /// </summary>
        public float TotalPain => CalculatePain();

        /// <summary>
        /// Gets the value of the total bleed rate
        /// </summary>
        public float TotalBleedRate => CalculateBleedRate();

        /// <summary>
        /// Gets the value of the hunger rate modifier
        /// </summary>
        public float HungerRateModifier => GetHungerRateModifier();

        /// <summary>
        /// Gets the value of the rest modifier
        /// </summary>
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

        /// <summary>
        /// Initializes a new instance of the <see cref="HealthModCollection"/> class
        /// </summary>
        /// <param name="pawn">The pawn</param>
        public HealthModCollection(Pawn pawn)
        {
            this.pawn = pawn;
            
            healthMods = new List<HealthMod>();
        }

        /// <summary>
        /// Adds the health mod using the specified health mod to add
        /// </summary>
        /// <param name="healthModToAdd">The health mod to add</param>
        /// <param name="bodyPart">The body part</param>
        public void AddHealthMod(HealthMod healthModToAdd, BodyPart bodyPart = null)
        {
            //todo there's a lot going on here. Add additional stuff as needed.

            if (bodyPart != null)
            {
                healthModToAdd.Part = bodyPart;
            }

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
                
                //todo thoughts and needs
            }

            var isMissingPart = healthModToAdd is MissingBodyPart;

            if (!isMissingPart && healthModToAdd.Part != null && !healthModToAdd.Part.IsCorePart &&
                GetBodyPartHealth(healthModToAdd.Part) == 0f)
            {
                //todo check for added parts

                var healthModRepo = Object.FindObjectOfType<HealthModRepo>();

                var missingPartHealthMod = HealthModMaker.MakeHealthMod(healthModRepo.missingBodyPart, pawn);
                
                //todo missing part is fresh = !hasAddedParts
                
                //todo last injury = healthModToAdd.template
                
                pawn.health.AddHealthMod(missingPartHealthMod, healthModToAdd.Part);
                
                //todo add missing part mod to damage result
                
                //todo if has added part do some stuff

                isMissingPart = true;
            }
            
            //todo notify apparel of lost body part if applicable
        }

        /// <summary>
        /// Removes the health mod using the specified health mod to remove
        /// </summary>
        /// <param name="healthModToRemove">The health mod to remove</param>
        public void RemoveHealthMod(HealthMod healthModToRemove)
        {
            if (!HasHealthMod(healthModToRemove.template))
            {
                return;
            }

            healthMods.Remove(healthModToRemove);
            
            healthModToRemove.PostRemove();
        }

        /// <summary>
        /// Gets the number of using the specified template
        /// </summary>
        /// <param name="template">The template</param>
        /// <returns>The int</returns>
        public int GetNumberOf(HealthModTemplate template)
        {
            return healthMods.ToArray().Count(healthMod => healthMod.template == template);
        }
        
        /// <summary>
        /// Gets the first health mod of using the specified health mod template
        /// </summary>
        /// <param name="healthModTemplate">The health mod template</param>
        /// <param name="mustBeVisible">The must be visible</param>
        /// <returns>The health mod</returns>
        public HealthMod GetFirstHealthModOf(HealthModTemplate healthModTemplate, bool mustBeVisible = false)
        {
            return healthMods.ToArray().FirstOrDefault(healthMod =>
                healthMod.template == healthModTemplate && (!mustBeVisible || healthMod.visible));
        }

        /// <summary>
        /// Describes whether this instance body part is missing
        /// </summary>
        /// <param name="part">The part</param>
        /// <returns>The bool</returns>
        public bool BodyPartIsMissing(BodyPart part)
        {
            return healthMods.ToArray().Any(healthMod => healthMod.Part == part && healthMod is MissingBodyPart);
        }

        /// <summary>
        /// Gets the body part health using the specified part
        /// </summary>
        /// <param name="part">The part</param>
        /// <returns>The float</returns>
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
        
        /// <summary>
        /// Describes whether this instance has health mod
        /// </summary>
        /// <param name="healthModTemplate">The health mod template</param>
        /// <param name="bodyPart">The body part</param>
        /// <param name="mustBeVisible">The must be visible</param>
        /// <returns>The bool</returns>
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
        
        /// <summary>
        /// Describes whether this instance has health mod
        /// </summary>
        /// <param name="healthModTemplate">The health mod template</param>
        /// <param name="mustBeVisible">The must be visible</param>
        /// <returns>The bool</returns>
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

        /// <summary>
        /// Gets the tendable health mods
        /// </summary>
        /// <returns>A list of health mod</returns>
        public List<HealthMod> GetTendableHealthMods()
        {
            return healthMods.ToArray().Where(healthMod => healthMod.NeedsTending()).ToList();
        }

        /// <summary>
        /// Describes whether this instance has tendable health mod
        /// </summary>
        /// <param name="forAlert">The for alert</param>
        /// <returns>The bool</returns>
        public bool HasTendableHealthMod(bool forAlert = false)
        {
            return healthMods.ToArray().Any(healthMod =>
                (!forAlert || healthMod.template.makesAlert) && healthMod.NeedsTending());
        }

        /// <summary>
        /// Gets the all comps
        /// </summary>
        /// <returns>The components</returns>
        public List<HealthModComp> GetAllComps()
        {
            var components = new List<HealthModComp>();

            foreach (var healthMod in healthMods)
            {
                if (!healthMod.HasComps)
                {
                    continue;
                }

                foreach (var healthModComp in healthMod.comps)
                {
                    components.Add(healthModComp);
                }
            }

            return components;
        }

        /// <summary>
        /// Gets the tendable injuries
        /// </summary>
        /// <returns>The tendable injuries</returns>
        public List<Injury> GetTendableInjuries()
        {
            var tendableInjuries = new List<Injury>();
            
            foreach (var healthMod in healthMods)
            {
                if (healthMod is Injury injury && injury.NeedsTending())
                {
                    tendableInjuries.Add(injury);
                }
            }

            return tendableInjuries;
        }
        
        /// <summary>
        /// Describes whether this instance has tendable injury
        /// </summary>
        /// <returns>The bool</returns>
        public bool HasTendableInjury()
        {
            foreach (var healthMod in healthMods)
            {
                if (healthMod is Injury injury && injury.NeedsTending())
                {
                    return true;
                }
            }

            return false;
        }
        
        /// <summary>
        /// Describes whether this instance has naturally healing injury
        /// </summary>
        /// <returns>The bool</returns>
        public bool HasNaturallyHealingInjury()
        {
            foreach (var healthMod in healthMods)
            {
                if (healthMod is Injury injury && injury.CanHealNaturally())
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Describes whether this instance has tended and healing injury
        /// </summary>
        /// <returns>The bool</returns>
        public bool HasTendedAndHealingInjury()
        {
            foreach (var healthMod in healthMods)
            {
                if (healthMod is not Injury injury)
                {
                    continue;
                }

                if (injury.CanHealFromTending() && injury.Severity > 0f)
                {
                    return true;
                }
            }

            return false;
        }
        
        // public bool HasTemperatureInjury(TemperatureInjuryStage minStage)
        // {
        //     todo
        // }

        /// <summary>
        /// Gets the injured parts
        /// </summary>
        /// <returns>An enumerable of body part</returns>
        public IEnumerable<BodyPart> GetInjuredParts()
        {
            return (from hm in healthMods where hm is Injury select hm.Part).Distinct();
        }

        /// <summary>
        /// Gets the naturally healing injured parts
        /// </summary>
        /// <returns>The natural healing parts</returns>
        public IEnumerable<BodyPart> GetNaturallyHealingInjuredParts()
        {
            var naturalHealingParts = new List<BodyPart>();
            
            foreach (var injuredPart in GetInjuredParts())
            {
                foreach (var healthMod in healthMods)
                {
                    if (healthMod is not Injury injury)
                    {
                        continue;
                    }

                    if (injury.Part != injuredPart)
                    {
                        continue;
                    }

                    if (!injury.CanHealNaturally())
                    {
                        continue;
                    }

                    naturalHealingParts.Add(injuredPart);
                    break;
                }
            }

            return naturalHealingParts;
        }
        
        //todo more injury methods

        /// <summary>
        /// Gets the missing body parts
        /// </summary>
        /// <returns>The missing body parts</returns>
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
        
        /// <summary>
        /// Gets the existing parts
        /// </summary>
        /// <returns>The existing parts</returns>
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

        /// <summary>
        /// Gets the existing parts using the specified height
        /// </summary>
        /// <param name="height">The height</param>
        /// <param name="depth">The depth</param>
        /// <param name="tag">The tag</param>
        /// <param name="parent">The parent</param>
        /// <returns>The existing parts</returns>
        public List<BodyPart> GetExistingParts(BodyPartHeight height, BodyPartDepth depth,
            BodyPartTagTemplate tag = null, BodyPart parent = null)
        {
            var existingParts = new List<BodyPart>();

            var healthUtils = Object.FindObjectOfType<HealthUtils>();

            foreach (var bodyPart in pawn.GetBody())
            {
                if (pawn.health.BodyPartIsMissing(bodyPart))
                {
                    continue;
                }

                if (height != healthUtils.heightUndefined)
                {
                    if (bodyPart.height != height)
                    {
                        continue;
                    }
                }

                if (depth != healthUtils.depthUndefined)
                {
                    if (bodyPart.depth != depth)
                    {
                        continue;
                    }
                }

                if (tag != null)
                {
                    if (!bodyPart.template.tags.Contains(tag))
                    {
                        continue;
                    }
                }

                if (parent == null)
                {
                    existingParts.Add(bodyPart);
                }
                else if (bodyPart.parent == parent)
                {
                    existingParts.Add(bodyPart);
                }
            }
            return existingParts;
        }

        /// <summary>
        /// Gets the random existing part
        /// </summary>
        /// <returns>The body part</returns>
        public BodyPart GetRandomExistingPart() 
        {
            var existingParts = GetExistingParts();

            return !existingParts.Any() ? null : existingParts.RandomElementByWeight(part => part.coverage);
        }

        /// <summary>
        /// Gets the random existing part using the specified height
        /// </summary>
        /// <param name="height">The height</param>
        /// <param name="depth">The depth</param>
        /// <param name="parent">The parent</param>
        /// <returns>The body part</returns>
        public BodyPart GetRandomExistingPart(BodyPartHeight height, BodyPartDepth depth, BodyPart parent = null)
        {
            var existingParts = GetExistingParts(height, depth, null, parent);

            if (existingParts.Any())
            {
                return existingParts.RandomElementByWeight(part => part.coverage);
            }

            var healthUtils = Object.FindObjectOfType<HealthUtils>();

            existingParts = GetExistingParts(healthUtils.heightUndefined, depth, null, parent);

            return !existingParts.Any() ? null : existingParts.RandomElementByWeight(part => part.coverage);
        }

        /// <summary>
        /// Gets the tendable non injury non missing health mods
        /// </summary>
        /// <returns>The tendable mods</returns>
        public List<HealthMod> GetTendableNonInjuryNonMissingHealthMods()
        {
            var tendableMods = new List<HealthMod>();

            foreach (var healthMod in healthMods)
            {
                if (healthMod is not Injury && healthMod is not MissingBodyPart && healthMod.NeedsTending())
                {
                    tendableMods.Add(healthMod);
                }
            }

            return tendableMods;
        }

        /// <summary>
        /// Describes whether this instance has tendable non injury non missing health mod
        /// </summary>
        /// <param name="forAlert">The for alert</param>
        /// <returns>The bool</returns>
        public bool HasTendableNonInjuryNonMissingHealthMod(bool forAlert = false)
        {
            foreach (var healthMod in healthMods)
            {
                if (forAlert && !healthMod.template.makesAlert)
                {
                    continue;
                }

                if (healthMod is not Injury && healthMod is not MissingBodyPart && healthMod.NeedsTending())
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Describes whether this instance has immunizable not immune health mod
        /// </summary>
        /// <returns>The bool</returns>
        public bool HasImmunizableNotImmuneHealthMod()
        {
            foreach (var healthMod in healthMods)
            {
                if (healthMod is Injury or MissingBodyPart)
                {
                    continue;
                }

                if (!healthMod.Visible)
                {
                    continue;
                }

                if (healthMod.template.CanDevelopImmunityNaturally() && !healthMod.FullyImmune())
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Gets the health mods
        /// </summary>
        /// <typeparam name="T">The </typeparam>
        /// <returns>The filtered mods</returns>
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

        /// <summary>
        /// Clears this instance
        /// </summary>
        public void Clear()
        {
            healthMods.Clear();
        }

        /// <summary>
        /// Gets the hunger rate modifier
        /// </summary>
        /// <returns>The float</returns>
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

        /// <summary>
        /// Calculates the pain
        /// </summary>
        /// <returns>The float</returns>
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
        
        /// <summary>
        /// Calculates the bleed rate
        /// </summary>
        /// <returns>The float</returns>
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
