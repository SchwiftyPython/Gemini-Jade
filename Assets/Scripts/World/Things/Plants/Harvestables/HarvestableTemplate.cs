using UnityEngine;

namespace World.Things.Plants.Harvestables
{
    [CreateAssetMenu(menuName = "Create HarvestableTemplate", fileName = "HarvestableTemplate")]
    public class HarvestableTemplate : PlantTemplate
    {
        public Thing thingDropped;

        public int numThingDropped;
    }
}
