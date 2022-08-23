using World.Pawns.Jobs;

namespace World.Pawns.AI.Goals
{
    /// <summary>
    /// The job goal class
    /// </summary>
    /// <seealso cref="Goal"/>
    public class JobGoal : Goal
    {
        /// <summary>
        /// Gets or sets the value of the job
        /// </summary>
        public Job Job { get; set; }
    }
}
