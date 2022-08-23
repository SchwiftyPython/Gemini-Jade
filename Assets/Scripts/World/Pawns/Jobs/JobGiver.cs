using System.Collections.Generic;
using System.Linq;
using Repos;
using UnityEngine;
using World.Pawns.Skills;

namespace World.Pawns.Jobs
{
    /// <summary>
    /// The job giver class
    /// </summary>
    public class JobGiver
    {
        /// <summary>
        /// The available jobs
        /// </summary>
        private Dictionary<Skill, List<Job>> _availableJobs;

        //private List<Job> _availableJobs;
    
        /// <summary>
        /// Initializes a new instance of the <see cref="JobGiver"/> class
        /// </summary>
        public JobGiver()
        {
            InitializeAvailableJobs();
        }

        /// <summary>
        /// Ticks this instance
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
        /// Registers the pawn using the specified pawn
        /// </summary>
        /// <param name="pawn">The pawn</param>
        public void RegisterPawn(Pawn pawn)
        {
            pawn.onJobTaken += OnJobTaken;
        }

        /// <summary>
        /// Registers the map using the specified map
        /// </summary>
        /// <param name="map">The map</param>
        public void RegisterMap(LocalMap map)
        {
            map.onJobAdded += AddJob;
        }
        
        /// <summary>
        /// Adds the job using the specified job
        /// </summary>
        /// <param name="job">The job</param>
        public void AddJob(Job job)
        {
            if (JobAlreadyAdded(job))
            {
                Debug.Log($"Job already add to available jobs. Job Location: {job.Location}");
                
                return;
            }

            if (job.IsAssigned)
            {
                job.UnAssignPawn();
            }

            _availableJobs[job.SkillNeeded].Add(job);
        }
        
        /// <summary>
        /// Removes the job using the specified job
        /// </summary>
        /// <param name="job">The job</param>
        public void RemoveJob(Job job)
        {
            _availableJobs[job.SkillNeeded].Remove(job);
        }
        
        /// <summary>
        /// Ons the job taken using the specified job
        /// </summary>
        /// <param name="job">The job</param>
        private void OnJobTaken(Job job)
        {
            job.onPawnUnassigned += OnJobUnassigned;
            
            RemoveJob(job);
        }

        /// <summary>
        /// Ons the job unassigned using the specified job
        /// </summary>
        /// <param name="job">The job</param>
        public void OnJobUnassigned(Job job)
        {
            job.onPawnUnassigned -= OnJobUnassigned;
            
            Debug.Log($"Pawn unassigned, adding job to available jobs. Job Location: {job.Location}");
            
            AddJob(job);
        }

        /// <summary>
        /// Initializes the available jobs
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
        /// Describes whether this instance job already added
        /// </summary>
        /// <param name="job">The job</param>
        /// <returns>The bool</returns>
        private bool JobAlreadyAdded(Job job)
        {
            return _availableJobs.Keys.ToArray().Any(skill => _availableJobs[skill].Contains(job));
        }
    }
}
