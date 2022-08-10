using System.Collections.Generic;
using World.Pawns.AI.Goals;

namespace World.Pawns.AI.Brains
{
    public class Brain
    {
        //todo brain types
        
        private Pawn _pawn;

        public Stack<Goal> Goals { get; private set; }

        public void Think()
        {
            if (Goals == null)
            {
                Goals = new Stack<Goal>();
            }
            
            if (Goals.Count == 0)
            {
                //todo bored goal
                
                //_goals.Push(new Goal());
            }
        }

        public void AddGoal(Goal goal)
        {
            if (Goals == null)
            {
                Goals = new Stack<Goal>();
            }
            
            Goals.Push(goal);
        }
    }
}
