namespace World.Pawns.AI.Goals
{
    public class Build : JobGoal
    {
        public Build()
        {
        }

        public override void TakeAction()
        {
            //todo if there are resources available, haul them to the location -- add haul child goal
        
            //todo if no resources available, fail to parent -- this should suspend that job until more resources are available
            // basically job subscribes to a stockpile event for that particular resource
        
            //todo if all resources are added, construct at some rate based on construction skill until work left is zero
            //placed object ca handle construction. Pawn stops at work site, placed object runs construction loop until work left is zero or pawn moves away from work site
        
        }
    }
}
