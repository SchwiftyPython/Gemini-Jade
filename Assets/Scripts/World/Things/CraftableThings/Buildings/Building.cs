using UnityEngine;

namespace World.Things.CraftableThings.Buildings
{
    /// <summary>
    /// The building class
    /// </summary>
    /// <seealso cref="GridObject"/>
    public class Building : GridObject
    {
        //not sure if this class will be needed
        
        /// <summary>
        /// Initializes a new instance of the <see cref="Building"/> class
        /// </summary>
        /// <param name="placedObject">The placed object</param>
        /// <param name="position">The position</param>
        /// <param name="isStatic">The is static</param>
        /// <param name="isWalkable">The is walkable</param>
        /// <param name="isTransparent">The is transparent</param>
        public Building(PlacedObject placedObject, Vector3 position, bool isStatic, bool isWalkable, bool isTransparent) : base(placedObject, position, isStatic, isWalkable, isTransparent)
        {
        }
    }
}
