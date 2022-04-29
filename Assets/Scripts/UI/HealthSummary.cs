using System.Collections.Generic;
using UnityEngine;
using World.Pawns.Health.HealthFunctions;

namespace UI
{
    public class HealthSummary : MonoBehaviour
    {
        [SerializeField] private List<HealthFunction> functions;
        
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
