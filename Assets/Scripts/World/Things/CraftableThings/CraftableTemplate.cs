using Assets.Scripts.World;
using UnityEngine;
using World.Pawns.Skills;

namespace World.Things.CraftableThings
{
    [CreateAssetMenu(menuName = "Templates/Create CraftableTemplate", fileName = "CraftableTemplate")]
    public class CraftableTemplate : ThingTemplate
    {
        public int workToMake;

        public Skill skill;
        
        public int minSkillLevel;

        //todo materials
    }

}