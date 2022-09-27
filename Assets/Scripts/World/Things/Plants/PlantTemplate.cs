using GoRogue;
using Sirenix.OdinInspector;
using UnityEngine;
using World.Pawns.Skills;

namespace World.Things.Plants
{
    [CreateAssetMenu(menuName = "Create PlantTemplate", fileName = "PlantTemplate")]
    public class PlantTemplate : ThingTemplate
    {
        public int minTemp = 32;

        public int maxTemp = 130;

        public bool canCut;

        public bool canHarvest;
        
        public ThingTemplate thingHarvested;

        public int numThingHarvested;

        public float minFertility = 0.5f;

        public int workToHarvest = -1;

        public int workToCut = -1;

        public Skill skill;

        public int minSkillLevel = 0;
        
        //todo growth states

        public float daysToMaturity = 5.8f;

        /// <summary>
        /// Multiplied by days to maturity to determine lifespan of plant
        /// </summary>
        [PropertyRange(3f, 20f)]
        public float lifeSpanMultiplier = 5f;

        public Plant NewPlant(Coord position)
        {
            return new Plant(this, position);
        }
    }
}
