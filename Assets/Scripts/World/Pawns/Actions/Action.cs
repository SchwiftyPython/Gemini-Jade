using World.Pawns.Health.DamageTemplates;
using World.Things;

namespace World.Pawns.Actions
{
    public class Action
    {
        protected int ticksTilNextAction;
        
        public string name;

        public Thing user;
    
        public Thing target;
    
        public DamageTemplate damageTemplate;
    
        public int lastActionTick = int.MinValue;

        public Thing equipmentSource;

        public virtual bool TryAction() //todo set back to protected once we implement Action Tick
        {
            return true;
        }
        
        public virtual bool CanHit(Thing targetThing)
        {
            if (user == null) //todo or user not spawned
            {
                return false;
            }

            if (user == targetThing)
            {
                return false;
            }

            return CanHitTargetFrom();
        }

        public virtual bool CanHitTargetFrom() //todo Coord and target
        {
            return true;
        }
    }
}
