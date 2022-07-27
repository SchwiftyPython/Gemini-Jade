using UnityEngine;

namespace World.Things.CraftableThings
{
    [CreateAssetMenu(menuName = "Templates/Placed Object Template")]
    public class PlacedObjectTemplate : CraftableTemplate
    {
        public Sprite blueprintTexture;

        public int width = 1;
    
        public int height = 1;
    }
}
