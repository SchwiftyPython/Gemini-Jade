using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Utilities;
using Assets.Scripts.World.Pawns;
using Assets.Scripts.World.Pawns.BodyPartDepth;
using Assets.Scripts.World.Pawns.BodyPartHeight;
using Assets.Scripts.World.Pawns.BodyPartTags;
using Assets.Scripts.World.Pawns.Health;
using Utilities;
using World.Pawns.Health.HealthFunctions;
using World.Pawns.Health.HealthModifiers;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace World.Pawns.Health
{
    public class Health
    {
        private const int TickInterval = 1000; //all of the intervals are in HealthTuning. Different intervals for vomit, death, health mod update.
        
        private Pawn _pawn;

        private HealthState _healthState = HealthState.Mobile;

        private HealthModCollection _healthModCollection;

        private FunctionsHandler _functions;

        private HealthFunctionRepo _functionsRepo;

        //todo health summary calculator

        private List<BodyPart> _body;

        private int _intervalCheckCounter;

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

            _healthModCollection = new HealthModCollection(_pawn);

            _intervalCheckCounter = 0;
            
            //todo set all the other properties
        }

        public List<BodyPart> GetBody()
        {
            return _body;
        }

        public List<BodyPart> GetExistingParts()
        {
            return _healthModCollection.GetExistingParts();
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
        
        public BodyPart GetRandomBodyPart()
        {
            return _healthModCollection.GetRandomExistingPart();
        }
        
        public BodyPart GetRandomBodyPart(BodyPartHeight height, BodyPartDepth depth, BodyPart parent = null)
        {
            return _healthModCollection.GetRandomExistingPart(height, depth, parent);
        }

        public float GetLevel(HealthFunctionTemplate function)
        {
            return _functions.GetLevel(function, GetHealthMods());
        }

        public void AddHealthMod(HealthMod healthModToAdd, BodyPart bodyPart = null)
        {
            _healthModCollection.AddHealthMod(healthModToAdd, bodyPart);
            
            CheckForHealthStateChange(healthModToAdd);

            if (_pawn.species.healthModAdderSets == null)
            {
                return;
            }

            foreach (var healthModAdderSet in _pawn.species.healthModAdderSets.ToArray())
            {
                foreach (var healthModAdder in healthModAdderSet.healthModAdders.ToArray())
                {
                    healthModAdder.OnHealthModAdded(_pawn, healthModToAdd);
                }
            }
        }

        public void RemoveHealthMod(HealthMod healthMod)
        {
            _healthModCollection.RemoveHealthMod(healthMod);
            
            CheckForHealthStateChange(null);
        }

        public List<HealthMod> GetHealthMods()
        {
            return _healthModCollection.healthMods;
        }

        public HealthMod GetFirstHealthModOf(HealthModTemplate healthModTemplate, bool mustBeVisible = false)
        {
            return _healthModCollection.GetFirstHealthModOf(healthModTemplate, mustBeVisible);
        }

        public bool BodyPartIsMissing(BodyPart part)
        {
            return _healthModCollection.BodyPartIsMissing(part);
        }

        public bool HasHealthMod(HealthModTemplate healthModTemplate, BodyPart bodyPart, bool mustBeVisible = false)
        {
            return _healthModCollection.HasHealthMod(healthModTemplate, bodyPart, mustBeVisible);
        }

        public bool HasHealthMod(HealthModTemplate healthModTemplate, bool mustBeVisible = false)
        {
            return _healthModCollection.HasHealthMod(healthModTemplate, mustBeVisible);
        }
        
        public float GetPartHealth(BodyPart bodyPart)
        {
            return _healthModCollection.GetBodyPartHealth(bodyPart);
        }

        public IDictionary<HealthFunctionTemplate, float> GetFunctionValues()
        {
            return _functions.GetFunctionLevels();
        }

        public float GetPainTotal()
        {
            return _healthModCollection.TotalPain;
        }

        public float GetBleedRateTotal()
        {
            return _healthModCollection.TotalBleedRate;
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
                    RemoveHealthMod(healthMod);
                    
                    healthModsChanged = true;
                }
            }

            if (healthModsChanged)
            {
                //todo notify health mods changed
                //looks like it resets a bunch of caches
                
                CheckForHealthStateChange(null);
            }
            
            //todo immunity handler tick

            if (_intervalCheckCounter > TickInterval) //todo use modulo to check multiple intervals and not have to worry about resetting counter
            {
                if (_pawn.IsOrganic) //todo and pawn needs food == null || not starving
                {
                    
                }
                
                ProcessHealthModAdders();

                _intervalCheckCounter = 0;
            }
            else
            {
                _intervalCheckCounter++;
            }
        }

        public void ProcessHealthModAdders()
        {
            var healthModAdderSets = _pawn.species.healthModAdderSets;

            if (healthModAdderSets != null)
            {
                foreach (var healthModAdderSet in healthModAdderSets.ToArray())
                {
                    var healthModAdders = healthModAdderSet.healthModAdders;

                    foreach (var healthModAdder in healthModAdders.ToArray())
                    {
                        healthModAdder.OnIntervalPassed(_pawn, null);
                        
                        if (_pawn.Dead)
                        {
                            return;
                        }
                    }
                }
            }
            
            //todo trigger on body changed event
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
