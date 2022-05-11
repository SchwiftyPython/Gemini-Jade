using System;
using System.Collections.Generic;
using Assets.Scripts.World.Pawns;
using Assets.Scripts.World.Pawns.Species;
using Time;
using World.Pawns.Health;
using World.Things;
using Object = UnityEngine.Object;

namespace World.Pawns
{
    public class Pawn : Thing
    {
        public SpeciesTemplate species;

        private string _name;

        public Health.Health health;

        public bool Dead => health.Dead;

        public bool CanWakeUp => health.CanWakeUp;

        public bool IsOrganic => species.fleshType != FleshType.Machine;

        public bool IsHumanoid => species.intellect >= Intellect.Humanoid;

        public bool ToolUser => species.intellect >= Intellect.ToolUser;

        public bool IsAnimal => !ToolUser && IsOrganic;

        public Pawn(SpeciesTemplate speciesTemplate)
        {
            species = speciesTemplate;

            _name = "Testy McTestes";

            health = new Health.Health(this);
        }

        public List<BodyPart> GetBody()
        {
            return health.GetBody();
        }

        public void Die()
        {
            //todo a ton of shit happens here. We'll revisit often.
            
            health.KillPawn();
        }

        public override void Tick()
        {
            base.Tick();

            var tickController = Object.FindObjectOfType<TickController>();

            //not sure if tick intervals are the same everywhere. May have to define per class using it.
            if (tickController.NumTicks % TickController.RareTickInterval == 0) 
            {
                TickRare();
            }
            
            //todo check if not suspended
            
            //todo bunch of trackers and whatnot ticking

            health.Tick();

            if (!Dead)
            {
                //todo mind state tick
                
                //todo carry tracker tick
            }
            
            //todo bunch of trackers and whatnot ticking
        }

        public override void TickRare()
        {
            base.TickRare();
            
            //todo 
        }
    }
}
