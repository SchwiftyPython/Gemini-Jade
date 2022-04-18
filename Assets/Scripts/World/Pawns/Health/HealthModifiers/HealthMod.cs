using Assets.Scripts.Utilities;
using Assets.Scripts.World.Things;
using UnityEngine;

namespace Assets.Scripts.World.Pawns.Health.HealthModifiers
{
    public class HealthMod 
    {
        public HealthModTemplate template;

        private int _durationTicks; 

        public BodyPart part; 

        private ThingTemplate _source; //source of health mod like Assault Rifle

        private float _severity;

        public bool painless;

        private bool _visible;

        public Pawn pawn;

        //todo need to look at all that stage label stuff

        public virtual string LabelBase => template.label;

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

        public virtual bool ShouldRemove => _severity <= 0f;

        public virtual float BleedRate => 0f;

        public bool Bleeding => BleedRate > 1E-05f;

        //todo pain stuff

        public virtual float PainOffset => 0f; //todo base on current stage's offset if it causes pain

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

        //todo tendable now

        public virtual void Tick()
        {
            _durationTicks++;

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

        public virtual void Notify_PawnDied()
        {
        }

        public virtual void Notify_PawnKilled()
        {
        }

        public override string ToString()
        {
            return "(" + template.templateName + ((part != null) ? (" " + part.Label) : "") +
                   " ticksSinceCreation=" + _durationTicks + ")";
        }
    }
}
