using System.Collections.Generic;
using System.Linq;
using Repos;
using World.Pawns.Jobs;
using Object = UnityEngine.Object;

namespace World.Pawns.Skills
{
    /// <summary>
    /// The skill set class
    /// </summary>
    public class SkillSet
    {
        /// <summary>
        /// The pawn
        /// </summary>
        private Pawn _pawn;

        /// <summary>
        /// The skills
        /// </summary>
        private Dictionary<Skill, int> _skills;
        
        /// <summary>
        /// The enabled skills
        /// </summary>
        private List<Skill> _enabledSkills;

        /// <summary>
        /// Initializes a new instance of the <see cref="SkillSet"/> class
        /// </summary>
        /// <param name="pawn">The pawn</param>
        public SkillSet(Pawn pawn)
        {
            _pawn = pawn;
            
            InitializeSkills();
        }

        /// <summary>
        /// Generates the base skills
        /// </summary>
        public void GenerateBaseSkills()
        {
            //todo
        }

        /// <summary>
        /// Enables the skill using the specified skill
        /// </summary>
        /// <param name="skill">The skill</param>
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
        
        /// <summary>
        /// Disables the skill using the specified skill
        /// </summary>
        /// <param name="skill">The skill</param>
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

        /// <summary>
        /// Initializes the skills
        /// </summary>
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

        /// <summary>
        /// Ons the skill needed using the specified job
        /// </summary>
        /// <param name="job">The job</param>
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
