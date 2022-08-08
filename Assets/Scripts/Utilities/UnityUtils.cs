using System;
using GoRogue;
using UnityEngine;
using World;
using Object = UnityEngine.Object;

namespace Utilities
{
    public static class UnityUtils
    {
        public static void DestroyAllChildren(this GameObject parent)
        {
            for (var i = 0; i < parent.transform.childCount; i++)
            {
                Object.Destroy(parent.transform.GetChild(i).gameObject);
            }
        }
        
        public static Coord ToCoord(this Vector2Int position)
        {
            return new Coord(position.x, position.y);
        }
        
        public static Coord ToCoord(this Vector3 position)
        {
            return new Coord((int) position.x, (int) position.y);
        }

        public static Vector3 ToVector3(this Vector2Int position)
        {
            return new Vector3(position.x, position.y);
        }
        
        public static Vector2Int ToVector2Int(this Vector3 position)
        {
            return new Vector2Int((int) position.x, (int) position.y);
        }

        public static Direction ToMapDirection(this BitMaskDirection bitMaskDirection)
        {
            return bitMaskDirection switch
            {
                BitMaskDirection.NorthWest => Direction.UP_LEFT,
                BitMaskDirection.North => Direction.UP,
                BitMaskDirection.NorthEast => Direction.UP_RIGHT,
                BitMaskDirection.West => Direction.LEFT,
                BitMaskDirection.East => Direction.RIGHT,
                BitMaskDirection.SouthWest => Direction.DOWN_LEFT,
                BitMaskDirection.South => Direction.DOWN,
                BitMaskDirection.SouthEast => Direction.DOWN_RIGHT,
                _ => throw new ArgumentOutOfRangeException(nameof(bitMaskDirection), bitMaskDirection, null)
            };
        }
    }
}
