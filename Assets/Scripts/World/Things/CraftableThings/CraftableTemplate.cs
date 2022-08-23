using UnityEngine;
using World.Pawns.Skills;

namespace World.Things.CraftableThings
{
    /// <summary>
    /// The craftable template class
    /// </summary>
    /// <seealso cref="ThingTemplate"/>
    [CreateAssetMenu(menuName = "Templates/Create CraftableTemplate", fileName = "CraftableTemplate")]
    public class CraftableTemplate : ThingTemplate
    {
        /// <summary>
        /// The work to make
        /// </summary>
        public int workToMake;

        /// <summary>
        /// The skill
        /// </summary>
        public Skill skill;
        
        /// <summary>
        /// The min skill level
        /// </summary>
        public int minSkillLevel;

        //todo materials
    }

}