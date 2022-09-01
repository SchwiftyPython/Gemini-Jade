using GoRogue.MapGeneration;
using GoRogue.MapViews;
using Repos;
using UnityEngine;
using Utilities;
using World;

namespace Generators
{
    /// <summary>
    /// The local map generator class
    /// </summary>
    public class LocalMapGenerator
    {
        /// <summary>
        /// Generates the map using the specified width
        /// </summary>
        /// <param name="width">The width</param>
        /// <param name="height">The height</param>
        /// <returns>The local map</returns>
        public LocalMap GenerateMap(int width, int height)
        {
            return GenerateTerrain(width, height);
        }
    
        /// <summary>
        /// Generates the terrain using the specified width
        /// </summary>
        /// <param name="width">The width</param>
        /// <param name="height">The height</param>
        /// <returns>The map</returns>
        private LocalMap GenerateTerrain(int width, int height)
        {
            //todo not sure if we'll use this gen or wfc. I'd prefer wfc I think.
            
            var terrainMap = new ArrayMap<bool>(width, height);
            
            QuickGenerators.GenerateRectangleMap(terrainMap);

            var map = new LocalMap(width, height);

            var noiseMap = PerlinNoise.GenerateNoiseMap(width, height, 5);

            var terrainRepo = Object.FindObjectOfType<TerrainRepo>();

            foreach (var position in terrainMap.Positions())
            {
                var noiseHeight = noiseMap[position.X + position.Y * width];

                var groundTile = terrainRepo.GetTerrainByHeight(noiseHeight, position);
                
                map.AddTile(groundTile, position);
                
                map.SetTerrain(groundTile); //todo not sure if needed. Probably not if we have to stick with layer grids.
            }

            return map;
        }
    }
}
