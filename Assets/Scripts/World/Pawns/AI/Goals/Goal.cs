using System;
using UnityEngine;
using World.Pawns.AI.Brains;

namespace World.Pawns.AI.Goals
{
    public class Goal
    {
        public Goal parentGoal;
        
        public Brain brain;

        public Pawn Pawn => brain.Pawn;
        
        public Goal(){}
        
        public virtual void Create(){}

        public virtual bool Finished()
        {
            return true;
        }
        
        public virtual void TakeAction(){}

        public void Pop()
        {
            if (brain.Goals.Count > 0)
            {
                brain.Goals.Pop();
            }
        }

        public virtual void PushGoal(Goal goal)
        {
            
        }

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
        
        public virtual void Failed(){}

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
