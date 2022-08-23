using World.Pawns.Health.DamageTemplates;
using World.Things;

namespace World.Pawns.Actions
{
    /// <summary>
    /// The action class
    /// </summary>
    public class Action
    {
        /// <summary>
        /// The ticks til next action
        /// </summary>
        protected int ticksTilNextAction;
        
        /// <summary>
        /// The name
        /// </summary>
        public string name;

        /// <summary>
        /// The user
        /// </summary>
        public Thing user;
    
        /// <summary>
        /// The target
        /// </summary>
        public Thing target;
    
        /// <summary>
        /// The damage template
        /// </summary>
        public DamageTemplate damageTemplate;
    
        /// <summary>
        /// The min value
        /// </summary>
        public int lastActionTick = int.MinValue;

        /// <summary>
        /// The equipment source
        /// </summary>
        public Thing equipmentSource;

        /// <summary>
        /// Describes whether this instance try action
        /// </summary>
        /// <returns>The bool</returns>
        public virtual bool TryAction() //todo set back to protected once we implement Action Tick
        {
            return true;
        }
        
        /// <summary>
        /// Describes whether this instance can hit
        /// </summary>
        /// <param name="targetThing">The target thing</param>
        /// <returns>The bool</returns>
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

        /// <summary>
        /// Describes whether this instance can hit target from
        /// </summary>
        /// <returns>The bool</returns>
        public virtual bool CanHitTargetFrom() //todo Coord and target
        {
            return true;
        }
    }
}
