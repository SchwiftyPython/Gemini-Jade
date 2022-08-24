using GoRogue;
using UnityEngine;
using Utilities;

namespace World.Pawns.AI.Goals
{
    /// <summary>
    /// Moves <see cref="Pawn"/> around a specified radius with small pauses in between.
    /// This Goal runs until it is canceled.
    /// </summary>
    /// <seealso cref="Goal"/>
    public class Wander : Goal
    {
        /// <summary>
        /// The wander radius
        /// </summary>
        private const float WanderRadius = 10f;

        /// <summary>
        /// Moves to some <see cref="Coord"/> within the radius after waiting a short time.
        /// </summary>
        public override void TakeAction()
        {
            _inProgress = true;
            
            var waitTime = Random.Range(1f, 4f);

            FunctionTimer.Create(AddLocalMoveChildGoal, waitTime);
        }

        /// <summary>
        /// Checks if finished
        /// </summary>
        /// <returns>Always returns false</returns>
        public override bool Finished()
        {
            return false;
        }

        /// <summary>
        /// Picks the random point
        /// </summary>
        /// <returns>The coord</returns>
        private Coord PickRandomPoint()
        {
            var point = Random.insideUnitSphere * WanderRadius;

            return Pawn.Position + point.ToCoord();
        }
        
        /// <summary>
        /// Adds <see cref="LocalMove"/> as a Child <see cref="Goal"/> with a random <see cref="Coord"/>
        /// from within the wander radius
        /// </summary>
        private void AddLocalMoveChildGoal()
        {
            var target = PickRandomPoint();
            
            var localMove = new LocalMove(Pawn.Movement, target);
        
            PushChildGoal(localMove);
            
            _inProgress = false;
        }
    }
}
