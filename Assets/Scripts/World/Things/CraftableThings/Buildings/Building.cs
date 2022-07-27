using UnityEngine;

namespace World.Things.CraftableThings.Buildings
{
    public class Building : GridObject
    {
        //not sure if this class will be needed
        
        public Building(PlacedObject placedObject, Vector3 position, bool isStatic, bool isWalkable, bool isTransparent) : base(placedObject, position, isStatic, isWalkable, isTransparent)
        {
        }
    }
}
