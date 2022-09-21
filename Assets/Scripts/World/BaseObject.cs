using System;
using System.Collections.Generic;
using System.Linq;
using GoRogue;
using GoRogue.GameFramework;
using Graphics;
using Graphics.GraphicTemplates;
using UnityEngine;
using GameObject = GoRogue.GameFramework.GameObject;

namespace World
{
    /// <summary>
    /// The base object class
    /// </summary>
    /// <seealso cref="IGameObject"/>
    public class BaseObject : IGameObject
    {
        /// <summary>
        /// The backing field
        /// </summary>
        protected IGameObject backingField;

        /// <summary>
        /// Gets the value of the id
        /// </summary>
        public uint ID => backingField.ID;

        /// <summary>
        /// Gets the value of the layer
        /// </summary>
        public int Layer => backingField.Layer;

        /// <summary>
        /// Gets the value of the current map
        /// </summary>
        public Map CurrentMap => backingField.CurrentMap;

        /// <summary>
        /// Gets the value of the is static
        /// </summary>
        public bool IsStatic => backingField.IsStatic;

        /// <summary>
        /// Gets or sets the value of the is transparent
        /// </summary>
        public bool IsTransparent 
        {
            get => backingField.IsTransparent;
            set => backingField.IsTransparent = value;
        }

        /// <summary>
        /// Gets or sets the value of the is walkable
        /// </summary>
        public bool IsWalkable 
        {
            get => backingField.IsWalkable;
            set => backingField.IsWalkable = value;
        }

        /// <summary>
        /// Gets or sets the value of the position
        /// </summary>
        public Coord Position 
        {
            get => backingField.Position;
            set => backingField.Position = value;
        }

        public Vector3 TransformPosition => SpriteInstance.transform.position;
        
        public bool HasInstancedGraphics => graphicTemplate.isInstanced;

        /// <summary>
        /// The moved
        /// </summary>
        public event EventHandler<ItemMovedEventArgs<IGameObject>> Moved 
        {
            add => backingField.Moved += value;
            remove => backingField.Moved -= value;
        }

        /// <summary>
        /// Gets or sets the value of the sprite instance
        /// </summary>
        public UnityEngine.GameObject SpriteInstance { get; private set; }
        
        /// <summary>
        /// Graphic instance
        /// </summary>
        public GraphicInstance MainGraphic { get; set; }
        
        /// <summary>
        /// Parent bucket
        /// </summary>
        private LayerGridBucket Bucket { get; set; }
        
        protected GraphicTemplate graphicTemplate;
        
        /// <summary>
        /// Matrices
        /// </summary>
        private Dictionary<int, Matrix4x4> _matrices;

        /// <summary>
        /// Do we need to reset matrices
        /// </summary>
        private bool resetMatrices;
        
        /// <summary>
        /// Scale
        /// </summary>
        private readonly Vector3 scale = Vector3.one;
        
        /// <summary>
        /// Additional graphics
        /// </summary>
        public Dictionary<string, GraphicInstance> AddGraphics { get; set; }
        
        /// <summary>
        /// If this is True the tile is not drawn
        /// </summary>
        public readonly bool hidden = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseObject"/> class
        /// </summary>
        /// <param name="position">The position</param>
        /// <param name="layer">The layer</param>
        /// <param name="isStatic">The is static</param>
        /// <param name="isWalkable">The is walkable</param>
        /// <param name="isTransparent">The is transparent</param>
        public BaseObject(Coord position, MapLayer layer, bool isStatic, bool isWalkable, bool isTransparent)
        {
            backingField = new GameObject(position, (int) layer, this, isStatic, isWalkable, isTransparent);
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseObject"/> class
        /// </summary>
        /// <param name="position">The position</param>
        /// <param name="layer">The layer</param>
        /// <param name="isStatic">The is static</param>
        /// <param name="isWalkable">The is walkable</param>
        /// <param name="isTransparent">The is transparent</param>
        protected BaseObject(Vector3 position, MapLayer layer, bool isStatic, bool isWalkable, bool isTransparent)
        {
            var coord = new Coord((int) position.x, (int) position.y);
            
            backingField = new GameObject(coord, (int) layer, this, isStatic, isWalkable, isTransparent);
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseObject"/> class
        /// </summary>
        /// <param name="layer">The layer</param>
        /// <param name="isStatic">The is static</param>
        /// <param name="isWalkable">The is walkable</param>
        /// <param name="isTransparent">The is transparent</param>
        protected BaseObject(MapLayer layer, bool isStatic, bool isWalkable, bool isTransparent)
        {
            var coord = new Coord();
            
            backingField = new GameObject(coord, (int) layer, this, isStatic, isWalkable, isTransparent);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseObject"/> class
        /// </summary>
        protected BaseObject()
        {
        }

        /// <summary>
        /// Sets the sprite instance using the specified instance
        /// </summary>
        /// <param name="instance">The instance</param>
        public void SetSpriteInstance(UnityEngine.GameObject instance)
        {
            SpriteInstance = instance;
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
                    Position.X - 0.5f
                    -graphicTemplate.pivot.x*scale.x
                    +(1f-scale.x)/2f
                    ,Position.Y - 0.5f
                     -graphicTemplate.pivot.y*scale.y
                     +(1f-scale.y)/2f
                    ,(float) (Layer + (byte) GraphicInstance.instances[graphicUid].Priority) 
                ), 
                Quaternion.identity, 
                scale
            );
            
            _matrices.Add(graphicUid, mat);
            
            return _matrices[graphicUid];
        }
        
        /// <summary>
        /// Gets the object's map layer 
        /// </summary>
        /// <returns></returns>
        public MapLayer GetMapLayer()
        {
            return (MapLayer) Layer;
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
        /// Adds the component using the specified component
        /// </summary>
        /// <param name="component">The component</param>
        public void AddComponent(object component)
        {
            backingField.AddComponent(component);
        }

        /// <summary>
        /// Gets the component
        /// </summary>
        /// <typeparam name="T">The </typeparam>
        /// <returns>The</returns>
        public T GetComponent<T>()
        {
            return backingField.GetComponent<T>();
        }

        /// <summary>
        /// Gets the components
        /// </summary>
        /// <typeparam name="T">The </typeparam>
        /// <returns>An enumerable of t</returns>
        public IEnumerable<T> GetComponents<T>()
        {
            return backingField.GetComponents<T>();
        }

        /// <summary>
        /// Describes whether this instance has component
        /// </summary>
        /// <param name="componentType">The component type</param>
        /// <returns>The bool</returns>
        public bool HasComponent(Type componentType)
        {
            return backingField.HasComponent(componentType);
        }

        /// <summary>
        /// Describes whether this instance has component
        /// </summary>
        /// <typeparam name="T">The </typeparam>
        /// <returns>The bool</returns>
        public bool HasComponent<T>()
        {
            return backingField.HasComponent<T>();
        }

        /// <summary>
        /// Describes whether this instance has components
        /// </summary>
        /// <param name="componentTypes">The component types</param>
        /// <returns>The bool</returns>
        public bool HasComponents(params Type[] componentTypes)
        {
            return backingField.HasComponents(componentTypes);
        }

        /// <summary>
        /// Removes the component using the specified component
        /// </summary>
        /// <param name="component">The component</param>
        public void RemoveComponent(object component)
        {
            backingField.RemoveComponent(component);
        }

        /// <summary>
        /// Removes the components using the specified components
        /// </summary>
        /// <param name="components">The components</param>
        public void RemoveComponents(params object[] components)
        {
            backingField.RemoveComponents(components);
        }

        /// <summary>
        /// Describes whether this instance move in
        /// </summary>
        /// <param name="direction">The direction</param>
        /// <returns>The bool</returns>
        public bool MoveIn(Direction direction)
        {
            return backingField.MoveIn(direction);
        }

        /// <summary>
        /// Ons the map changed using the specified new map
        /// </summary>
        /// <param name="newMap">The new map</param>
        public void OnMapChanged(Map newMap)
        {
            backingField.OnMapChanged(newMap);
        }

        /// <summary>
        /// Gets the texture from using the specified object type
        /// </summary>
        /// <param name="objectType">The object type</param>
        /// <returns>The sprite</returns>
        protected virtual Sprite GetTextureFrom(ScriptableObject objectType)
        {
            return null;
        }
    }
}
