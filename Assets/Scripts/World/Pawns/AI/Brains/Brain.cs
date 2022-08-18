using System.Collections.Generic;
using UnityEngine;
using World.Pawns.AI.Goals;

namespace World.Pawns.AI.Brains
{
    public class Brain
    {
        //todo brain types

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

                //testing Wander

                Debug.Log("No goals, testing wandering...");

               new Wander().Push(this);
            }

            while (Goals.Count > 0 && Goals.Peek().Finished())
            {
                Goals.Pop();
            }

            if (Goals.Count > 0)
            {
                if (Goals.Peek().InProgress())
                {
                    return;
                }

                Goals.Peek().TakeAction();
            }
        }

        public void AddGoal(Goal goal)
        {
            if (goal.brain == null)
            {
                goal.brain = this;
            }
            
            if (Goals == null)
            {
                Goals = new Stack<Goal>();
            }
            
            Goals.Push(goal);
        }

        public void AddPriorityGoal(Goal goal)
        {
            if (goal.brain == null)
            {
                goal.brain = this;
            }

            Goals = new Stack<Goal>();

            Goals.Push(goal);
        }
    }
}
