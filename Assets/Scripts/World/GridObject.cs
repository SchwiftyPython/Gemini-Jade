using UnityEngine;
using Utilities;
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
        
        public bool IsWall()
        {
            var wallObject = PlacedObject as WallPlacedObject;
            
            return wallObject != null;
        }

        public bool HasWallNeighborTo(BitMaskDirection bitMaskDirection)
        {
            var mapDirection = bitMaskDirection.ToMapDirection();

            var targetCoord = Position + mapDirection;

            var localMap = Object.FindObjectOfType<GridBuildingSystem>().LocalMap;
            
            var neighbor = localMap.GetGridObjectAt(targetCoord);
            
            return neighbor != null && neighbor.IsWall();
        }
    }
}
