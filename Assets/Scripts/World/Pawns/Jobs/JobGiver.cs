using System.Collections.Generic;
using System.Linq;
using Repos;
using UnityEngine;
using World.Pawns.Skills;

namespace World.Pawns.Jobs
{
    public class JobGiver
    {
        private Dictionary<Skill, List<Job>> _availableJobs;

        //private List<Job> _availableJobs;
    
        public JobGiver()
        {
            InitializeAvailableJobs();
        }

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

        public void RegisterPawn(Pawn pawn)
        {
            pawn.onJobTaken += OnJobTaken;
        }

        public void RegisterMap(LocalMap map)
        {
            map.onJobAdded += AddJob;
        }
        
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
        
        public void RemoveJob(Job job)
        {
            _availableJobs[job.SkillNeeded].Remove(job);
        }
        
        private void OnJobTaken(Job job)
        {
            job.onPawnUnassigned += OnJobUnassigned;
            
            RemoveJob(job);
        }

        public void OnJobUnassigned(Job job)
        {
            job.onPawnUnassigned -= OnJobUnassigned;
            
            Debug.Log($"Pawn unassigned, adding job to available jobs. Job Location: {job.Location}");
            
            AddJob(job);
        }

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

        private bool JobAlreadyAdded(Job job)
        {
            return _availableJobs.Keys.ToArray().Any(skill => _availableJobs[skill].Contains(job));
        }
    }
}
