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
    }
}
