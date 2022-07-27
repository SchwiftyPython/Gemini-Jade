using UnityEngine;
using World.Things;

namespace World
{
    public class GridObject : Thing
    {
        public PlacedObject PlacedObject { get; }

        public GridObject(PlacedObject placedObject, Vector3 position, bool isStatic, bool isWalkable, bool isTransparent) : base(position,
            MapLayer.GridObject, isStatic, isWalkable, isTransparent)
        {
            PlacedObject = placedObject;
        }
        
        public GridObject(PlacedObject placedObject, MapLayer layer, Vector3 position, bool isStatic, bool isWalkable, bool isTransparent) : base(position,
            layer, isStatic, isWalkable, isTransparent)
        {
            PlacedObject = placedObject;
        }

        public bool IsBlueprint()
        {
            return PlacedObject.NeedsToBeMade;
        }
    }
}
