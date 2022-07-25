using UnityEngine;

namespace World
{
    public class GridObject : BaseObject
    {
        private PlacedObject _placedObject;
        
        public GridObject(PlacedObject placedObject, Vector3 position, bool isStatic, bool isWalkable, bool isTransparent) : base(position,
            MapLayer.GridObject, isStatic, isWalkable, isTransparent)
        {
            _placedObject = placedObject;
        }
    }
}
