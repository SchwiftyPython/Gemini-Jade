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
            _movement = movement;
            
            _target = target;
            
            _movement.onDestinationReached += OnDestinationReached;
        }

        public override bool Finished()
        {
            return _finished;
        }

        public override void TakeAction()
        {
            if (Pawn.IsImmobile())
            {
                FailToParent();
                return;
            }
            
            Pawn.MoveToLocal(_target);
        }
        
        private void OnDestinationReached()
        {
            _movement.onDestinationReached -= OnDestinationReached;
            
            _finished = true;
        }
    }
}
