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
    }
}
