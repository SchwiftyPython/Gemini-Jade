using System;
using Assets.Scripts.World.Things;
using World.Pawns.Health.DamageWorkers;
using Random = UnityEngine.Random;

namespace World.Things
{
    public class Thing
    {
        private int _hitPoints;
        
        public ThingTemplate template;
        
        public bool Destroyed;

        public int id = -1;

        public virtual int HitPoints
        {
            get
            {
                return _hitPoints;
            }
            set
            {
                _hitPoints = value;
            }
        }

        public virtual void Kill(DamageInfo damageInfo)
        {
            Destroy();
        }

        public virtual void Destroy() //todo destroy mode
        {
            //todo
        }

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
