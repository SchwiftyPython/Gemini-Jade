using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using World.Pawns.AI.Goals;
using World.Pawns.Jobs;

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

        public void AddPriorityGoal(Goal pGoal)
        {
            var jobGoals = Goals.Where(g => g is JobGoal);

            foreach (var jobGoal in jobGoals)
            {
                ((JobGoal)jobGoal).Job.UnAssignPawn();
            }
            
            if (pGoal.brain == null)
            {
                pGoal.brain = this;
            }

            Goals = new Stack<Goal>();

            Goals.Push(pGoal);
        }

        public bool HasJobGoal()
        {
            return Goals.Any(g => g is JobGoal);
        }

        public Job GetCurrentJob()
        {
            var jobGoal = Goals.SingleOrDefault(g => g is JobGoal);

            return ((JobGoal) jobGoal)?.Job;
        }
    }
}
