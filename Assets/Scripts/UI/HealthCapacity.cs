using TMPro;
using UnityEngine;
using World.Pawns.Health;

namespace UI
{
    public class HealthCapacity : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI value;
        
        public Capacity capacityType;

        public void SetCapacityValue(int capacityValue)
        {
            value.text = capacityValue.ToString();
        }
    }
}
