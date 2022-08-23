using TMPro;
using UnityEngine;
using World.Pawns.Health.HealthFunctions;

namespace UI
{
    /// <summary>
    /// The health function class
    /// </summary>
    /// <seealso cref="MonoBehaviour"/>
    public class HealthFunction : MonoBehaviour
    {
        /// <summary>
        /// The value
        /// </summary>
        [SerializeField] private TextMeshProUGUI value;
        
        /// <summary>
        /// The function type
        /// </summary>
        public HealthFunctionTemplate functionType;

        /// <summary>
        /// Sets the function value using the specified capacity value
        /// </summary>
        /// <param name="capacityValue">The capacity value</param>
        public void SetFunctionValue(int capacityValue)
        {
            value.text = $"{capacityValue.ToString()}%";
        }
    }
}
