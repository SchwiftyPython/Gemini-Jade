using Assets.Scripts.World;
using UnityEngine;
using World.Pawns.Actions;

namespace World.Things.Parts
{
    
    /// <summary>
    /// The part template class
    /// </summary>
    /// <seealso cref="Template"/>
    [CreateAssetMenu(menuName = "Templates/Create PartTemplate", fileName = "PartTemplate")]
    public class PartTemplate : Template
    {
        /// <summary>
        /// The action
        /// </summary>
        public Action action;

        /// <summary>
        /// The damage
        /// </summary>
        public int damage;
        
        /// <summary>
        /// The armor penetration
        /// </summary>
        public int armorPenetration;

        /// <summary>
        /// The speed
        /// </summary>
        public int speed;

        /// <summary>
        /// The range
        /// </summary>
        public int range;

        /// <summary>
        /// The warmup
        /// </summary>
        public int warmup;

        /// <summary>
        /// The cooldown
        /// </summary>
        public int cooldown;

        /// <summary>
        /// The action count
        /// </summary>
        public int actionCount;
        
        //todo sounds and special fx
    }
}
