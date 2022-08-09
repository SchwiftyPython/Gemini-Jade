using GoRogue;
using UnityEngine;
using World.Pawns.Health.DamageWorkers;

namespace World.Things
{
    public class Thing : BaseObject
    {
        private int _hitPoints;
        
        public ThingTemplate template;
        
        public bool Destroyed;

        public int id = -1;
        
        public bool spawned;

        protected Thing(Vector3 position, MapLayer layer, bool isStatic, bool isWalkable, bool isTransparent) : base(position,
            layer, isStatic, isWalkable, isTransparent)
        {
        }
        
        protected Thing(MapLayer layer, bool isStatic, bool isWalkable, bool isTransparent) : base(layer, isStatic, isWalkable, isTransparent)
        {
        }

        protected Thing()
        {
        }

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

        public DamageResult TakeDamage(DamageInfo damageInfo)
        {
            if (Destroyed)
            {
                return new DamageResult();
            }

            if (damageInfo.Amount <= 0f)
            {
                return new DamageResult();
            }
            
            PreTakeDamage(ref damageInfo, out var absorbed);

            if (absorbed)
            {
                return new DamageResult();
            }

            var damageResult = damageInfo.Apply(this);
            
            //todo notify damage taken

            if (damageInfo.Template.isAttack)
            {
                //todo splatter blood
                
                //todo record damage taken
            }
            
            PostTakeDamage(damageInfo, damageResult.totalDamage);

            return damageResult;
        }
        
        public virtual void PreTakeDamage(ref DamageInfo damageInfo, out bool absorbed)
        {
            absorbed = false;
        }
        
        public virtual void PostTakeDamage(DamageInfo damageInfo, float totalDamageDealt)
        {
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
