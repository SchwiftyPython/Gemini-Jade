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
        private class FunctionLevel
        {
            public bool dirty;
            public float value;
        }
    
        private Pawn _pawn;

        private Dictionary<HealthFunctionTemplate, FunctionLevel> _functionLevels;

        public bool CanWakeUp
        {
            get
            {
                var functionRepo = Object.FindObjectOfType<HealthFunctionRepo>();

                return GetLevel(functionRepo.consciousness, _pawn.health.GetHealthMods()) >= 0.3f;
            }
        }

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

            if (!_functionLevels[function].dirty)
            {
                return _functionLevels[function].value;
            }

            _functionLevels[function].value = HealthFunctionUtils.CalculateFunctionLevel(_pawn, healthMods, function);

            _functionLevels[function].dirty = false;

            return _functionLevels[function].value;
        }

        public Dictionary<HealthFunctionTemplate, float> GetFunctionLevels()
        {
            var levels = new Dictionary<HealthFunctionTemplate, float>();

            foreach (var functionLevel in _functionLevels)
            {
                levels.Add(functionLevel.Key, functionLevel.Value.value);
            }
            
            return levels;
        }

        public bool CapableOf(HealthFunctionTemplate function)
        {
            return GetLevel(function, _pawn.health.GetHealthMods()) > function.functionalMin;
        }

        private void InitializeFunctionLevels()
        {
            _functionLevels = new Dictionary<HealthFunctionTemplate, FunctionLevel>();

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

                var functionLevel = new FunctionLevel
                {
                    dirty = true,
                    value = initFunctionLevel
                };
                
                _functionLevels.Add(template, functionLevel);
            }
        }

        private void UpdateFunctionLevels()
        {
            if (_functionLevels == null)
            {
                InitializeFunctionLevels();
            }
            else if (!_functionLevels.Any())
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

                GetLevel(healthFunction, _pawn.health.GetHealthMods());
            }
        }
        
        private void HealthDebug_OnBodyChanged()
        {
            foreach (var healthFunction in _functionLevels.Keys.ToArray())
            {
                _functionLevels[healthFunction].dirty = true;
            }
            
            UpdateFunctionLevels();
        }
    }
}
