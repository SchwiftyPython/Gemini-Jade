using Assets.Scripts.World;
using UnityEngine;

namespace World.Things.CraftableThings
{
    [CreateAssetMenu(menuName = "Templates/Create CraftableTemplate", fileName = "CraftableTemplate")]
    public class CraftableTemplate : ThingTemplate
    {
        public int workToMake;
        
        //todo materials
    }

}