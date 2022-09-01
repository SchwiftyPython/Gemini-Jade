using System.Collections.Generic;
using System.Linq;
using GoRogue;
using UnityEngine;
using World;
using World.TileTypes;

namespace Repos
{
    /// <summary>
    /// Stores all Terrain Tile Types.
    /// </summary>
    /// <remarks>Functions like a Singleton. Use FindObjectOfType&lt;TerrainRepo&gt;() to get a reference.</remarks>
    public class TerrainRepo : MonoBehaviour
    {
        private SortedDictionary<float, TileType> _heightOrderedTileTypes;
        
        private Dictionary<string, TileType> _unOrderedTileTypes;

        /// <summary>
        /// Default material for tiles
        /// </summary>
        [SerializeField] private Material tileMaterial;

        /// <summary>
        /// Collection of <see cref="TileType"/>s
        /// </summary>
        [SerializeField] private TileType[] tileTypes;

        private void Awake()
        {
           Populate();
           
           HeightOrderTileTypes();
        }

        /// <summary>
        /// Gets tile material
        /// </summary>
        /// <returns></returns>
        public Material GetTileMaterial()
        {
            return tileMaterial;
        }

        /// <summary>
        /// Gets a terrain tile based on height
        /// </summary>
        /// <param name="height"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public Tile GetTerrainByHeight(float height, Coord position)
        {
            foreach (var tileType in _heightOrderedTileTypes.Values)
            {
                if (height <= tileType.maxHeight)
                {
                    return tileType.NewTile(position);
                }
            }

            return tileTypes.First().NewTile(position); //todo return water
        }

        /// <summary>
        /// Populates unordered collections
        /// </summary>
        private void Populate()
        {
            _unOrderedTileTypes = new Dictionary<string, TileType>();
            
            foreach (var tileType in tileTypes)
            {
                _unOrderedTileTypes.Add(tileType.Uid, tileType);
            }
        }

        /// <summary>
        /// Sorts <see cref="TileType"/>s by height into a <see cref="SortedDictionary{TKey,TValue}"/>
        /// </summary>
        private void HeightOrderTileTypes()
        {
            _heightOrderedTileTypes = new SortedDictionary<float, TileType>();
            
            foreach (var tileType in _unOrderedTileTypes.Values)
            {
                _heightOrderedTileTypes.Add(tileType.maxHeight, tileType);
            }
        }
    }
}
