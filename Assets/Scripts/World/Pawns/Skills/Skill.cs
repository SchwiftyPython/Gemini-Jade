using System;
using Assets.Scripts.World;
using UnityEngine;
using World.Pawns.Jobs;

namespace World.Pawns.Skills
{
    /// <summary>
    /// The skill class
    /// </summary>
    /// <seealso cref="Template"/>
    [CreateAssetMenu(menuName = "My Assets/Skill")]
    public class Skill : Template
    {
        /// <summary>
        /// The min skill level
        /// </summary>
        public static int minSkillLevel = 0;
        /// <summary>
        /// The max skill level
        /// </summary>
        public static int maxSkillLevel = 16;

        /// <summary>
        /// The on skill needed
        /// </summary>
        [NonSerialized] public Action<Job> onSkillNeeded;
        
        /// <summary>
        /// The goal
        /// </summary>
        public Type goalClass;

        /// <summary>
        /// The verb
        /// </summary>
        public string verb;

        /// <summary>
        /// The default priority
        /// </summary>
        public int defaultPriority;
    }
}
