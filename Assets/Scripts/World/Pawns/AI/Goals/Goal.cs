using System;
using UnityEngine;
using World.Pawns.AI.Brains;

namespace World.Pawns.AI.Goals
{
    /// <summary>
    /// The goal class
    /// </summary>
    public class Goal
    {
        /// <summary>
        /// The in progress
        /// </summary>
        protected bool _inProgress;
        
        /// <summary>
        /// The parent goal
        /// </summary>
        public Goal parentGoal;
        
        /// <summary>
        /// The brain
        /// </summary>
        public Brain brain;

        /// <summary>
        /// Gets the value of the pawn
        /// </summary>
        public Pawn Pawn
        {
            get => brain.Pawn;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Goal"/> class
        /// </summary>
        public Goal(){}
        
        /// <summary>
        /// Creates this instance
        /// </summary>
        public virtual void Create(){}

        /// <summary>
        /// Describes whether this instance finished
        /// </summary>
        /// <returns>The bool</returns>
        public virtual bool Finished()
        {
            return true;
        }

        /// <summary>
        /// Describes whether this instance in progress
        /// </summary>
        /// <returns>The in progress</returns>
        public virtual bool InProgress()
        {
            return _inProgress;
        }
        
        /// <summary>
        /// Takes the action
        /// </summary>
        public virtual void TakeAction(){}

        /// <summary>
        /// Pops this instance
        /// </summary>
        public void Pop()
        {
            if (brain.Goals.Count > 0)
            {
                brain.Goals.Pop();
            }
        }

        /// <summary>
        /// Pushes the goal using the specified goal
        /// </summary>
        /// <param name="goal">The goal</param>
        public virtual void PushGoal(Goal goal)
        {
            goal.Push(brain);
        }

        /// <summary>
        /// Pushes the child goal using the specified child
        /// </summary>
        /// <param name="child">The child</param>
        public virtual void PushChildGoal(Goal child)
        {
            child.parentGoal = this;
            
            child.Push(brain);
        }
        
        /// <summary>
        /// Pushes the child goal using the specified child
        /// </summary>
        /// <param name="child">The child</param>
        /// <param name="parent">The parent</param>
        public virtual void PushChildGoal(Goal child, Goal parent)
        {
            child.parentGoal = parent;
            
            child.Push(brain);
        }

        /// <summary>
        /// Pushes the new brain
        /// </summary>
        /// <param name="newBrain">The new brain</param>
        public virtual void Push(Brain newBrain)
        {
            if (newBrain == null)
            {
                return;
            }

            try
            {
                brain = newBrain;

                brain.AddGoal(this);

                Create();
            }
            catch(Exception e)
            {
                Debug.LogError($@"Pawn: {Pawn.id}, Number of Goals: {brain.Goals.Count}");
                
                Debug.LogError(e.InnerException);
            }
        }
        
        /// <summary>
        /// Faileds this instance
        /// </summary>
        public virtual void Failed(){}

        /// <summary>
        /// Fails the to parent
        /// </summary>
        public void FailToParent()
        {
            while(brain.Goals.Count > 0 && brain.Goals.Peek() != parentGoal)
            {
                brain.Goals.Pop();
            }
            
            if (brain.Goals.Count > 0)
            {
                brain.Goals.Peek().Failed();
            }
        }
    }
}
