namespace World.Pawns.AI.Goals
{
    public class Build : JobGoal
    {
        private GridObject _blueprint;
        
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

            if (Pawn.Position != Job.Location)
            {
                //todo we want to move to an adjacent location. If not available suspend job until a walkable neighbor is available
                
                var localMove = new LocalMove(Pawn.Movement, Job.Location);
                
                PushChildGoal(localMove);

                _inProgress = false;

                return;
            }
            
            //todo figure out construction speed

            _blueprint.PlacedObject.Construct(.2f);
        }

        public override void Failed()
        {
            _blueprint.PlacedObject.PauseConstruction();
            
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
