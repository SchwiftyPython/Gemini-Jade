using UnityEngine;
using World.Things.Plants;
using Random = UnityEngine.Random;

namespace Repos
{
    public class PlantRepo : MonoBehaviour
    {
        public PlantTemplate[] allPlants;

        private void Awake()
        {
        
        }

        public PlantTemplate GetRandomPlant()
        {
            return allPlants[Random.Range(0, allPlants.Length)];
        }
    }
}
