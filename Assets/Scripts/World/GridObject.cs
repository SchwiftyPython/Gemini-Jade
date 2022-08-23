using System.Collections.Generic;
using GoRogue;
using UnityEngine;
using Utilities;
using World.Things;

namespace World
{
    /// <summary>
    /// The grid object class
    /// </summary>
    /// <seealso cref="Thing"/>
    public class GridObject : Thing
    {
        /// <summary>
        /// Gets the value of the placed object
        /// </summary>
        public PlacedObject PlacedObject { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GridObject"/> class
        /// </summary>
        /// <param name="placedObject">The placed object</param>
        /// <param name="position">The position</param>
        /// <param name="isStatic">The is static</param>
        /// <param name="isWalkable">The is walkable</param>
        /// <param name="isTransparent">The is transparent</param>
        public GridObject(PlacedObject placedObject, Vector3 position, bool isStatic, bool isWalkable, bool isTransparent) : base(position,
            MapLayer.GridObject, isStatic, isWalkable, isTransparent)
        {
            PlacedObject = placedObject;
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="GridObject"/> class
        /// </summary>
        /// <param name="placedObject">The placed object</param>
        /// <param name="layer">The layer</param>
        /// <param name="position">The position</param>
        /// <param name="isStatic">The is static</param>
        /// <param name="isWalkable">The is walkable</param>
        /// <param name="isTransparent">The is transparent</param>
        public GridObject(PlacedObject placedObject, MapLayer layer, Vector3 position, bool isStatic, bool isWalkable, bool isTransparent) : base(position,
            layer, isStatic, isWalkable, isTransparent)
        {
            PlacedObject = placedObject;
        }

        /// <summary>
        /// Describes whether this instance is blueprint
        /// </summary>
        /// <returns>The bool</returns>
        public bool IsBlueprint()
        {
            return PlacedObject.NeedsToBeMade;
        }
        
        /// <summary>
        /// Describes whether this instance is wall
        /// </summary>
        /// <returns>The bool</returns>
        public bool IsWall()
        {
            var wallObject = PlacedObject as WallPlacedObject;
            
            return wallObject != null;
        }

        /// <summary>
        /// Describes whether this instance has wall neighbor to
        /// </summary>
        /// <param name="bitMaskDirection">The bit mask direction</param>
        /// <returns>The bool</returns>
        public bool HasWallNeighborTo(BitMaskDirection bitMaskDirection)
        {
            var mapDirection = bitMaskDirection.ToMapDirection();

            var targetCoord = Position + mapDirection;

            var localMap = Object.FindObjectOfType<GridBuildingSystem>().LocalMap;
            
            var neighbor = localMap.GetGridObjectAt(targetCoord);
            
            return neighbor != null && neighbor.IsWall();
        }

        /// <summary>
        /// Finishes the construction
        /// </summary>
        public void FinishConstruction()
        {
            if (PlacedObject.placedObjectType.isWall)
            {
                ((WallPlacedObject)PlacedObject).FinishConstruction();
            }
            else
            {
                PlacedObject.FinishConstruction();
            }
        }
    }
}
