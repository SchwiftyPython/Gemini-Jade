using System;
using Assets.Scripts.World.Things;
using Random = UnityEngine.Random;

namespace World.Things
{
    public class Thing
    {
        public ThingTemplate template;
        
        public bool Destroyed;

        public int id = -1;

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

        public void GenerateId()
        {
            //todo stick into thing maker and ensure unique ids

            id = 0;
        }
    }
}
