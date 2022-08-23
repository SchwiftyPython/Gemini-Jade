using System.Collections.Generic;
using System.Linq;
using World.Pawns.AI.Goals;
using World.Pawns.Jobs;

namespace World.Pawns.AI.Brains
{
    /// <summary>
    /// The brain class
    /// </summary>
    public class Brain
    {
        //todo brain types

        /// <summary>
        /// Gets or sets the value of the goals
        /// </summary>
        public Stack<Goal> Goals { get; private set; }

        /// <summary>
        /// Gets the value of the pawn
        /// </summary>
        public Pawn Pawn { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Brain"/> class
        /// </summary>
        /// <param name="pawn">The pawn</param>
        public Brain(Pawn pawn)
        {
            Pawn = pawn;
            
            Goals = new Stack<Goal>();
        }

        /// <summary>
        /// Thinks this instance
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
        /// Adds the goal using the specified goal
        /// </summary>
        /// <param name="goal">The goal</param>
        public void AddGoal(Goal goal)
        {
            goal.brain ??= this;
            
            Goals ??= new Stack<Goal>();
            
            Goals.Push(goal);
        }

        /// <summary>
        /// Adds the priority goal using the specified p goal
        /// </summary>
        /// <param name="pGoal">The goal</param>
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
        /// Describes whether this instance has job goal
        /// </summary>
        /// <returns>The bool</returns>
        public bool HasJobGoal()
        {
            return Goals.Any(g => g is JobGoal);
        }

        /// <summary>
        /// Gets the current job
        /// </summary>
        /// <returns>The job</returns>
        public Job GetCurrentJob()
        {
            var jobGoal = Goals.SingleOrDefault(g => g is JobGoal);

            return ((JobGoal) jobGoal)?.Job;
        }
    }
}
