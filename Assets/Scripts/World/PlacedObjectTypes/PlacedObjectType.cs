using UnityEngine;

namespace World.PlacedObjectTypes
{
    [CreateAssetMenu(menuName = "My Assets/Placed Object Type")]
    public class PlacedObjectType : ScriptableObject
    {
        public string nameString;
    
        public Transform prefab;
    
        public int width;
    
        public int height;
    }
}
