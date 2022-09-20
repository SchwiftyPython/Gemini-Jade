using System.Collections.Generic;
using System.Linq;
using GoRogue;
using Graphics;
using UnityEngine;
using World.TileTypes;

namespace World
{
    /// <summary>
    /// A better name for this might be terrain. Represents the
    /// bottom layer of the map. Not directly interactable.
    /// </summary>
    /// <seealso cref="BaseObject"/>
    public class Tile : BaseObject
    {
        private readonly TileType _tileType;
        
        /// <summary>
        /// Gets the value of the texture
        /// </summary>
        public Sprite Texture { get; }

        /// <summary>
        /// Tile tick counts
        /// </summary>
        private int ticks = 0;

        public float Fertility => _tileType.fertility;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Tile(Sprite texture, Dictionary<string, GraphicInstance> addGraphics, int ticks)
        {
            Texture = texture;
            AddGraphics = addGraphics;
            this.ticks = ticks;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="tileType">Tile's <see cref="TileType"/>.</param>
        /// <param name="position">Tile's position.</param>
        public Tile(Coord position, TileType tileType) : base(position, (int) MapLayer.Terrain, true, tileType.walkable, tileType.transparent)
        {
            _tileType = tileType;
            
            MainGraphic = GraphicInstance.GetNew(_tileType.graphics);

            graphicTemplate = _tileType.graphics;
        }

        /// <summary>
        /// Gets tile's max height
        /// </summary>
        /// <returns></returns>
        public float GetMaxHeight()
        {
            return _tileType.maxHeight;
        }
    }
}
