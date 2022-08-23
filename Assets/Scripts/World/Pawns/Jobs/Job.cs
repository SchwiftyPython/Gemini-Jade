using System;
using GoRogue;
using World.Pawns.Skills;

namespace World.Pawns.Jobs
{
    /// <summary>
    /// The job class
    /// </summary>
    public class Job
    {
        /// <summary>
        /// Gets the value of the location
        /// </summary>
        public Coord Location { get; } //todo this could be a list

        /// <summary>
        /// The assigned pawn
        /// </summary>
        private Pawn _assignedPawn;

        /// <summary>
        /// The on pawn unassigned
        /// </summary>
        public Action<Job> onPawnUnassigned;

        /// <summary>
        /// Gets the value of the skill needed
        /// </summary>
        public Skill SkillNeeded { get; }

        /// <summary>
        /// Gets the value of the skill level needed
        /// </summary>
        public int SkillLevelNeeded { get; }
        
        /// <summary>
        /// Gets the value of the is assigned
        /// </summary>
        public bool IsAssigned => _assignedPawn != null;
    
        //todo resources needed

        /// <summary>
        /// Initializes a new instance of the <see cref="Job"/> class
        /// </summary>
        /// <param name="location">The location</param>
        /// <param name="skillNeeded">The skill needed</param>
        /// <param name="skillLevelNeeded">The skill level needed</param>
        public Job(Coord location, Skill skillNeeded, int skillLevelNeeded)
        {
            Location = location;
            
            SkillNeeded = skillNeeded;
            
            SkillLevelNeeded = skillLevelNeeded;
        }
        
        /// <summary>
        /// Assigns the pawn using the specified pawn
        /// </summary>
        /// <param name="pawn">The pawn</param>
        public void AssignPawn(Pawn pawn)
        {
            _assignedPawn = pawn;
        }
        
        /// <summary>
        /// Uns the assign pawn
        /// </summary>
        public void UnAssignPawn()
        {
            _assignedPawn = null;
            
            onPawnUnassigned?.Invoke(this);
        }
    }
}
