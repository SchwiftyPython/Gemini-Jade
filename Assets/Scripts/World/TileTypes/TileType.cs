using System.Linq;
using GoRogue;
using GoRogue.GameFramework;
using UnityEngine;

namespace World.TileTypes
{
    /// <summary>
    /// Scriptable Object used to hold data for different types of <see cref="Tile"/>s.
    /// </summary>
    [CreateAssetMenu(menuName = "My Assets/Tile Type")]
    public class TileType : ScriptableObject
    {
        /// <summary>
        /// The textures available for the TileType.
        /// </summary>
        [SerializeField] private Sprite[] textures;
        
        public Sprite[] Textures => textures;

        /// <summary>
        /// Whether or not the object is considered "transparent", eg. whether or not light passes through it.
        /// </summary>
        public bool transparent;

        /// <summary>
        /// Whether or not the object is to be considered "walkable", eg. whether or not the square it resides
        /// on can be traversed by other, non-walkable objects on the same <see cref="Map"/>.  Effectively, whether or not this
        /// object collides.
        /// </summary>
        public bool walkable;
        
        /// <summary>
        /// Creates an <see cref="Tile"/> using this <see cref="TileType"/>.
        /// </summary>
        /// <param name="position">Position tile is created at.</param>
        /// <returns>A <see cref="Tile"/> using this <see cref="TileType"/>.</returns>
        public Tile NewTile(Coord position)
        {
            return new Tile(position, this);
        }
    }
}
