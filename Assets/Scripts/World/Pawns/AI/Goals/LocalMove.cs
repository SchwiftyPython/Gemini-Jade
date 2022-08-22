using GoRogue;

namespace World.Pawns.AI.Goals
{
    public class LocalMove : Goal
    {
        private PawnMovement _movement;
        
        private Coord _target;

        private bool _finished;

        public LocalMove(PawnMovement movement, Coord target)
        {
            _finished = false;

            _movement = movement;
            
            _target = target;
        }

        public override bool Finished()
        {
            return _finished;
        }

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
        
        private void OnDestinationReached()
        {
            _movement.onDestinationReached -= OnDestinationReached;
            
            _movement.onDestinationUnreachable -= OnDestinationUnreachable;

            _inProgress = false;
            
            _finished = true;
        }
        
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
