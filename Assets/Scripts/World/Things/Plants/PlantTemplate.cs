using Sirenix.OdinInspector;
using UnityEngine;

namespace World.Things.Plants
{
    [CreateAssetMenu(menuName = "Create PlantTemplate", fileName = "PlantTemplate")]
    public class PlantTemplate : ThingTemplate
    {
        public int minTemp = 32;

        public int maxTemp = 130;

        public bool canCut;

        public bool canHarvest;
        
        public Thing thingHarvested;

        public int numThingHarvested;

        public float minFertility = 0.5f;
        
        //todo growth states

        public float daysToMaturity = 5.8f;

        /// <summary>
        /// Multiplied by days to maturity to determine lifespan of plant
        /// </summary>
        [PropertyRange(3f, 7f)]
        public float lifeSpanMultiplier = 5f;
    }
}
