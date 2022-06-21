using System;
using Assets.Scripts.World;
using UnityEngine;
using World.Pawns.Health.DamageTypes;
using World.Pawns.Health.DamageWorkers;
using World.Pawns.Health.HealthModifiers;

namespace World.Pawns.Health.DamageTemplates
{
    [CreateAssetMenu(menuName = "Templates/DamageTemplate")]
    public class DamageTemplate : Template
    {
        private DamageWorker _worker;

        public Type workerClass = typeof(DamageWorker);

        public bool isAttack;

        public bool knocksBack = true;

        public bool damageHp = true;

        public bool interrupt = true;

        public bool ranged;

        public float buildingDamageMod = 1f;

        public float plantDamageMod = 1f;
        
        //todo sounds

        public DamageType[] damageTypes;

        public string deathMessage = "{0} has been killed.";
        
        //todo visual effects

        public int baseDamage = -1;

        public float baseArmorPen = -1f;

        public float baseStoppingPower;
        
        //todo additional health mods

        public HealthModTemplate healthModTemplate;

        public bool explodes;

        public float explosionHeatPerTile;
        
        //todo explosion sound

        public AnimationCurve hitMultipleTargetsCurve;
        
        //todo maybe stun chance and duration

        public DamageWorker Worker
        {
            get
            {
                if (_worker == null)
                {
                    _worker = (DamageWorker)Activator.CreateInstance(workerClass);
                    
                    //todo worker template = this
                }

                return _worker;
            }
        }
    }
}
