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
    /// <summary>
    /// The damage template class
    /// </summary>
    /// <seealso cref="Template"/>
    [CreateAssetMenu(menuName = "Templates/DamageTemplate")]
    public class DamageTemplate : Template
    {
        /// <summary>
        /// The worker
        /// </summary>
        private DamageWorker _worker;

        /// <summary>
        /// The damage worker
        /// </summary>
        public Type workerClass = typeof(DamageWorker);

        /// <summary>
        /// The is attack
        /// </summary>
        public bool isAttack;

        /// <summary>
        /// The knocks back
        /// </summary>
        public bool knocksBack = true;

        /// <summary>
        /// The damages hp
        /// </summary>
        public bool damagesHp = true;

        /// <summary>
        /// The interrupt
        /// </summary>
        public bool interrupt = true;

        /// <summary>
        /// The height
        /// </summary>
        public BodyPartHeight height;

        /// <summary>
        /// The depth
        /// </summary>
        public BodyPartDepth depth;

        /// <summary>
        /// The ranged
        /// </summary>
        public bool ranged;

        /// <summary>
        /// The building damage mod
        /// </summary>
        public float buildingDamageMod = 1f;

        /// <summary>
        /// The plant damage mod
        /// </summary>
        public float plantDamageMod = 1f;
        
        //todo sounds

        /// <summary>
        /// The damage types
        /// </summary>
        public DamageType[] damageTypes;

        /// <summary>
        /// The death message
        /// </summary>
        public string deathMessage = "{0} has been killed.";
        
        //todo visual effects

        /// <summary>
        /// The base damage
        /// </summary>
        public int baseDamage = -1;

        /// <summary>
        /// The base armor pen
        /// </summary>
        public float baseArmorPen = -1f;

        /// <summary>
        /// The base stopping power
        /// </summary>
        public float baseStoppingPower;
        
        //todo additional health mods

        //if we decide to use different health mods depending on if hit part
        //is solid or flesh we'll need a method in health utils to get the correct health mod
        /// <summary>
        /// The health mod template
        /// </summary>
        public HealthModTemplate healthModTemplate; 

        /// <summary>
        /// The explodes
        /// </summary>
        public bool explodes;

        /// <summary>
        /// The explosion heat per tile
        /// </summary>
        public float explosionHeatPerTile;
        
        //todo explosion sound

        /// <summary>
        /// The hit multiple targets curve
        /// </summary>
        public AnimationCurve hitMultipleTargetsCurve;
        
        //todo maybe stun chance and duration

        /// <summary>
        /// Gets the value of the worker
        /// </summary>
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
