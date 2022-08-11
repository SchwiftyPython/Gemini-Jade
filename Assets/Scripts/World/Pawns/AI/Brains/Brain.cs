using System.Collections.Generic;
using World.Pawns.AI.Goals;

namespace World.Pawns.AI.Brains
{
    public class Brain
    {
        //todo brain types

        private bool testGoalInProgress;

        public Stack<Goal> Goals { get; private set; }

        public Pawn Pawn { get; }

        public Brain(Pawn pawn)
        {
            Pawn = pawn;
            
            Goals = new Stack<Goal>();
        }

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
                
                //testing LocalMove

                var target = ((LocalMap) Pawn.CurrentMap).GetRandomTile(true);
                
                new LocalMove(Pawn.Movement, target.Position).Push(this);
            }

            while (Goals.Count > 0 && Goals.Peek().Finished())
            {
                Goals.Pop();
                
                testGoalInProgress = false;
            }

            if (Goals.Count > 0)
            {
                //todo need to guard against taking action if goal is in progress
                //This would likely be checked already in Pawn.Tick(). 

                if (testGoalInProgress)
                {
                    return;
                }
                
                Goals.Peek().TakeAction();
                
                testGoalInProgress = true;
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
