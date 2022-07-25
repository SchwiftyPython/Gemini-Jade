using GoRogue;
using UnityEngine;
using World;
using World.TileTypes;

namespace Repos
{
    /// <summary>
    /// Stores all Buildable Tile Types.
    /// </summary>
    /// <remarks>Functions like a Singleton. Use FindObjectOfType&lt;BuildablesRepo&gt;() to get a reference.</remarks>
    public class BuildablesRepo : MonoBehaviour
    {
        [SerializeField] private TileType wall;
    
        /// <summary>
        /// Get a new Wall <see cref="Tile"/> at given position.
        /// </summary>
        /// <param name="position">Given position.</param>
        /// <returns>A Ground <see cref="Tile"/> at given position.</returns>
        /// <remarks>Just for testing out grid system. Might work if we pass in wall type as a param.</remarks>
        public Tile WallTile(Coord position)
        {
            return wall.NewTile(position);
        }
    }
}
