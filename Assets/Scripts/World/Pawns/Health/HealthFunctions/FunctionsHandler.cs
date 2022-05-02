using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Utilities;
using UnityEngine;
using Utilities;
using World.Pawns.Health.HealthModifiers;

namespace World.Pawns.Health.HealthFunctions
{
    public class FunctionsHandler
    {
        private Pawn _pawn;

        private Dictionary<HealthFunctionTemplate, float> _functionLevels;

        public bool canWakeUp; //todo if consciousness >= 0.3f

        public FunctionsHandler(Pawn pawn)
        {
            _pawn = pawn;
            
            InitializeFunctionLevels();
            
            HealthDebug.OnBodyChanged += HealthDebug_OnBodyChanged;
        }

        public void Clear()
        {
            _functionLevels = null;
        }

        public float GetLevel(HealthFunctionTemplate function, List<HealthMod> healthMods)
        {
            //todo throw exception if pawn is null somehow
            
            if (_pawn.Dead)
            {
                return 0f;
            }

            if (_functionLevels == null)
            {
                InitializeFunctionLevels();
            }

            _functionLevels[function] = HealthFunctionUtils.CalculateFunctionLevel(_pawn, healthMods, function); 

            return _functionLevels[function];
        }

        public Dictionary<HealthFunctionTemplate, float> GetFunctionLevels()
        {
            return _functionLevels;
        }

        public bool CapableOf(HealthFunctionTemplate function)
        {
            return GetLevel(function, _pawn.health.GetHealthMods()) > function.functionalMin;
        }

        private void InitializeFunctionLevels()
        {
            _functionLevels = new Dictionary<HealthFunctionTemplate, float>();

            var healthFunctionRepo = Object.FindObjectOfType<HealthFunctionRepo>();
            
            var templates = healthFunctionRepo.GetAllHealthFunctions();

            foreach (var template in templates)
            {
                if (template == null)
                {
                    continue;
                }

                if (_functionLevels.ContainsKey(template))
                {
                    Debug.LogError($"Health Function Template: {template.templateName} already added to Function Levels!");
                    continue;
                }

                const float initFunctionLevel = 1f;
                
                _functionLevels.Add(template, initFunctionLevel);
            }
        }

        private void UpdateFunctionLevels()
        {
            if (_functionLevels == null || !_functionLevels.Any())
            {
                InitializeFunctionLevels();
            }

            foreach (var healthFunction in _functionLevels.Keys.ToArray())
            {
                if (!_functionLevels.ContainsKey(healthFunction))
                {
                    Debug.LogError(
                        $"Health Function Template: {healthFunction.templateName} doesn't exist in Function Levels!");
                    continue;
                }

                _functionLevels[healthFunction] = GetLevel(healthFunction, _pawn.health.GetHealthMods());
            }
        }
        
        private void HealthDebug_OnBodyChanged()
        {
            UpdateFunctionLevels();
        }
    }
}
