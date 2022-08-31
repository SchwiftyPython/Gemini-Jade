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
        /// The ground
        /// </summary>
        [SerializeField] private Material tileMaterial;

        [SerializeField] private TileType[] tileTypes;

        private void Awake()
        {
           Populate();
           
           HeightOrderTileTypes();
        }

        public Material GetTileMaterial()
        {
            return tileMaterial;
        }

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

        public void Populate()
        {
            _unOrderedTileTypes = new Dictionary<string, TileType>();
            
            foreach (var tileType in tileTypes)
            {
                _unOrderedTileTypes.Add(tileType.Uid, tileType);
            }
        }

        public void HeightOrderTileTypes()
        {
            _heightOrderedTileTypes = new SortedDictionary<float, TileType>();
            
            foreach (var tileType in _unOrderedTileTypes.Values)
            {
                _heightOrderedTileTypes.Add(tileType.maxHeight, tileType);
            }
        }
    }
}
