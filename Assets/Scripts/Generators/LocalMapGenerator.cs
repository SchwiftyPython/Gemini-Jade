using GoRogue.MapGeneration;
using GoRogue.MapViews;
using Repos;
using UnityEngine;
using World;

namespace Generators
{
    public class LocalMapGenerator
    {
        public LocalMap GenerateMap(int width, int height)
        {
            return GenerateTerrain(width, height);
        }
    
        private LocalMap GenerateTerrain(int width, int height)
        {
            //todo not sure if we'll use this gen or wfc. I'd prefer wfc I think.
            
            var terrainMap = new ArrayMap<bool>(width, height);
            
            QuickGenerators.GenerateRectangleMap(terrainMap);

            var map = new LocalMap(width, height);

            var terrainRepo = Object.FindObjectOfType<TerrainRepo>();

            foreach (var position in terrainMap.Positions())
            {
                var testGroundTile = terrainRepo.GroundTile(position);
                
                map.SetTerrain(testGroundTile);
            }

            return map;
        }
    }
}
