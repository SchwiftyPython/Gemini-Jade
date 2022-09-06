using GoRogue;
using UnityEngine;

namespace World.Pawns.AI.Goals
{
    /// <summary>
    /// Moves a <see cref="Pawn"/> to a walkable <see cref="Coord"/> on their current <see cref="LocalMap"/>
    /// </summary>
    public class LocalMove : Goal
    {
        /// <summary>
        /// Reference to <see cref="Pawn"/>'s <see cref="PawnMovement"/> instance
        /// </summary>
        private readonly PawnMovement _movement;
        
        /// <summary>
        /// Target walkable <see cref="Coord"/> <see cref="Pawn"/> is moving to
        /// </summary>
        private readonly Coord _target;

        /// <summary>
        /// Returns true if finished
        /// </summary>
        private bool _finished;

        /// <summary>
        /// Constructor for <see cref="LocalMove"/>
        /// </summary>
        /// <param name="movement">Reference to <see cref="Pawn"/>'s <see cref="PawnMovement"/> instance</param>
        /// <param name="target">Target walkable <see cref="Coord"/> <see cref="Pawn"/> is moving to</param>
        public LocalMove(PawnMovement movement, Coord target)
        {
            _finished = false;

            _movement = movement;
            
            _target = target;
        }

        /// <summary>
        /// Checks if finished
        /// </summary>
        /// <returns>True if finished</returns>
        public override bool Finished()
        {
            return _finished;
        }

        /// <summary>
        /// Attempts to move <see cref="Pawn"/> to target <see cref="Coord"/>. If Pawn cannot reach
        /// target then <see cref="LocalMove"/> fails.
        /// </summary>
        public override void TakeAction()
        {
            _inProgress = true;

            if (Pawn.IsImmobile())
            {
                FailToParent();
                return;
            }
            
            _movement.onDestinationReached += OnDestinationReached;
            
            _movement.onDestinationUnreachable += OnDestinationUnreachable;

            Pawn.Movement.MoveTo(_target);
        }
        
        /// <summary>
        /// Handles business once <see cref="Pawn"/> reaches target
        /// </summary>
        private void OnDestinationReached()
        {
            _movement.onDestinationReached -= OnDestinationReached;
            
            _movement.onDestinationUnreachable -= OnDestinationUnreachable;

            _inProgress = false;
            
            _finished = true;
        }
        
        /// <summary>
        /// Handles failure to reach target. If <see cref="LocalMove"/> is a child <see cref="Goal"/>
        /// then it fails to parent. Otherwise, it is just removed from Goals
        /// </summary>
        private void OnDestinationUnreachable()
        {
            _movement.onDestinationReached -= OnDestinationReached;
            
            _movement.onDestinationUnreachable -= OnDestinationUnreachable;

            if (parentGoal == null)
            {
                Pop();
            }
            else
            {
                FailToParent();
            }
        }
    }
}
