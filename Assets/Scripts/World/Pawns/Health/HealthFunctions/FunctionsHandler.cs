using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Utilities;
using UnityEngine;
using Utilities;
using World.Pawns.Health.HealthModifiers;

namespace World.Pawns.Health.HealthFunctions
{
    /// <summary>
    /// The functions handler class
    /// </summary>
    public class FunctionsHandler
    {
        /// <summary>
        /// The function level class
        /// </summary>
        private class FunctionLevel
        {
            /// <summary>
            /// The dirty
            /// </summary>
            public bool dirty;
            /// <summary>
            /// The value
            /// </summary>
            public float value;
        }
    
        /// <summary>
        /// The pawn
        /// </summary>
        private Pawn _pawn;

        /// <summary>
        /// The function levels
        /// </summary>
        private Dictionary<HealthFunctionTemplate, FunctionLevel> _functionLevels;

        /// <summary>
        /// Gets the value of the can wake up
        /// </summary>
        public bool CanWakeUp
        {
            get
            {
                var functionRepo = Object.FindObjectOfType<HealthFunctionRepo>();

                return GetLevel(functionRepo.consciousness, _pawn.health.GetHealthMods()) >= 0.3f;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FunctionsHandler"/> class
        /// </summary>
        /// <param name="pawn">The pawn</param>
        public FunctionsHandler(Pawn pawn)
        {
            _pawn = pawn;
            
            InitializeFunctionLevels();
            
            HealthDebug.OnBodyChanged += HealthDebug_OnBodyChanged;
        }

        /// <summary>
        /// Clears this instance
        /// </summary>
        public void Clear()
        {
            _functionLevels = null;
        }

        /// <summary>
        /// Gets the level using the specified function
        /// </summary>
        /// <param name="function">The function</param>
        /// <param name="healthMods">The health mods</param>
        /// <returns>The float</returns>
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

        /// <summary>
        /// Gets the function levels
        /// </summary>
        /// <returns>The levels</returns>
        public Dictionary<HealthFunctionTemplate, float> GetFunctionLevels()
        {
            var levels = new Dictionary<HealthFunctionTemplate, float>();

            foreach (var functionLevel in _functionLevels)
            {
                levels.Add(functionLevel.Key, functionLevel.Value.value);
            }
            
            return levels;
        }

        /// <summary>
        /// Describes whether this instance capable of
        /// </summary>
        /// <param name="function">The function</param>
        /// <returns>The bool</returns>
        public bool CapableOf(HealthFunctionTemplate function)
        {
            return GetLevel(function, _pawn.health.GetHealthMods()) > function.functionalMin;
        }

        /// <summary>
        /// Initializes the function levels
        /// </summary>
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

        /// <summary>
        /// Updates the function levels
        /// </summary>
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
        
        /// <summary>
        /// Healths the debug on body changed
        /// </summary>
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
