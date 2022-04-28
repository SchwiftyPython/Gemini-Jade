using TMPro;
using UnityEngine;
using World.Pawns.Health.HealthFunctions;

namespace UI
{
    public class HealthFunction : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI value;
        
        public HealthFunctionTemplate functionType;

        public void SetCapacityValue(int capacityValue)
        {
            value.text = capacityValue.ToString();
        }
    }
}
