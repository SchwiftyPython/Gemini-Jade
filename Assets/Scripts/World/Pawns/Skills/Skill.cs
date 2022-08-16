using System;
using Assets.Scripts.World;
using UnityEngine;

namespace World.Pawns.Skills
{
    [CreateAssetMenu(menuName = "My Assets/Skill")]
    public class Skill : Template
    {
        public static int minSkillLevel = 0;
        public static int maxSkillLevel = 16;

        [NonSerialized] public Action<Job> onSkillNeeded;
        
        public Type goalClass;

        public string verb;
    }
}
