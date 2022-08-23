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
    /// <summary>
    /// The health class
    /// </summary>
    public class Health
    {
        /// <summary>
        /// The tick interval
        /// </summary>
        private const int TickInterval = 1000; //all of the intervals are in HealthTuning. Different intervals for vomit, death, health mod update.
        
        /// <summary>
        /// The pawn
        /// </summary>
        private Pawn _pawn;

        /// <summary>
        /// The mobile
        /// </summary>
        private HealthState _healthState = HealthState.Mobile;

        /// <summary>
        /// The health mod collection
        /// </summary>
        private HealthModCollection _healthModCollection;

        /// <summary>
        /// The functions
        /// </summary>
        private FunctionsHandler _functions;

        /// <summary>
        /// The functions repo
        /// </summary>
        private HealthFunctionRepo _functionsRepo;

        //todo health summary calculator

        /// <summary>
        /// The body
        /// </summary>
        private List<BodyPart> _body;

        /// <summary>
        /// The interval check counter
        /// </summary>
        private int _intervalCheckCounter;

        /// <summary>
        /// Gets the value of the state
        /// </summary>
        public HealthState State => _healthState;

        /// <summary>
        /// Gets the value of the downed
        /// </summary>
        public bool Downed => _healthState == HealthState.Down;

        /// <summary>
        /// Gets the value of the dead
        /// </summary>
        public bool Dead => _healthState == HealthState.Dead;

        /// <summary>
        /// The max lethal damage
        /// </summary>
        public float maxLethalDamage = 150f;

        /// <summary>
        /// The pain shock threshold
        /// </summary>
        public float painShockThreshold = 0.8f; //todo define in pawn's stats

        /// <summary>
        /// Gets the value of the can wake up
        /// </summary>
        public bool CanWakeUp => _functions.CanWakeUp;

        /// <summary>
        /// Initializes a new instance of the <see cref="Health"/> class
        /// </summary>
        /// <param name="pawn">The pawn</param>
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

        /// <summary>
        /// Gets the body
        /// </summary>
        /// <returns>The body</returns>
        public List<BodyPart> GetBody()
        {
            return _body;
        }

        /// <summary>
        /// Gets the existing parts
        /// </summary>
        /// <returns>A list of body part</returns>
        public List<BodyPart> GetExistingParts()
        {
            return _healthModCollection.GetExistingParts();
        }

        /// <summary>
        /// Gets the body parts with tag using the specified tag
        /// </summary>
        /// <param name="tag">The tag</param>
        /// <returns>The tagged parts</returns>
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

        /// <summary>
        /// Describes whether this instance has parts with tag
        /// </summary>
        /// <param name="tag">The tag</param>
        /// <returns>The bool</returns>
        public bool HasPartsWithTag(BodyPartTagTemplate tag)
        {
            return _pawn.species.bodyTemplate.HasPartsWithTag(tag);
        }
        
        /// <summary>
        /// Gets the core body part
        /// </summary>
        /// <returns>The body part</returns>
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
        
        /// <summary>
        /// Gets the random body part
        /// </summary>
        /// <returns>The body part</returns>
        public BodyPart GetRandomBodyPart()
        {
            return _healthModCollection.GetRandomExistingPart();
        }
        
        /// <summary>
        /// Gets the random body part using the specified height
        /// </summary>
        /// <param name="height">The height</param>
        /// <param name="depth">The depth</param>
        /// <param name="parent">The parent</param>
        /// <returns>The body part</returns>
        public BodyPart GetRandomBodyPart(BodyPartHeight height, BodyPartDepth depth, BodyPart parent = null)
        {
            return _healthModCollection.GetRandomExistingPart(height, depth, parent);
        }

        /// <summary>
        /// Gets the level using the specified function
        /// </summary>
        /// <param name="function">The function</param>
        /// <returns>The float</returns>
        public float GetLevel(HealthFunctionTemplate function)
        {
            return _functions.GetLevel(function, GetHealthMods());
        }

        /// <summary>
        /// Adds the health mod using the specified health mod to add
        /// </summary>
        /// <param name="healthModToAdd">The health mod to add</param>
        /// <param name="bodyPart">The body part</param>
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

        /// <summary>
        /// Removes the health mod using the specified health mod
        /// </summary>
        /// <param name="healthMod">The health mod</param>
        public void RemoveHealthMod(HealthMod healthMod)
        {
            _healthModCollection.RemoveHealthMod(healthMod);
            
            CheckForHealthStateChange(null);
        }

        /// <summary>
        /// Gets the health mods
        /// </summary>
        /// <returns>A list of health mod</returns>
        public List<HealthMod> GetHealthMods()
        {
            return _healthModCollection.healthMods;
        }

        /// <summary>
        /// Gets the first health mod of using the specified health mod template
        /// </summary>
        /// <param name="healthModTemplate">The health mod template</param>
        /// <param name="mustBeVisible">The must be visible</param>
        /// <returns>The health mod</returns>
        public HealthMod GetFirstHealthModOf(HealthModTemplate healthModTemplate, bool mustBeVisible = false)
        {
            return _healthModCollection.GetFirstHealthModOf(healthModTemplate, mustBeVisible);
        }

        /// <summary>
        /// Describes whether this instance body part is missing
        /// </summary>
        /// <param name="part">The part</param>
        /// <returns>The bool</returns>
        public bool BodyPartIsMissing(BodyPart part)
        {
            return _healthModCollection.BodyPartIsMissing(part);
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
            return _healthModCollection.HasHealthMod(healthModTemplate, bodyPart, mustBeVisible);
        }

        /// <summary>
        /// Describes whether this instance has health mod
        /// </summary>
        /// <param name="healthModTemplate">The health mod template</param>
        /// <param name="mustBeVisible">The must be visible</param>
        /// <returns>The bool</returns>
        public bool HasHealthMod(HealthModTemplate healthModTemplate, bool mustBeVisible = false)
        {
            return _healthModCollection.HasHealthMod(healthModTemplate, mustBeVisible);
        }
        
        /// <summary>
        /// Gets the part health using the specified body part
        /// </summary>
        /// <param name="bodyPart">The body part</param>
        /// <returns>The float</returns>
        public float GetPartHealth(BodyPart bodyPart)
        {
            return _healthModCollection.GetBodyPartHealth(bodyPart);
        }

        /// <summary>
        /// Gets the function values
        /// </summary>
        /// <returns>A dictionary of health function template and float</returns>
        public IDictionary<HealthFunctionTemplate, float> GetFunctionValues()
        {
            return _functions.GetFunctionLevels();
        }

        /// <summary>
        /// Gets the pain total
        /// </summary>
        /// <returns>The float</returns>
        public float GetPainTotal()
        {
            return _healthModCollection.TotalPain;
        }

        /// <summary>
        /// Gets the bleed rate total
        /// </summary>
        /// <returns>The float</returns>
        public float GetBleedRateTotal()
        {
            return _healthModCollection.TotalBleedRate;
        }

        /// <summary>
        /// Ticks this instance
        /// </summary>
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

        /// <summary>
        /// Processes the health mod adders
        /// </summary>
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

        /// <summary>
        /// Checks the for health state change using the specified health mod
        /// </summary>
        /// <param name="healthMod">The health mod</param>
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
        
        /// <summary>
        /// Kills the pawn
        /// </summary>
        public void KillPawn()
        {
            if (Dead)
            {
                return;
            }

            _healthState = HealthState.Dead;
        }

        /// <summary>
        /// Describes whether this instance should be downed
        /// </summary>
        /// <returns>The bool</returns>
        private bool ShouldBeDowned()
        {
            if (_functions.CanWakeUp) //todo !InPainShock
            {
                return !_functions.CapableOf(_functionsRepo.moving);
            }

            return true;
        }

        /// <summary>
        /// Describes whether this instance should be dead
        /// </summary>
        /// <returns>The bool</returns>
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

        /// <summary>
        /// Describes whether this instance should be dead from critical function
        /// </summary>
        /// <returns>The bool</returns>
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

        /// <summary>
        /// Describes whether this instance should be dead from lethal damage
        /// </summary>
        /// <returns>The bool</returns>
        private bool ShouldBeDeadFromLethalDamage()
        {
            //todo add up severity of all injury health mods
            
            //todo if severity >= lethal damage threshold then return true

            return false;
        }

        /// <summary>
        /// Downs the pawn
        /// </summary>
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

        /// <summary>
        /// Uns the down pawn
        /// </summary>
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

        /// <summary>
        /// Builds the body
        /// </summary>
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
