using System;
using System.Collections.Generic;
using System.Linq;
using GoRogue;
using GoRogue.GameFramework;
using UnityEngine;
using World.TileTypes;
using GameObject = GoRogue.GameFramework.GameObject;

namespace World
{
    public class Tile : IGameObject
    {
        protected IGameObject _backingField;

        public uint ID => _backingField.ID;

        public int Layer => _backingField.Layer;
        
        public Map CurrentMap => _backingField.CurrentMap;

        public bool IsStatic => _backingField.IsStatic;

        public bool IsTransparent 
        {
            get => _backingField.IsTransparent;
            set => _backingField.IsTransparent = value;
        }

        public bool IsWalkable 
        {
            get => _backingField.IsWalkable;
            set => _backingField.IsWalkable = value;
        }

        public Coord Position 
        {
            get => _backingField.Position;
            set => _backingField.Position = value;
        }

        public event EventHandler<ItemMovedEventArgs<IGameObject>> Moved 
        {
            add => _backingField.Moved += value;
            remove => _backingField.Moved -= value;
        }
        
        public Sprite Texture { get; set; }

        public UnityEngine.GameObject SpriteInstance { get; private set; }
        
        
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
        public Tile(Coord position, TileType tileType)
        {
            _backingField = new GameObject(position, 0, this, true,
                tileType.walkable, tileType.transparent);
            
            Texture = tileType.GetTexture();
        }
        
        public void SetSpriteInstance(UnityEngine.GameObject instance)
        {
            SpriteInstance = instance;
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

        public void AddComponent(object component)
        {
            _backingField.AddComponent(component);
        }

        public T GetComponent<T>()
        {
            return _backingField.GetComponent<T>();
        }

        public IEnumerable<T> GetComponents<T>()
        {
            return _backingField.GetComponents<T>();
        }

        public bool HasComponent(Type componentType)
        {
            return _backingField.HasComponent(componentType);
        }

        public bool HasComponent<T>()
        {
            return _backingField.HasComponent<T>();
        }

        public bool HasComponents(params Type[] componentTypes)
        {
            return _backingField.HasComponents(componentTypes);
        }

        public void RemoveComponent(object component)
        {
            _backingField.RemoveComponent(component);
        }

        public void RemoveComponents(params object[] components)
        {
            _backingField.RemoveComponents(components);
        }

        public bool MoveIn(Direction direction)
        {
            return _backingField.MoveIn(direction);
        }

        public void OnMapChanged(Map newMap)
        {
            _backingField.OnMapChanged(newMap);
        }
    }
}
