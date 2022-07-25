using System.Collections.Generic;
using System.Linq;
using GoRogue;
using UnityEngine;
using World.TileTypes;
using Random = UnityEngine.Random;

namespace World
{
    public class Tile : BaseObject
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public Tile()
        {
        }
        
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="tileType">Tile's <see cref="TileType"/>.</param>
        /// <param name="position">Tile's position.</param>
        public Tile(Coord position, TileType tileType) : base(position, (int) MapLayer.Terrain,  true, tileType.walkable, tileType.transparent)
        {
            Texture = ChooseTexture(tileType);
        }

        public Tile GetAdjacentTileByDirection(Direction direction)
        {
            var neighbors = AdjacencyRule.EIGHT_WAY.NeighborsClockwise(Position, direction);

            return ((LocalMap)CurrentMap).GetTileAt(neighbors.First());
        }

        public List<Tile> GetAdjacentTiles()
        {
            var neighbors = AdjacencyRule.EIGHT_WAY.NeighborsClockwise(Position);

            var tiles = new List<Tile>();

            foreach (var coord in neighbors)
            {
                var tile = ((LocalMap)CurrentMap).GetTileAt(coord);

                if (tile == null)
                {
                    continue;
                }

                tiles.Add(tile);
            }

            return tiles;
        }

        protected override Sprite GetTextureFrom(ScriptableObject objectType)
        {
            var tileType = objectType as TileType;

            return tileType == null ? base.GetTextureFrom(objectType) : ChooseTexture(tileType);
        }

        /// <summary>
        /// Gets a random Sprite that's assigned to the TileType.
        /// </summary>
        /// <returns>A random Sprite that's assigned to the TileType.</returns>
        private static Sprite ChooseTexture(TileType tileType)
        {
            if (tileType.Textures == null)
            {
                Debug.LogError($"No texture defined for {tileType.name}");
                return null;
            }

            if (tileType.Textures.Any())
            {
                return tileType.Textures[Random.Range(0, tileType.Textures.Length)];
            }

            Debug.LogError($"No texture defined for {tileType.name}");
            return null;
        }
    }
}
