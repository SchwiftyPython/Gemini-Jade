using System.Collections.Generic;
using Assets.Scripts.World.Pawns;
using UnityEngine;
using UnityEngine.AddressableAssets;
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

        public bool CapableOf(HealthFunctionTemplate function)
        {
            return GetLevel(function, _pawn.health.GetHealthMods()) > function.functionalMin;
        }

        private void InitializeFunctionLevels()
        {
            _functionLevels = new Dictionary<HealthFunctionTemplate, float>();
            
            //todo add all health function templates. 

            var locations = Addressables.LoadResourceLocationsAsync("healthfunctionstemplates", typeof(HealthFunctionTemplate)).Result;

            foreach (var location in locations)
            {
                var template = Addressables.LoadAssetAsync<HealthFunctionTemplate>(location.PrimaryKey).Result;

                if (template == null)
                {
                    Debug.LogError($"Could not load Health Function Template: {location.PrimaryKey}!");
                    continue;
                }

                if (_functionLevels.ContainsKey(template))
                {
                    Debug.LogError($"Health Function Template: {location.PrimaryKey} already added to Function Levels!");
                    continue;
                }
                
                _functionLevels.Add(template, -1f);
            }
        }
    }
}
