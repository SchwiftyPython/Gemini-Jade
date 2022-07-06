using System;
using Assets.Scripts.World;
using Assets.Scripts.World.Pawns.BodyPartDepth;
using Assets.Scripts.World.Pawns.BodyPartHeight;
using UnityEngine;
using UnityEngine.Serialization;
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

        public bool damagesHp = true;

        public bool interrupt = true;

        public BodyPartHeight height;

        public BodyPartDepth depth;

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

        //if we decide to use different health mods depending on if hit part
        //is solid or flesh we'll need a method in health utils to get the correct health mod
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
