using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Utilities;
using Assets.Scripts.World.Pawns;
using UnityEngine;
using World.Pawns.Health.HealthModifierComponents;
using World.Things;

namespace World.Pawns.Health.HealthModifiers
{
    /// <summary>
    /// The health mod class
    /// </summary>
    public class HealthMod 
    {
        /// <summary>
        /// The health mod adder interval
        /// </summary>
        private const int HealthModAdderInterval = 1000;
        
        /// <summary>
        /// The interval check counter
        /// </summary>
        private int _intervalCheckCounter;
        
        /// <summary>
        /// The health mod comp
        /// </summary>
        public List<HealthModComp> comps = new List<HealthModComp>();

        /// <summary>
        /// The template
        /// </summary>
        public HealthModTemplate template;

        /// <summary>
        /// The duration ticks
        /// </summary>
        public int durationTicks; 

        /// <summary>
        /// The part
        /// </summary>
        private BodyPart _part; 

        /// <summary>
        /// The source
        /// </summary>
        private ThingTemplate _source; //source of health mod like Assault Rifle

        /// <summary>
        /// The severity
        /// </summary>
        private float _severity;

        /// <summary>
        /// The painless
        /// </summary>
        public bool painless;

        /// <summary>
        /// The visible
        /// </summary>
        public bool visible;

        /// <summary>
        /// The pawn
        /// </summary>
        public Pawn pawn;

        //todo need to look at all that stage label stuff

        /// <summary>
        /// Gets the value of the label base
        /// </summary>
        public virtual string LabelBase => template.label.CapitalizeFirst();

        /// <summary>
        /// Gets the value of the severity label
        /// </summary>
        public virtual string SeverityLabel
        {
            get
            {
                if (!(template.lethalSeverity <= 0f))
                {
                    return (_severity / template.lethalSeverity).ToStringPercent();
                }
                return null;
            }
        }

        /// <summary>
        /// Gets the value of the current stage
        /// </summary>
        public virtual HealthModStage CurrentStage
        {
            get
            {
                if (template.stages != null && template.stages.Any())
                {
                    return template.stages[CurrentStageIndex];
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the value of the current stage index
        /// </summary>
        public virtual int CurrentStageIndex
        {
            get
            {
                if (template.stages == null)
                {
                    return 0;
                }

                var stages = template.stages;

                var severity = Severity;

                for (var index = stages.Count - 1; index >= 0; index--)
                {
                    if (severity >= stages[index].minSeverity)
                    {
                        return index;
                    }
                }

                return 0;
            }
        }

        /// <summary>
        /// Gets or sets the value of the severity
        /// </summary>
        public virtual float Severity
        {
            get => _severity;
            set
            {
                var severityIsLethal = false;

                if (template.lethalSeverity > 0f)
                {
                    if (value >= template.lethalSeverity)
                    {
                        value = template.lethalSeverity;
                        severityIsLethal = true;
                    }
                }

                var isInjury = this is Injury && value > _severity &&
                               Mathf.RoundToInt(value) != Mathf.RoundToInt(_severity);

                var currentStageIndex = CurrentStageIndex;

                _severity = Mathf.Clamp(value, template.minSeverity, template.maxSeverity);

                if ((CurrentStageIndex != currentStageIndex || severityIsLethal || isInjury) && pawn.health.HasHealthMod(template)) //they check for the health mod directly so might need an overload
                {
                   pawn.health.CheckForHealthStateChange(this);
                    
                    //todo mood and thoughts
                }
            }
        }

        /// <summary>
        /// Gets the value of the should remove
        /// </summary>
        public virtual bool ShouldRemove
        {
            get
            {
                if (comps == null)
                {
                    return Severity <= 0f;
                }

                if (!comps.Any())
                {
                    return Severity <= 0f;
                }

                foreach (var comp in comps)
                {
                    if (comp.ShouldRemove)
                    {
                        return true;
                    }
                }

                return Severity <= 0f;
            }
        }

        /// <summary>
        /// Gets the value of the visible
        /// </summary>
        public virtual bool Visible
        {
            get
            {
                if (comps != null)
                {
                    foreach (var comp in comps)
                    {
                        if (comp.DisallowVisible())
                        {
                            return false;
                        }
                    }
                }

                if (visible)
                {
                    return true;
                }

                if (CurrentStage != null)
                {
                    return CurrentStage.visible;
                }

                return true;
            }
        }

        /// <summary>
        /// Gets the value of the bleed rate
        /// </summary>
        public virtual float BleedRate => 0f;

        /// <summary>
        /// Gets the value of the bleeding
        /// </summary>
        public bool Bleeding => BleedRate > 1E-05f;

        /// <summary>
        /// Gets the value of the has comps
        /// </summary>
        public bool HasComps => template.comps != null && template.comps.Any();

        //todo pain stuff

        /// <summary>
        /// Gets the value of the pain offset
        /// </summary>
        public virtual float PainOffset => 0f; //todo base on current stage's offset if it causes pain
        
        /// <summary>
        /// Gets the value of the pain factor
        /// </summary>
        public virtual float PainFactor => 0f; //todo base on current stage's offset if it causes pain

        //todo capacity modifiers

        /// <summary>
        /// Gets the value of the summary health percent impact
        /// </summary>
        public virtual float SummaryHealthPercentImpact => 0f;

        //todo tend priority

        //todo color 

        //todo replace _severity with Severity

        /// <summary>
        /// Gets or sets the value of the part
        /// </summary>
        public BodyPart Part
        {
            get => _part;
            set
            {
                if (pawn == null && _part != null)
                {
                    Debug.LogError("HealthMod: Cannot set Part without setting pawn first.");
                }
                else
                {
                    _part = value;
                }
            }
        }

        /// <summary>
        /// Describes whether this instance needs tending
        /// </summary>
        /// <returns>The bool</returns>
        public virtual bool NeedsTending()
        {
            if (!template.tendable || Severity <= 0f || !visible) //todo is perm or immune
            {
                return false;
            }
            
            //todo check tend duration

            return true;
        }

        /// <summary>
        /// Ticks this instance
        /// </summary>
        public virtual void Tick()
        {
            durationTicks++;

            if (_intervalCheckCounter > HealthModAdderInterval)
            {
                _intervalCheckCounter = 0;
                
                if (template.healthModAdders != null)
                {
                    foreach (var healthModAdder in template.healthModAdders)
                    {
                        healthModAdder.OnIntervalPassed(pawn, this);
                    }
                }
                
                var currentStage = CurrentStage;

                if (currentStage == null)
                {
                    return;
                }

                if (currentStage.healthModAdders != null) //magic number
                {
                    foreach (var healthModAdder in currentStage.healthModAdders)
                    {
                        healthModAdder.OnIntervalPassed(pawn, this);
                    }
                }
            }
            else
            {
                _intervalCheckCounter++;
            }

            //todo
        }

        //todo try mental break

        /// <summary>
        /// Describes whether this instance cause death now
        /// </summary>
        /// <returns>The bool</returns>
        public virtual bool CauseDeathNow()
        {
            if (template.lethalSeverity >= 0f)
            {
                bool lethal = _severity >= template.lethalSeverity;
                if (lethal)
                {
                    Debug.Log("CauseOfDeath: lethal severity exceeded " + _severity + " >= " +
                              template.lethalSeverity);
                }

                return lethal;
            }

            return false;
        }

        /// <summary>
        /// Describes whether this instance try merge with
        /// </summary>
        /// <param name="otherHealthMod">The other health mod</param>
        /// <returns>The bool</returns>
        public virtual bool TryMergeWith(HealthMod otherHealthMod)
        {
            if (otherHealthMod == null)
            {
                return false;
            }

            if (otherHealthMod.template != template)
            {
                return false;
            }

            if (otherHealthMod.Part != Part)
            {
                return false;
            }
            
            if (!template.canMerge) 
            {
                return false;
            }

            Severity += otherHealthMod.Severity;
            durationTicks = 0;

            foreach (var comp in comps)
            {
                comp.PostMerge(otherHealthMod);
            }
            
            return true;
        }

        /// <summary>
        /// Alerts the pawn died
        /// </summary>
        public virtual void Alert_PawnDied()
        {
            foreach (var comp in comps)
            {
                comp.Alert_PawnDied();
            }
        }

        /// <summary>
        /// Alerts the pawn killed
        /// </summary>
        public virtual void Alert_PawnKilled()
        {
            foreach (var comp in comps)
            {
                comp.Alert_PawnKilled();
            }
        }
        
        /// <summary>
        /// Alerts the pawn post apply damage
        /// </summary>
        public virtual void Alert_PawnPostApplyDamage() //todo args DamageInfo dinfo, float totalDamageDealt
        {
            foreach (var comp in comps)
            {
                comp.Alert_PawnPostApplyDamage();
            }
        }
        
        /// <summary>
        /// Alerts the pawn used verb
        /// </summary>
        public virtual void Alert_PawnUsedVerb() //todo args Verb verb, LocalTargetInfo target
        {
            foreach (var comp in comps)
            {
                comp.Alert_PawnUsedVerb();
            }
        }
        
        /// <summary>
        /// Alerts the entropy gained using the specified base amount
        /// </summary>
        /// <param name="baseAmount">The base amount</param>
        /// <param name="finalAmount">The final amount</param>
        /// <param name="source">The source</param>
        public virtual void Alert_EntropyGained(float baseAmount, float finalAmount, Thing source = null)
        {
            foreach (var comp in comps)
            {
                comp.Alert_EntropyGained(baseAmount, finalAmount, source);
            }
        }
        
        /// <summary>
        /// Alerts the implant used using the specified violation source name
        /// </summary>
        /// <param name="violationSourceName">The violation source name</param>
        /// <param name="detectionChance">The detection chance</param>
        /// <param name="violationSourceLevel">The violation source level</param>
        public virtual void Alert_ImplantUsed(string violationSourceName, float detectionChance, int violationSourceLevel = -1)
        {
            foreach (var comp in comps)
            {
                comp.Alert_ImplantUsed(violationSourceName, detectionChance, violationSourceLevel);
            }
        }
        
        /// <summary>
        /// Modifies the chemical effect
        /// </summary>
        public virtual void ModifyChemicalEffect() //todo chemical template and effect amount
        {
            foreach (var comp in comps)
            {
                comp.ModifyChemicalEffect();
            }
        }

        /// <summary>
        /// Posts the make
        /// </summary>
        public virtual void PostMake()
        {
        }

        /// <summary>
        /// Posts the add
        /// </summary>
        public virtual void PostAdd() //todo damage info as arg
        {
            //todo check if template affects needs

            if (comps == null)
            {
                return;
            }

            foreach (var comp in comps)
            {
                comp.PostAdd();
            }

        }

        /// <summary>
        /// Posts the remove
        /// </summary>
        public virtual void PostRemove()
        {
            //todo check if template affects needs

            if (comps == null)
            {
                return;
            }

            foreach (var comp in comps)
            {
                comp.PostRemove();
            }
        }

        /// <summary>
        /// Posts the tick
        /// </summary>
        public virtual void PostTick()
        {
            if (comps != null)
            {
                var severityAdjustment = 0f;

                foreach (var comp in comps)
                {
                    comp.PostTick(ref severityAdjustment);
                }

                Severity += severityAdjustment;
            }
        }

        /// <summary>
        /// Tends the quality
        /// </summary>
        /// <param name="quality">The quality</param>
        /// <param name="maxQuality">The max quality</param>
        public virtual void Tend(float quality, float maxQuality)
        {
            foreach (var comp in comps)
            {
                comp.Tend(quality, maxQuality);
            }
        }
        
        /// <summary>
        /// Initializes the comps
        /// </summary>
        private void InitializeComps()
        {
            if (template.comps == null)
            {
                return;
            }
            
            comps = new List<HealthModComp>();
            
            foreach (var compProp in template.comps)
            {
                HealthModComp healthModComp = null;
                
                try
                {
                    healthModComp = (HealthModComp)Activator.CreateInstance(compProp.compClass);
                    
                    healthModComp.props = compProp;
                    
                    healthModComp.parent = this;
                    
                    comps.Add(healthModComp);
                }
                catch (Exception ex)
                {
                    Debug.LogError("Could not instantiate or initialize a HealthModComp: " + ex);
                    
                    comps.Remove(healthModComp);
                }
            }
        }

        /// <summary>
        /// Returns the string
        /// </summary>
        /// <returns>The string</returns>
        public override string ToString()
        {
            return "(" + template.templateName + ((_part != null) ? (" " + _part.Label) : "") +
                   " ticksSinceCreation=" + durationTicks + ")";
        }
    }
}
