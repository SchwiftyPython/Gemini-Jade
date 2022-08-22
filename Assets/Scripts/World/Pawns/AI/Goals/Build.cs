using GoRogue;
using UnityEngine;

namespace World.Pawns.AI.Goals
{
    public class Build : JobGoal
    {
        private GridObject _blueprint;
        
        private Coord _workSpot;
        
        public Build()
        {
        }

        public override void TakeAction()
        {
            _inProgress = true;
            
            _blueprint = ((LocalMap) Pawn.CurrentMap).GetBlueprintAt(Job.Location);
            
            //todo if there are resources available, haul them to the location -- add haul child goal
        
            //todo if no resources available, fail to parent -- this should suspend that job until more resources are available
            // basically job subscribes to a stockpile event for that particular resource
        
            //todo if all resources are added, construct at some rate based on construction skill until work left is zero
            //placed object can handle construction. Pawn stops at work site, placed object runs construction loop until work left is zero or pawn moves away from work site

            if (Pawn.Position != _workSpot)
            {
                //todo this works for one tile jobs, but for bigger blueprints we need to find adjacent tiles around whole object
                // maybe loop through all grid objects and return walkable adjacent coords that are not part of the same placed object
                //changing job location to list to make this easier
                
                
                var walkablePositions = ((LocalMap) Pawn.CurrentMap).GetAdjacentWalkableLocations(Job.Location);

                _workSpot = Coord.NONE;

                foreach (var position in walkablePositions)
                {
                    if (Pawn.Movement.CanPathTo(position))
                    {
                        _workSpot = position;
                        break;
                    }
                }

                if (_workSpot == Coord.NONE)
                {
                    //todo suspend job until a walkable neighbor is available
                    
                    Debug.Log($"{Pawn.id} cannot path to {Job.Location}");
                    
                    Failed();
                    
                    return;
                }
                
                var localMove = new LocalMove(Pawn.Movement, _workSpot);
                
                PushChildGoal(localMove);

                _inProgress = false;

                return;
            }
            
            Pawn.FaceToward(Job.Location);

            _blueprint.PlacedObject.Construct(Job, Pawn,16); //todo testing 
        }

        public override void Failed()
        {
            Job.UnAssignPawn();
            
            Pop();
        }

        public override bool Finished()
        {
            if (_blueprint == null)
            {
                return false;
            }
            
            return !_blueprint.PlacedObject.NeedsToBeMade;
        }
    }
}
