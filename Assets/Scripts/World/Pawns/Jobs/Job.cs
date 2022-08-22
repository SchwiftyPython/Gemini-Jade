using System;
using GoRogue;
using World.Pawns.Skills;

namespace World.Pawns.Jobs
{
    public class Job
    {
        public Coord Location { get; } //todo this could be a list

        private Pawn _assignedPawn;

        public Action<Job> onPawnUnassigned;

        public Skill SkillNeeded { get; }

        public int SkillLevelNeeded { get; }
        
        public bool IsAssigned => _assignedPawn != null;
    
        //todo resources needed

        public Job(Coord location, Skill skillNeeded, int skillLevelNeeded)
        {
            Location = location;
            
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
            
            onPawnUnassigned?.Invoke(this);
        }
    }
}
