using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.World.Pawns;
using Assets.Scripts.World.Pawns.BodyPartTags;
using Assets.Scripts.World.Pawns.Health;
using UnityEngine;
using World.Pawns.Health.HealthFunctions;
using World.Pawns.Health.HealthModifiers;

namespace World.Pawns.Health
{
    public class Health
    {
        private Pawn _pawn;

        private HealthState _healthState = HealthState.Mobile;

        //todo health mods -- might be too slow to get mods from body parts for health summary
        //downside is we have to add and remove from two places if we do this so lets see what performance is like

        private FunctionsHandler _functions;

        //todo health summary calculator

        private List<BodyPart> _body;

        public HealthState State => _healthState;

        public bool Downed => _healthState == HealthState.Down;

        public bool Dead => _healthState == HealthState.Dead;

        public float maxLethalDamage = 150f;

        public float painShockThreshold = 0.8f; //todo define in pawn's stats

        public bool CanWakeUp => _functions.canWakeUp;

        public Health(Pawn pawn)
        {
            _pawn = pawn;

            BuildBody();

            _functions = new FunctionsHandler(_pawn);

            //todo set all the other properties
        }

        public List<BodyPart> GetBody()
        {
            return _body;
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

        public float GetLevel(HealthFunctionTemplate function)
        {
            return _functions.GetLevel(function, GetHealthMods());
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
