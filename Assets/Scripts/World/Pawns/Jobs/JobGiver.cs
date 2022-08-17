using System.Collections.Generic;

namespace World.Pawns.Jobs
{
    public class JobGiver
    {
        private List<Job> _availableJobs;
    
        public JobGiver()
        {
            _availableJobs = new List<Job>();
        }

        public void Tick()
        {
            foreach (var job in _availableJobs.ToArray())
            {
                //todo assign pawn to job and have pawn check for an assigned pawn when taking job
                //if this doesn't work then we'll have pawn's ask job giver for something
            
                job.SkillNeeded.onSkillNeeded?.Invoke(job);
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
            if (_availableJobs.Contains(job))
            {
                return;
            }
            
            _availableJobs.Add(job);
        }
        
        public void RemoveJob(Job job)
        {
            _availableJobs.Remove(job);
        }
        
        private void OnJobTaken(Job job)
        {
            RemoveJob(job);
        }

        public void OnJobCancelled(Job job)
        {
            AddJob(job);
        }
    }
}
