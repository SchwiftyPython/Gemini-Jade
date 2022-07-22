using UnityEngine;
using World.PlacedObjectTypes;

namespace World
{
    public class PlacedObject : MonoBehaviour
    {
        public static PlacedObject Create(Vector3 gridPosition, Dir direction, PlacedObjectType placedObjectType)
        {
            var placedObjectInstance = Instantiate(placedObjectType.prefab, gridPosition, Quaternion.Euler(0, 0, 0));
            
            PlacedObject placedObject = placedObjectInstance.GetComponent<PlacedObject>();

            placedObject.placedObjectType = placedObjectType;
            placedObject.gridPosition = gridPosition;
            placedObject.direction = direction;

            return placedObject;

        }

        public static Dir GetNextDir(Dir dir)
        {
            switch (dir)
            {
                default:
                case Dir.Down: return Dir.Left;
                case Dir.Left: return Dir.Up;
                case Dir.Up: return Dir.Right;
                case Dir.Right: return Dir.Down;
            }
        }

        public static Vector2Int GetDirForwardVector(Dir dir)
        {
            switch (dir)
            {
                default:
                case Dir.Down: return new Vector2Int(0, -1);
                case Dir.Left: return new Vector2Int(-1, 0);
                case Dir.Up: return new Vector2Int(0, +1);
                case Dir.Right: return new Vector2Int(+1, 0);
            }
        }

        public static Dir GetDir(Vector2Int from, Vector2Int to)
        {
            if (from.x < to.x)
            {
                return Dir.Right;
            }

            if (from.x > to.x)
            {
                return Dir.Left;
            }

            return from.y < to.y ? Dir.Up : Dir.Down;
        }
        
        public static int GetRotationAngle(Dir dir) 
        {
            switch (dir) 
            {
                default:
                case Dir.Down:  return 0;
                case Dir.Left:  return 90;
                case Dir.Up:    return 180;
                case Dir.Right: return 270;
            }
        }
    
        public enum Dir 
        {
            Down,
            Left,
            Up,
            Right,
        }

        protected PlacedObjectType placedObjectType;

        protected Vector3 gridPosition;
        
        protected Dir direction;
    }
}
