using System.Collections.Generic;
using System.Linq;
using Repos;
using World.Pawns.Skills;
using Object = UnityEngine.Object;

namespace World.Pawns.Jobs
{
    /// <summary>
    /// Keeps track of available <see cref="Job"/>s and assigns them to idle <see cref="Pawn"/>s.
    /// </summary>
    public class JobGiver
    {
        /// <summary>
        /// Available <see cref="Job"/>s organized by <see cref="Skill"/> and ordered by default
        /// Skill priority. 
        /// </summary>
        private Dictionary<Skill, List<Job>> _availableJobs;

        /// <summary>
        /// Constructor
        /// </summary>
        public JobGiver()
        {
            InitializeAvailableJobs();
        }

        /// <summary>
        /// Iterates through available <see cref="Job"/>s and assigns them to
        /// <see cref="Pawn"/>s when possible.
        /// </summary>
        public void Tick()
        {
            foreach (var skill in _availableJobs.Keys.ToArray())
            {
                foreach (var job in _availableJobs[skill].ToArray())
                {
                    if (job.IsAssigned)
                    {
                        RemoveJob(job);
                    }
            
                    job.SkillNeeded.onSkillNeeded?.Invoke(job);
                }
            }
        }

        /// <summary>
        /// Registers the pawn
        /// </summary>
        /// <param name="pawn">The pawn to be registered</param>
        public void RegisterPawn(Pawn pawn)
        {
            pawn.onJobTaken += OnJobTaken;
        }

        /// <summary>
        /// Registers the map
        /// </summary>
        /// <param name="map">The map to be registered</param>
        public void RegisterMap(LocalMap map)
        {
            map.onJobAdded += AddJob;
        }
        
        /// <summary>
        /// Adds <see cref="Job"/> to available jobs.
        /// </summary>
        /// <param name="job">The job to be added</param>
        public void AddJob(Job job)
        {
            if (JobAlreadyAdded(job))
            {
                return;
            }

            if (job.IsAssigned)
            {
                job.UnAssignPawn();
            }

            _availableJobs[job.SkillNeeded].Add(job);
        }
        
        /// <summary>
        /// Removes the <see cref="Job"/> from available jobs.
        /// </summary>
        /// <param name="job">The job to remove</param>
        public void RemoveJob(Job job)
        {
            _availableJobs[job.SkillNeeded].Remove(job);
        }

        /// <summary>
        /// Removes <see cref="Job"/> from available jobs.
        /// </summary>
        /// <param name="job">The job</param>
        private void OnJobTaken(Job job)
        {
            job.onPawnUnassigned += OnJobUnassigned;
            
            RemoveJob(job);
        }

        /// <summary>
        /// Adds <see cref="Job"/> back to available jobs
        /// </summary>
        /// <param name="job">The job</param>
        public void OnJobUnassigned(Job job)
        {
            job.onPawnUnassigned -= OnJobUnassigned;

            AddJob(job);
        }

        /// <summary>
        /// Initializes the available <see cref="Job"/>s grouped by <see cref="Skill"/> and ordered by default
        /// skill priority
        /// </summary>
        private void InitializeAvailableJobs()
        {
            _availableJobs = new Dictionary<Skill, List<Job>>();
            
            var pawnRepo = Object.FindObjectOfType<PawnRepo>();

            var skills = pawnRepo.GetSkills().OrderBy(s => s.defaultPriority);

            foreach (var skill in skills)
            {
                _availableJobs.Add(skill, new List<Job>());
            }
        }

        /// <summary>
        /// Checks if <see cref="Job"/> is already added to available jobs
        /// </summary>
        /// <param name="job">The job to check for existence</param>
        /// <returns>True if job is already added</returns>
        private bool JobAlreadyAdded(Job job)
        {
            return _availableJobs.Keys.ToArray().Any(skill => _availableJobs[skill].Contains(job));
        }
    }
}
