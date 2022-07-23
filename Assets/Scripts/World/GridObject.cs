using UnityEngine;

namespace World
{
    public class GridObject : BaseObject
    {
        public GridObject(Vector3 position, bool isStatic, bool isWalkable, bool isTransparent) : base(position,
            MapLayer.GridObject, isStatic, isWalkable, isTransparent)
        {
        }
    }
}
