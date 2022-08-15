using GoRogue;
using UnityEngine;
using Utilities;

namespace World.Pawns.AI.Goals
{
    public class Wander : Goal
    {
        private const float WanderRadius = 10f;
    
        public Wander(){}

        public override void TakeAction()
        {
            //todo pause for a bit
            
            _inProgress = true;
            
            var target = PickRandomPoint();
        
            var localMove = new LocalMove(Pawn.Movement, target);
        
            PushChildGoal(localMove);

            _inProgress = false;
        }

        public override bool Finished()
        {
            return false;
        }

        private Coord PickRandomPoint()
        {
            var point = Random.insideUnitSphere * WanderRadius;

            return Pawn.Position + point.ToCoord();
        }
    }
}
