using System;
using Assets.Scripts.World.Things;

namespace World.Things
{
    public class Thing
    {
        public ThingTemplate template;
        
        public bool Destroyed;

        public virtual void Tick()
        {
            //todo
        }
        
        public virtual void TickRare()
        {
            //todo
        }
        
        public virtual void TickLong()
        {
            //todo
        }
    }
}
