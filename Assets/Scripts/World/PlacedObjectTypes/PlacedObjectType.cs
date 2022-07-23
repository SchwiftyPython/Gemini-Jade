using UnityEngine;

namespace World.PlacedObjectTypes
{
    [CreateAssetMenu(menuName = "My Assets/Placed Object Type")]
    public class PlacedObjectType : ScriptableObject
    {
        public string label;
    
        public Transform prefab;

        public Sprite texture; //todo could make this a dictionary for all the different material types or do a recolor with shader
        
        /// <summary>
        /// Whether or not the object is considered "transparent", eg. whether or not light passes through it.
        /// </summary>
        public bool transparent;

        /// <summary>
        /// Whether or not the object is to be considered "walkable", eg. whether or not the square it resides
        /// on can be traversed by other, non-walkable objects on the same <see cref="Map"/>.  Effectively, whether or not this
        /// object collides.
        /// </summary>
        public bool walkable;

        public int width = 1;
    
        public int height = 1;
    }
}
