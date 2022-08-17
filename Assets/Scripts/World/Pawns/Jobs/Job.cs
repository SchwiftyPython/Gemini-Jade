using GoRogue;
using World.Pawns.Skills;

namespace World.Pawns.Jobs
{
    public class Job
    {
        private Coord _location;

        private Pawn _assignedPawn;

        public Skill SkillNeeded { get; }

        public int SkillLevelNeeded { get; }
        
        public bool IsAssigned => _assignedPawn != null;
    
        //todo resources needed

        public Job(Coord location, Skill skillNeeded, int skillLevelNeeded)
        {
            _location = location;
            
            SkillNeeded = skillNeeded;
            
            SkillLevelNeeded = skillLevelNeeded;
        }
        
        public void AssignPawn(Pawn pawn)
        {
            _assignedPawn = pawn;
        }
        
        public void UnAssignPawn()
        {
            _assignedPawn = null;
        }
    }
}
