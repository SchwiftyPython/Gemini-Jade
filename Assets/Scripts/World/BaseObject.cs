using System;
using System.Collections.Generic;
using GoRogue;
using GoRogue.GameFramework;
using UnityEngine;
using GameObject = GoRogue.GameFramework.GameObject;

namespace World
{
    public class BaseObject : IGameObject
    {
        protected IGameObject backingField;

        public uint ID => backingField.ID;

        public int Layer => backingField.Layer;
        
        public Map CurrentMap => backingField.CurrentMap;

        public bool IsStatic => backingField.IsStatic;

        public bool IsTransparent 
        {
            get => backingField.IsTransparent;
            set => backingField.IsTransparent = value;
        }

        public bool IsWalkable 
        {
            get => backingField.IsWalkable;
            set => backingField.IsWalkable = value;
        }

        public Coord Position 
        {
            get => backingField.Position;
            set => backingField.Position = value;
        }

        public event EventHandler<ItemMovedEventArgs<IGameObject>> Moved 
        {
            add => backingField.Moved += value;
            remove => backingField.Moved -= value;
        }
        
        public Sprite Texture { get; set; }

        public UnityEngine.GameObject SpriteInstance { get; private set; }

        public BaseObject(Coord position, MapLayer layer, bool isStatic, bool isWalkable, bool isTransparent)
        {
            backingField = new GameObject(position, (int) layer, this, isStatic, isWalkable, isTransparent);
        }
        
        protected BaseObject(Vector3 position, MapLayer layer, bool isStatic, bool isWalkable, bool isTransparent)
        {
            var coord = new Coord((int) position.x, (int) position.y);
            
            backingField = new GameObject(coord, (int) layer, this, isStatic, isWalkable, isTransparent);
        }
        
        protected BaseObject(MapLayer layer, bool isStatic, bool isWalkable, bool isTransparent)
        {
            var coord = new Coord();
            
            backingField = new GameObject(coord, (int) layer, this, isStatic, isWalkable, isTransparent);
        }

        protected BaseObject()
        {
        }

        public void SetSpriteInstance(UnityEngine.GameObject instance)
        {
            SpriteInstance = instance;
        }

        public void AddComponent(object component)
        {
            backingField.AddComponent(component);
        }

        public T GetComponent<T>()
        {
            return backingField.GetComponent<T>();
        }

        public IEnumerable<T> GetComponents<T>()
        {
            return backingField.GetComponents<T>();
        }

        public bool HasComponent(Type componentType)
        {
            return backingField.HasComponent(componentType);
        }

        public bool HasComponent<T>()
        {
            return backingField.HasComponent<T>();
        }

        public bool HasComponents(params Type[] componentTypes)
        {
            return backingField.HasComponents(componentTypes);
        }

        public void RemoveComponent(object component)
        {
            backingField.RemoveComponent(component);
        }

        public void RemoveComponents(params object[] components)
        {
            backingField.RemoveComponents(components);
        }

        public bool MoveIn(Direction direction)
        {
            return backingField.MoveIn(direction);
        }

        public void OnMapChanged(Map newMap)
        {
            backingField.OnMapChanged(newMap);
        }

        protected virtual Sprite GetTextureFrom(ScriptableObject objectType)
        {
            return null;
        }
    }
}
