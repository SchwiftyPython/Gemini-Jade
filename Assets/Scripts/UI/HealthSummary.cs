using System.Collections.Generic;
using UnityEngine;
using World.Pawns.Health.HealthFunctions;

namespace UI
{
    /// <summary>
    /// The health summary class
    /// </summary>
    /// <seealso cref="MonoBehaviour"/>
    public class HealthSummary : MonoBehaviour
    {
        /// <summary>
        /// The functions
        /// </summary>
        [SerializeField] private List<HealthFunction> functions;
        
        /// <summary>
        /// Starts this instance
        /// </summary>
        private void Start()
        {
        }

        /// <summary>
        /// Shows this instance
        /// </summary>
        private void Show()
        {
            //todo add to interface or base class

            gameObject.SetActive(true);
        }

        /// <summary>
        /// Hides this instance
        /// </summary>
        private void Hide()
        {
            //todo add to interface or base class
            
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Draws the function values
        /// </summary>
        /// <param name="functionValues">The function values</param>
        public void Draw(IReadOnlyDictionary<HealthFunctionTemplate, int> functionValues)
        {
            foreach (var healthFunction in functions)
            {
                if (healthFunction.functionType == null || !functionValues.ContainsKey(healthFunction.functionType))
                {
                    continue;
                }

                healthFunction.SetFunctionValue(functionValues[healthFunction.functionType]);
            }
        }
    }
}
