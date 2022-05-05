using System;
using Assets.Scripts.World.Things;

namespace World.Things
{
    public class Thing
    {
        public ThingTemplate template;
        
        public bool Destroyed;

        public void Tick()
        {
            throw new NotImplementedException();
        }
        
        public void TickRare()
        {
            throw new NotImplementedException();
        }
        
        public void TickLong()
        {
            throw new NotImplementedException();
        }
    }
}
