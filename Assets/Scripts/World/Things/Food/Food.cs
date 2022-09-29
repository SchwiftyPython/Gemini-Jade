using GoRogue;
using Settings;

namespace World.Things.Food
{
    public class Food : StackThing
    {
        public int RemainingShelfLife { get; }

        private FoodTemplate FoodTemplate => (FoodTemplate) template;

        public int Nutrition => FoodTemplate.nutrition;

        public Food(FoodTemplate thingTemplate, Coord position, int count) : base(thingTemplate, position, count)
        {
            RemainingShelfLife = thingTemplate.shelfLifeDays * Constants.TicksPerDay;
        }

        public override void Tick()
        {
            //todo need to update remaining shelf life
        }
    }
}
