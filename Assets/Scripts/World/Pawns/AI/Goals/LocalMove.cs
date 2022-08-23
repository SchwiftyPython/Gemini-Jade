using GoRogue;

namespace World.Pawns.AI.Goals
{
    /// <summary>
    /// 
    /// </summary>
    public class LocalMove : Goal
    {
        /// <summary>
        /// The movement
        /// </summary>
        private readonly PawnMovement _movement;
        
        /// <summary>
        /// The target
        /// </summary>
        private readonly Coord _target;

        /// <summary>
        /// The finished
        /// </summary>
        private bool _finished;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="movement"></param>
        /// <param name="target"></param>
        public LocalMove(PawnMovement movement, Coord target)
        {
            _finished = false;

            _movement = movement;
            
            _target = target;
        }

        /// <summary>
        /// Describes whether this instance finished
        /// </summary>
        /// <returns>The finished</returns>
        public override bool Finished()
        {
            return _finished;
        }

        /// <summary>
        /// Takes the action
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
        /// Ons the destination reached
        /// </summary>
        private void OnDestinationReached()
        {
            _movement.onDestinationReached -= OnDestinationReached;
            
            _movement.onDestinationUnreachable -= OnDestinationUnreachable;

            _inProgress = false;
            
            _finished = true;
        }
        
        /// <summary>
        /// Ons the destination unreachable
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
