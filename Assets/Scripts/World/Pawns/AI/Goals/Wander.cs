using GoRogue;
using UnityEngine;
using Utilities;

namespace World.Pawns.AI.Goals
{
    /// <summary>
    /// The wander class
    /// </summary>
    /// <seealso cref="Goal"/>
    public class Wander : Goal
    {
        /// <summary>
        /// The wander radius
        /// </summary>
        private const float WanderRadius = 10f;

        /// <summary>
        /// Takes the action
        /// </summary>
        public override void TakeAction()
        {
            //todo pause for a bit
            
            _inProgress = true;
            
            var target = PickRandomPoint();
        
            var localMove = new LocalMove(Pawn.Movement, target);
        
            PushChildGoal(localMove);

            _inProgress = false;
        }

        /// <summary>
        /// Describes whether this instance finished
        /// </summary>
        /// <returns>The bool</returns>
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
    }
}
