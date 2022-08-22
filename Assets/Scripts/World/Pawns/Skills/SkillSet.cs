using System.Collections.Generic;
using System.Linq;
using Repos;
using World.Pawns.Jobs;
using Object = UnityEngine.Object;

namespace World.Pawns.Skills
{
    public class SkillSet
    {
        private Pawn _pawn;

        private Dictionary<Skill, int> _skills;
        
        private List<Skill> _enabledSkills;

        public SkillSet(Pawn pawn)
        {
            _pawn = pawn;
            
            InitializeSkills();
        }

        public void GenerateBaseSkills()
        {
            //todo
        }

        public void EnableSkill(Skill skill)
        {
            if (_enabledSkills == null)
            {
                _enabledSkills = new List<Skill>();
            }
            
            _enabledSkills.Add(skill);
            
            //todo sub and unsub dependent on skill enabled and no current job
            
            //probably sub in Bored Goal and unsub when bored goal is finished or adding another goal
            
            skill.onSkillNeeded += OnSkillNeeded;
        }
        
        public void DisableSkill(Skill skill)
        {
            if (_enabledSkills == null)
            {
                return;
            }

            if (!_enabledSkills.Contains(skill))
            {
                return;
            }

            _enabledSkills.Remove(skill);
            
            skill.onSkillNeeded -= OnSkillNeeded;
        }

        private void InitializeSkills()
        {
            _skills = new Dictionary<Skill, int>();

            var pawnRepo = Object.FindObjectOfType<PawnRepo>();
            
            var allSkills = pawnRepo.GetSkills().OrderBy(s => s.defaultPriority);

            foreach (var skill in allSkills)
            {
                _skills.Add(skill, 0);
                
                //todo only if initially enabled
                //maybe initially enable skills that
                //meet a minimum or are part of a background
                
                EnableSkill(skill);
            }
        }

        private void OnSkillNeeded(Job job)
        {
            if (!_enabledSkills.Contains(job.SkillNeeded))
            {
                job.SkillNeeded.onSkillNeeded -= OnSkillNeeded;
                
                return;
            }
            
            var skillLevel = _skills[job.SkillNeeded];
            
            if(job.SkillLevelNeeded > skillLevel)
            {
                return;
            }

            if (_pawn.HasJobAssigned())
            {
                var currentJob = _pawn.GetCurrentJob();

                var currentJobPriority = _skills.Keys.ToList().IndexOf(currentJob.SkillNeeded);
                
                var newJobPriority = _skills.Keys.ToList().IndexOf(job.SkillNeeded);

                if (currentJobPriority <= newJobPriority)
                {
                    return;
                }
            }

            _pawn.AddJob(job);
        }
    }
}
