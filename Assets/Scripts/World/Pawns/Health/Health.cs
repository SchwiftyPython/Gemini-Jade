using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.World.Pawns;
using Assets.Scripts.World.Pawns.BodyPartTags;
using Assets.Scripts.World.Pawns.Health;
using UnityEngine;
using Utilities;
using World.Pawns.Health.HealthFunctions;
using World.Pawns.Health.HealthModifiers;
using Object = UnityEngine.Object;

namespace World.Pawns.Health
{
    public class Health
    {
        private Pawn _pawn;

        private HealthState _healthState = HealthState.Mobile;

        //todo health mods -- might be too slow to get mods from body parts for health summary
        //downside is we have to add and remove from two places if we do this so lets see what performance is like

        private FunctionsHandler _functions;

        private HealthFunctionRepo _functionsRepo;

        //todo health summary calculator

        private List<BodyPart> _body;

        public HealthState State => _healthState;

        public bool Downed => _healthState == HealthState.Down;

        public bool Dead => _healthState == HealthState.Dead;

        public float maxLethalDamage = 150f;

        public float painShockThreshold = 0.8f; //todo define in pawn's stats

        public bool CanWakeUp => _functions.CanWakeUp;

        public Health(Pawn pawn)
        {
            _pawn = pawn;

            BuildBody();

            _functionsRepo = Object.FindObjectOfType<HealthFunctionRepo>();

            _functions = new FunctionsHandler(_pawn);

            //todo set all the other properties
        }

        public List<BodyPart> GetBody()
        {
            return _body;
        }

        public List<BodyPart> GetExistingParts()
        {
            var existingParts = new List<BodyPart>();

            foreach (var bodyPart in _body)
            {
                if (!bodyPart.IsMissing())
                {
                    existingParts.Add(bodyPart);
                }
            }

            return existingParts;
        }

        public List<BodyPart> GetBodyPartsWithTag(BodyPartTagTemplate tag)
        {
            var taggedParts = new List<BodyPart>();

            foreach (var bodyPart in _body)
            {
                if (bodyPart.template.tags.Contains(tag))
                {
                    taggedParts.Add(bodyPart);
                }
            }

            return taggedParts;
        }

        public bool HasPartsWithTag(BodyPartTagTemplate tag)
        {
            return _pawn.species.bodyTemplate.HasPartsWithTag(tag);
        }
        
        public BodyPart GetCoreBodyPart()
        {
            foreach (var bodyPart in _body)
            {
                if (bodyPart.IsCorePart)
                {
                    return bodyPart;
                }
            }

            return null;
        }

        public float GetLevel(HealthFunctionTemplate function)
        {
            return _functions.GetLevel(function, GetHealthMods());
        }

        public void AddHealthMod(HealthMod healthMod, BodyPart bodyPart = null)
        {
            //todo there's a lot going on here including merging with existing health mods. Add additional stuff as needed.
            
            //basically assuming here that if no parts to affect then it applies to core. May not be true.

            if (bodyPart != null)
            {
                healthMod.Part = bodyPart;
            }
            else
            {
                var corePart = _pawn.health.GetCoreBodyPart();

                healthMod.Part = corePart;
                
                corePart.AddHealthMod(healthMod);
            }

            healthMod.durationTicks = 0;
            
            healthMod.pawn = _pawn;
            
            healthMod.PostAdd();
        }

        public void RemoveHealthMod(HealthMod healthMod)
        {
            healthMod.Part.RemoveHealthMod(healthMod);
            
            healthMod.PostRemove();
            
            CheckForHealthStateChange(null);
        }

        public List<HealthMod> GetHealthMods()
        {
            var mods = new List<HealthMod>();
            
            foreach (var bodyPart in _body)
            {
                if (!bodyPart.HasHealthMods())
                {
                    continue;
                }
                
                mods.AddRange(bodyPart.GetHealthMods());
            }

            return mods;
        }

        public HealthMod GetFirstHealthModOf(HealthModTemplate healthModTemplate, bool mustBeVisible = false)
        {
            var healthMods = GetHealthMods();

            return healthMods.ToArray().FirstOrDefault(healthMod =>
                healthMod.template == healthModTemplate && (!mustBeVisible || healthMod.visible));
        }

        public bool HasHealthMod(HealthModTemplate healthModTemplate, BodyPart bodyPart, bool mustBeVisible = false)
        {
            var healthMods = GetHealthMods();

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
            var healthMods = GetHealthMods();

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
        
        public float GetPartHealth(BodyPart bodyPart)
        {
            var health = bodyPart.template.GetMaxHealth(_pawn);

            var healthMods = bodyPart.GetHealthMods();

            if (healthMods != null && healthMods.Any())
            {
                foreach (var healthMod in healthMods)
                {
                    if (healthMod is MissingBodyPart)
                    {
                        return 0f;
                    }
                    //todo if injury health -= injury severity
                }
            }

            health = Mathf.Max(health, 0f);

            if (!bodyPart.template.destroyableByDamage)
            {
                health = Mathf.Max(health, 1f);
            }

            return Mathf.RoundToInt(health);
        }

        public IDictionary<HealthFunctionTemplate, float> GetFunctionValues()
        {
            return _functions.GetFunctionLevels();
        }

        public float GetPainTotal()
        {
            //todo need to get stages implemented for us to get a number besides 0 here
            
            return CalculatePain();
        }

        public float GetBleedRateTotal()
        {
            return CalculateBleedRate();
        }

        public void Tick()
        {
            if (Dead)
            {
                return;
            }

            var healthMods = GetHealthMods();

            foreach (var healthMod in healthMods)
            {
                healthMod.Tick();
                healthMod.PostTick();

                if (Dead)
                {
                    return;
                }
            }

            var healthModsChanged = false;

            //they iterate backwards here
            foreach (var healthMod in healthMods.ToArray())
            {
                if (healthMod.ShouldRemove)
                {
                    healthMods.Remove(healthMod);
                    healthMod.PostRemove();
                    healthModsChanged = true;
                }
            }

            if (healthModsChanged)
            {
                //todo notify health mods changed
                //looks like it resets a bunch of caches
                
                CheckForHealthStateChange(null);
            }
        }

        public void CheckForHealthStateChange(HealthMod healthMod)
        {
            if (Dead)
            {
                return;
            }

            if (ShouldBeDead())
            {
                _pawn.Die();
            }
            else if (!Downed)
            {
                if (ShouldBeDowned())
                {
                    DownPawn();
                }
                else
                {
                    if(_functions.CapableOf(_functionsRepo.manipulation))
                    {
                        return;
                    }
                    
                    //todo bunch of other checks mostly equipment stuff
                }
            }
            else if (!ShouldBeDowned())
            {
                UnDownPawn();
            }
        }
        
        public void KillPawn()
        {
            if (Dead)
            {
                return;
            }

            _healthState = HealthState.Dead;
        }

        private float CalculateBleedRate()
        {
            if (!_pawn.IsOrganic || _pawn.Dead)
            {
                return 0f;
            }

            var bleedmodifier = 1f;

            var bleedRateTotal = 0f;

            var healthMods = GetHealthMods();

            foreach (var healthMod in healthMods.ToArray())
            {
                var currentStage = healthMod.CurrentStage;

                if (currentStage != null)
                {
                    bleedmodifier *= currentStage.bleedModifier;
                }

                bleedRateTotal += healthMod.BleedRate;
            }

            return bleedRateTotal * bleedmodifier; //todo divided by pawn health scale
        }

        private bool ShouldBeDowned()
        {
            if (_functions.CanWakeUp) //todo !InPainShock
            {
                return !_functions.CapableOf(_functionsRepo.moving);
            }

            return true;
        }

        private bool ShouldBeDead()
        {
            if (Dead)
            {
                return true;
            }

            var healthMods = GetHealthMods();

            foreach (var healthMod in healthMods)
            {
                if (healthMod.CauseDeathNow())
                {
                    return true;
                }
            }

            if (ShouldBeDeadFromCriticalFunction())
            {
                return true;
            }

            if (HealthFunctionUtils.CalculatePartEfficiency(GetCoreBodyPart(), _pawn, healthMods) <= 0f)
            {
                return true;
            }

            return ShouldBeDeadFromLethalDamage();
        }

        private bool ShouldBeDeadFromCriticalFunction()
        {
            var allHealthFunctions = _functionsRepo.GetAllHealthFunctions();

            foreach (var healthFunction in allHealthFunctions)
            {
                if ((_pawn.IsOrganic ? healthFunction.criticalOrganic : healthFunction.criticalMachines) &&
                    !_functions.CapableOf(healthFunction))
                {
                    return true;
                }
            }

            return false;
        }

        private bool ShouldBeDeadFromLethalDamage()
        {
            //todo add up severity of all injury health mods
            
            //todo if severity >= lethal damage threshold then return true

            return false;
        }

        private void DownPawn()
        {
            if (Downed)
            {
                return;
            }

            _healthState = HealthState.Down;
            
            //todo if spawned drop and forbid their shit
            
            //todo checks damage info for an instigator then notifies pawn downed
        }

        private void UnDownPawn()
        {
            if (!Downed)
            {
                return;
            }

            _healthState = HealthState.Mobile;
            
            //todo message no longer downed
            
            //todo if spawned end current job as unable to complete. Guessing current job is bed rest or something 
        }

        private float CalculatePain()
        {
            if (!_pawn.IsOrganic || _pawn.Dead)
            {
                return 0f;
            }

            var healthMods = GetHealthMods();
            
            if (healthMods == null || !healthMods.Any())
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

            return pain;
        }

        private void BuildBody()
        {
            var corePartInfo = _pawn.species.bodyTemplate.parts.SingleOrDefault(p =>
                p.self.templateName.Equals(_pawn.species.bodyTemplate.corePart.templateName));

            var corePart = new BodyPart(_pawn.species.bodyTemplate, corePartInfo);

            _body = new List<BodyPart> {corePart};

            _body.AddRange(corePart.GetAllChildren());
        }
    }
}
