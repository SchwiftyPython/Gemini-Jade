using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using World.Pawns.AI.Goals;
using World.Pawns.Jobs;

namespace World.Pawns.AI.Brains
{
    /// <summary>
    /// Represents a <see cref="Pawn"/>'s thinkin'
    /// </summary>
    public class Brain
    {
        //todo brain types

        /// <summary>
        /// Collection of current <see cref="Goal"/>s 
        /// </summary>
        public Stack<Goal> Goals { get; private set; }

        /// <summary>
        /// Brain's owner
        /// </summary>
        public Pawn Pawn { get; }

        /// <summary>
        /// <see cref="Brain"/> class constructor
        /// </summary>
        /// <param name="pawn">The pawn using the brain</param>
        public Brain(Pawn pawn)
        {
            Pawn = pawn;
            
            Goals = new Stack<Goal>();
        }

        /// <summary>
        /// Determines next actions based on what <see cref="Goal"/>s are in the stack
        /// </summary>
        public void Think()
        {
            Goals ??= new Stack<Goal>();
            
            if (Goals.Count == 0)
            {
                //todo bored goal

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

        /// <summary>
        /// Adds the specified goal
        /// </summary>
        /// <param name="goal">The goal to add</param>
        public void AddGoal(Goal goal)
        {
            goal.brain ??= this;
            
            Goals ??= new Stack<Goal>();
            
            Goals.Push(goal);
        }

        /// <summary>
        /// Adds a goal as a priority. Clears all current goals.
        /// </summary>
        /// <param name="pGoal">The <see cref="Goal"/> to add</param>
        public void AddPriorityGoal(Goal pGoal)
        {
            var jobGoals = Goals.Where(g => g is JobGoal);

            foreach (var jobGoal in jobGoals)
            {
                ((JobGoal)jobGoal).Job.UnAssignPawn();
            }
            
            pGoal.brain ??= this;

            Goals = new Stack<Goal>();

            Goals.Push(pGoal);
        }

        /// <summary>
        /// Checks for existence of a <see cref="JobGoal"/>
        /// </summary>
        /// <returns>Returns true if any of the current goals are job goals</returns>
        public bool HasJobGoal()
        {
            return Goals.Any(g => g is JobGoal);
        }

        /// <summary>
        /// Gets the current job
        /// </summary>
        /// <returns>A <see cref="Job"/> if one exists. Null if one doesn't exist</returns>
        public Job GetCurrentJob()
        {
            var jobGoal = Goals.SingleOrDefault(g => g is JobGoal);

            return ((JobGoal) jobGoal)?.Job;
        }
    }
}
