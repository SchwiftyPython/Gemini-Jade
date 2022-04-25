using System.Collections.Generic;
using UnityEngine;
using World.Pawns.Health;

namespace UI
{
    public class HealthSummary : MonoBehaviour
    {
        [SerializeField] private List<HealthCapacity> capacities;
        
        private void Start()
        {
        }

        private void Show()
        {
            //todo add to interface or base class

            gameObject.SetActive(true);
        }

        private void Hide()
        {
            //todo add to interface or base class
            
            gameObject.SetActive(false);
        }

        private void Draw(IReadOnlyDictionary<Capacity, int> capacityValues)
        {
            foreach (var healthCapacity in capacities)
            {
                if (!capacityValues.ContainsKey(healthCapacity.capacityType))
                {
                    continue;
                }
                
                healthCapacity.SetCapacityValue(capacityValues[healthCapacity.capacityType]);
            }
        }
    }
}
