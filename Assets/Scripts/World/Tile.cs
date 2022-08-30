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
        private TileType _tileType;
        
        /// <summary>
        /// Gets the value of the texture
        /// </summary>
        public Sprite Texture { get; }
        
        /// <summary>
        /// Scale
        /// </summary>
        public Vector3 scale = Vector3.one;
        
        /// <summary>
        /// Graphic instance
        /// </summary>
        public GraphicInstance MainGraphic { get; set; }
        
        /// <summary>
        /// Additional graphics
        /// </summary>
        public Dictionary<string, GraphicInstance> AddGraphics { get; set; }

        /// <summary>
        /// Tile tick counts
        /// </summary>
        protected int ticks = 0;

        /// <summary>
        /// Matrices
        /// </summary>
        private Dictionary<int, Matrix4x4> _matrices;

        /// <summary>
        /// Do we need to reset matrices
        /// </summary>
        public bool resetMatrices;

        /// <summary>
        /// If this is True the tile is not drawn
        /// </summary>
        public bool hidden = false;

        public bool HasInstancedGraphics => _tileType.graphics.isInstanced;

        /// <summary>
        /// Parent bucket
        /// </summary>
        public LayerGridBucket Bucket { get; protected set; }

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
            _tileType = tileType;
            
            MainGraphic = GraphicInstance.GetNew(_tileType.graphics);
            
            //Texture = ChooseTexture(tileType);
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
            
            if (!_matrices.ContainsKey(graphicUid)) 
            {
                var mat = Matrix4x4.identity;
                
                mat.SetTRS(
                    new Vector3(
                        Position.X
                        -_tileType.graphics.pivot.x*scale.x
                        +(1f-scale.x)/2f
                        ,Position.Y
                         -_tileType.graphics.pivot.y*scale.y
                         +(1f-scale.y)/2f
                        ,(float) (_tileType.layer + (byte) GraphicInstance.instances[graphicUid].Priority) //todo not sure how this will look
                    ), 
                    Quaternion.identity, 
                    scale
                );
                _matrices.Add(graphicUid, mat);
            }
            return _matrices[graphicUid];
        }
        
        public void SetBucket(LayerGridBucket layerGridBucket)
        {
            Bucket = layerGridBucket;
        }

        public MapLayer GetMapLayer()
        {
            return (MapLayer) Layer;
        }

        /// <summary>
        /// Gets the texture from using the specified object type
        /// </summary>
        /// <param name="objectType">The object type</param>
        /// <returns>The sprite</returns>
        protected override Sprite GetTextureFrom(ScriptableObject objectType)
        {
            var tileType = objectType as TileType;

            return tileType == null ? Texture : ChooseTexture(tileType);
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
