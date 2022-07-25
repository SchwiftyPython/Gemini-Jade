using GoRogue;
using UnityEngine;

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

        public static Vector3 ToVector3(this Vector2Int position)
        {
            return new Vector3(position.x, position.y);
        }
        
        public static Vector2Int ToVector2Int(this Vector3 position)
        {
            return new Vector2Int((int) position.x, (int) position.y);
        }
    }
}
