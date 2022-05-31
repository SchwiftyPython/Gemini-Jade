using System.Linq;
using Assets.Scripts.Utilities;
using Assets.Scripts.World.Pawns;
using Assets.Scripts.World.Things;
using UnityEngine;
using Utilities;

namespace World.Pawns.Health.HealthModifiers
{
    public class HealthMod 
    {
        private const int HealthModAdderInterval = 1000;
        
        private int _intervalCheckCounter;

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

        public virtual bool ShouldRemove => Severity <= 0f;

        public virtual float BleedRate => 0f;

        public bool Bleeding => BleedRate > 1E-05f;

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
            return true;
        }

        public virtual void Notify_PawnDied()
        {
        }

        public virtual void Notify_PawnKilled()
        {
        }

        public virtual void PostMake()
        {
        }

        public virtual void PostAdd()
        {
        }

        public virtual void PostRemove()
        {
        }

        public virtual void PostTick()
        {
        }

        public override string ToString()
        {
            return "(" + template.templateName + ((part != null) ? (" " + part.Label) : "") +
                   " ticksSinceCreation=" + durationTicks + ")";
        }
    }
}
