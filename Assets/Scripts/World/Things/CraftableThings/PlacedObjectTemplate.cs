using UnityEngine;

namespace World.Things.CraftableThings
{
    /// <summary>
    /// The placed object template class
    /// </summary>
    /// <seealso cref="CraftableTemplate"/>
    [CreateAssetMenu(menuName = "Templates/Placed Object Template")]
    public class PlacedObjectTemplate : CraftableTemplate
    {
        /// <summary>
        /// The blueprint texture
        /// </summary>
        public Sprite blueprintTexture;
        
        /// <summary>
        /// The built texture
        /// </summary>
        public Sprite builtTexture;

        /// <summary>
        /// The width
        /// </summary>
        public int width = 1;
    
        /// <summary>
        /// The height
        /// </summary>
        public int height = 1;

        /// <summary>
        /// The is wall
        /// </summary>
        public bool isWall = false;
    }
}
