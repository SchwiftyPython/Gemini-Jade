using System.Collections.Generic;
using Repos;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace World.Things.CraftableThings.Buildings
{
    public class BuildingLoader : MonoBehaviour
    {
        [SerializeField] private AssetLabelReference buildingAssetLabel;

        private BuildablesRepo _buildingRepo;

        public void LoadBuildings()
        {
            if (_buildingRepo == null)
            {
                _buildingRepo = FindObjectOfType<BuildablesRepo>();
            }

            _buildingRepo.buildings = new List<PlacedObjectTemplate>();
        
            Addressables.LoadAssetsAsync<PlacedObjectTemplate>(buildingAssetLabel, template =>
            {
                AddBuilding(template);
            
                Debug.Log($"{template.label} loaded");
            });
        }
    
        private void AddBuilding(PlacedObjectTemplate building)
        {
            _buildingRepo.buildings.Add(building);
        }
    }
}
