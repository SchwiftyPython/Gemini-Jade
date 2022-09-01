using System.Collections.Generic;
using System.Linq;
using GoRogue;
using Graphics;
using UnityEngine;
using World.TileTypes;

namespace World
{
    /// <summary>
    /// The tile class
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
        /// Scale
        /// </summary>
        private readonly Vector3 scale = Vector3.one;
        
        /// <summary>
        /// Graphic instance
        /// </summary>
        public GraphicInstance MainGraphic { get; }
        
        /// <summary>
        /// Additional graphics
        /// </summary>
        public Dictionary<string, GraphicInstance> AddGraphics { get; }

        /// <summary>
        /// Tile tick counts
        /// </summary>
        private int ticks = 0;

        /// <summary>
        /// Matrices
        /// </summary>
        private Dictionary<int, Matrix4x4> _matrices;

        /// <summary>
        /// Do we need to reset matrices
        /// </summary>
        private bool resetMatrices;

        /// <summary>
        /// If this is True the tile is not drawn
        /// </summary>
        public readonly bool hidden = false;

        public bool HasInstancedGraphics => _tileType.graphics.isInstanced;

        /// <summary>
        /// Parent bucket
        /// </summary>
        private LayerGridBucket Bucket { get; set; }

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
        }

        /// <summary>
        /// Gets the adjacent tile by direction using the specified direction
        /// </summary>
        /// <param name="direction">The direction</param>
        /// <returns>The tile</returns>
        public Tile GetAdjacentTileByDirection(Direction direction)
        {
            var neighbors = AdjacencyRule.EIGHT_WAY.NeighborsClockwise(Position, direction);

            return ((LocalMap)CurrentMap).GetTileAt(neighbors.First());
        }

        /// <summary>
        /// Gets the adjacent tiles
        /// </summary>
        /// <returns>The tiles</returns>
        public List<Tile> GetAdjacentTiles()
        {
            var neighbors = AdjacencyRule.EIGHT_WAY.NeighborsClockwise(Position);

            return neighbors.Select(coord => ((LocalMap) CurrentMap).GetTileAt(coord)).Where(tile => tile != null)
                .ToList();
        }
        
        /// <summary>
        /// Get the Tile Matrix by Uid
        /// </summary>
        /// <param name="graphicUid"></param>
        /// <returns></returns>
        public Matrix4x4 GetMatrix(int graphicUid) 
        {
            if (_matrices == null || resetMatrices) 
            {
                _matrices = new Dictionary<int, Matrix4x4>();
                resetMatrices = true;
            }

            if (_matrices.ContainsKey(graphicUid))
            {
                return _matrices[graphicUid];
            }

            var mat = Matrix4x4.identity;
                
            mat.SetTRS(
                new Vector3(
                    Position.X
                    -_tileType.graphics.pivot.x*scale.x
                    +(1f-scale.x)/2f
                    ,Position.Y
                     -_tileType.graphics.pivot.y*scale.y
                     +(1f-scale.y)/2f
                    ,(float) (_tileType.layer + (byte) GraphicInstance.instances[graphicUid].Priority) 
                ), 
                Quaternion.identity, 
                scale
            );
            
            _matrices.Add(graphicUid, mat);
            
            return _matrices[graphicUid];
        }
        
        /// <summary>
        /// Sets the tile's bucket
        /// </summary>
        /// <param name="layerGridBucket"></param>
        public void SetBucket(LayerGridBucket layerGridBucket)
        {
            Bucket = layerGridBucket;
        }

        /// <summary>
        /// Gets the tile's map layer 
        /// </summary>
        /// <returns></returns>
        public MapLayer GetMapLayer()
        {
            return (MapLayer) Layer;
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
