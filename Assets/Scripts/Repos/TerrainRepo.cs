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
        /// <summary>
        /// The ground
        /// </summary>
        [SerializeField] private TileType ground;
        
        /// <summary>
        /// Get a new Ground <see cref="Tile"/> at given position.
        /// </summary>
        /// <param name="position">Given position.</param>
        /// <returns>A Ground <see cref="Tile"/> at given position.</returns>
        public Tile GroundTile(Coord position)
        {
            return ground.NewTile(position);
        }
    }
}
