using Assets.Scripts.Utilities;
using Assets.Scripts.World.Things;
using UnityEngine;

namespace Assets.Scripts.World.Pawns.Health.HealthModifiers
{
    public class HealthMod 
    {
        private HealthModTemplate _template;

        private int _durationTicks; 

        private BodyPart _part; 

        private ThingTemplate _source; //source of health mod like Assault Rifle

        private float _severity;

        private bool _painless;

        private bool _visible;

        private Pawn _pawn;

        //todo need to look at all that stage label stuff

        public virtual string SeverityLabel
        {
            get
            {
                if (!(_template.lethalSeverity <= 0f))
                {
                    return (_severity / _template.lethalSeverity).ToStringPercent();
                }
                return null;
            }
        }

        public virtual bool ShouldRemove => _severity <= 0f;

        public virtual float BleedRate => 0f;

        public bool Bleeding => BleedRate > 1E-05f;

        //todo pain stuff 

        //todo capacity modifiers

        public virtual float SummaryHealthPercentImpact => 0f;

        //todo tend priority

        //todo color 

        //todo replace _severity with Severity

        public BodyPart Part
        {
            get => _part;
            set
            {
                if (_pawn == null && _part != null)
                {
                    Debug.LogError("HealthMod: Cannot set Part without setting pawn first.");
                }
                else
                {
                    _part = value;
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
            if (_template.lethalSeverity >= 0f)
            {
                bool lethal = _severity >= _template.lethalSeverity;
                if (lethal)
                {
                    Debug.Log("CauseOfDeath: lethal severity exceeded " + _severity + " >= " +
                              _template.lethalSeverity);
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
            return "(" + _template.templateName + ((_part != null) ? (" " + _part.Label) : "") +
                   " ticksSinceCreation=" + _durationTicks + ")";
        }
    }
}
