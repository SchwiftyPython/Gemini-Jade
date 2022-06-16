using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Utilities;
using Assets.Scripts.World.Pawns;
using Assets.Scripts.World.Things;
using UnityEngine;
using Utilities;
using World.Pawns.Health.HealthModifierComponents;
using World.Pawns.Health.HealthModifierComponents.HealthModCompProperties;
using World.Things;

namespace World.Pawns.Health.HealthModifiers
{
    public class HealthMod 
    {
        private const int HealthModAdderInterval = 1000;
        
        private int _intervalCheckCounter;
        
        public List<HealthModComp> comps = new List<HealthModComp>();

        public HealthModTemplate template;

        public int durationTicks; 

        public BodyPart part; 

        private ThingTemplate _source; //source of health mod like Assault Rifle

        private float _severity;

        public bool painless;

        public bool visible;

        public Pawn pawn;

        //todo need to look at all that stage label stuff

        public virtual string LabelBase => template.label.CapitalizeFirst();

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

        public virtual float BleedRate => 0f;

        public bool Bleeding => BleedRate > 1E-05f;

        public bool HasComps => template.comps != null && template.comps.Any();

        //todo pain stuff

        public virtual float PainOffset => 0f; //todo base on current stage's offset if it causes pain
        
        public virtual float PainFactor => 0f; //todo base on current stage's offset if it causes pain

        //todo capacity modifiers

        public virtual float SummaryHealthPercentImpact => 0f;

        //todo tend priority

        //todo color 

        //todo replace _severity with Severity

        public BodyPart Part
        {
            get => part;
            set
            {
                if (pawn == null && part != null)
                {
                    Debug.LogError("HealthMod: Cannot set Part without setting pawn first.");
                }
                else
                {
                    part = value;
                }
            }
        }

        public virtual bool NeedsTending()
        {
            if (!template.tendable || Severity <= 0f || !visible) //todo is perm or immune
            {
                return false;
            }
            
            //todo check tend duration

            return true;
        }

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

        public virtual void Alert_PawnDied()
        {
            foreach (var comp in comps)
            {
                comp.Alert_PawnDied();
            }
        }

        public virtual void Alert_PawnKilled()
        {
            foreach (var comp in comps)
            {
                comp.Alert_PawnKilled();
            }
        }
        
        public virtual void Alert_PawnPostApplyDamage() //todo args DamageInfo dinfo, float totalDamageDealt
        {
            foreach (var comp in comps)
            {
                comp.Alert_PawnPostApplyDamage();
            }
        }
        
        public virtual void Alert_PawnUsedVerb() //todo args Verb verb, LocalTargetInfo target
        {
            foreach (var comp in comps)
            {
                comp.Alert_PawnUsedVerb();
            }
        }
        
        public virtual void Alert_EntropyGained(float baseAmount, float finalAmount, Thing source = null)
        {
            foreach (var comp in comps)
            {
                comp.Alert_EntropyGained(baseAmount, finalAmount, source);
            }
        }
        
        public virtual void Alert_ImplantUsed(string violationSourceName, float detectionChance, int violationSourceLevel = -1)
        {
            foreach (var comp in comps)
            {
                comp.Alert_ImplantUsed(violationSourceName, detectionChance, violationSourceLevel);
            }
        }
        
        public virtual void ModifyChemicalEffect() //todo chemical template and effect amount
        {
            foreach (var comp in comps)
            {
                comp.ModifyChemicalEffect();
            }
        }

        public virtual void PostMake()
        {
        }

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

        public virtual void Tend(float quality, float maxQuality)
        {
            foreach (var comp in comps)
            {
                comp.Tend(quality, maxQuality);
            }
        }
        
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

        public override string ToString()
        {
            return "(" + template.templateName + ((part != null) ? (" " + part.Label) : "") +
                   " ticksSinceCreation=" + durationTicks + ")";
        }
    }
}
