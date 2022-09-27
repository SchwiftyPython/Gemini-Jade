using UnityEngine;

namespace World.Things.Food
{
    [CreateAssetMenu(menuName = "Create FoodTemplate", fileName = "FoodTemplate")]
    public class FoodTemplate : ThingTemplate
    {
        public int shelfLifeDays = 20;

        public int nutrition = 15;
    }
}
