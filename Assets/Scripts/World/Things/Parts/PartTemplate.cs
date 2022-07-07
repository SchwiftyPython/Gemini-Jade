using Assets.Scripts.World;
using UnityEngine;
using World.Pawns.Actions;

namespace World.Things.Parts
{
    
    [CreateAssetMenu(menuName = "Templates/Create PartTemplate", fileName = "PartTemplate")]
    public class PartTemplate : Template
    {
        public Action action;

        public int damage;
        
        public int armorPenetration;

        public int speed;

        public int range;

        public int warmup;

        public int cooldown;

        public int actionCount;
        
        //todo sounds and special fx
    }
}
