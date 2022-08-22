using System;
using System.Collections.Generic;
using Assets.Scripts.World.Pawns;
using Assets.Scripts.World.Pawns.Species;
using GoRogue;
using Time;
using UnityEngine;
using World.Pawns.AI.Brains;
using World.Pawns.AI.Goals;
using World.Pawns.Health;
using World.Pawns.Jobs;
using World.Pawns.Skills;
using World.Things;
using Object = UnityEngine.Object;

namespace World.Pawns
{
    public class Pawn : Thing
    {
        private SkillSet _skillSet;
        
        private PawnMovement _movement;
        
        public PawnMovement Movement
        {
            get
            {
                if (_movement == null)
                {
                    _movement = SpriteInstance.GetComponent<PawnMovement>();
                }

                return _movement;
            }
            private set => _movement = value;
        }

        public SpeciesTemplate species;

        private string _name;

        private Brain _brain;

        public Health.Health health;

        public bool Dead => health.Dead;

        public bool CanWakeUp => health.CanWakeUp;

        public bool IsOrganic => species.fleshType != FleshType.Machine;

        public bool IsHumanoid => species.intellect >= Intellect.Humanoid;

        public bool ToolUser => species.intellect >= Intellect.ToolUser;

        public bool IsAnimal => !ToolUser && IsOrganic;

        public Action<Job> onJobTaken;

        public Action onPawnMoved;

        public Pawn(SpeciesTemplate speciesTemplate) : base(speciesTemplate)
        {
            species = speciesTemplate;

            _name = "Testy McTestes";

            health = new Health.Health(this);
            
            _skillSet = new SkillSet(this);

            _brain = new Brain(this);
            
            GenerateId();
        }

        public List<BodyPart> GetBody()
        {
            return health.GetBody();
        }

        public void Die()
        {
            //todo a ton of shit happens here. We'll revisit often.
            
            health.KillPawn();
        }

        public bool IsImmobile()
        {
            return health.Downed; //todo or sleeping, etc
        }

        public override void Tick()
        {
            base.Tick();

            var tickController = Object.FindObjectOfType<TickController>();

            //not sure if tick intervals are the same everywhere. May have to define per class using it.
            if (tickController.NumTicks % TickController.RareTickInterval == 0) 
            {
                TickRare();
                
                //testing brain
            
                _brain.Think();
            }
            
            //todo check if not suspended
            
            //todo bunch of trackers and whatnot ticking

            health.Tick();

            if (!Dead)
            {
                //todo mind state tick
                
                //todo carry tracker tick
            }
            
            //todo bunch of trackers and whatnot ticking
        }

        public override void TickRare()
        {
            base.TickRare();
            
            //todo 
        }

        public void MoveToLocal(Coord position)
        {
            if (!spawned)
            {
                Debug.LogError($"Tried to move {this} to {position} but it's not spawned. ID: {id}");
                
                return;
            }
            
            if (SpriteInstance == null)
            {
                Debug.LogError($"Tried to move {this} to {position} but it has no sprite. ID: {id}");
                
                return;
            }

            if (Movement == null)
            {
                Debug.LogError($"No PawnMovement component found on {SpriteInstance}. ID: {id}");
                
                return;
            }
            
            onPawnMoved?.Invoke();

            var moveGoal = new LocalMove(Movement, position);
            
            _brain.AddPriorityGoal(moveGoal);
            
            //Movement.MoveTo(position);
        }

        public void FaceToward(Coord position)
        {
            Movement.UpdateFacing(position);
        }

        public void AddJob(Job job)
        {
            if (job.IsAssigned)
            {
                Debug.Log($"{job} is already assigned.");
                
                return;
            }
            
            job.AssignPawn(this);
            
            var goal = Activator.CreateInstance(job.SkillNeeded.goalClass) as JobGoal;
            
            if (goal == null)
            {
                return;
            }
            
            goal.Job = job;

            _brain.AddPriorityGoal(goal);
            
            onJobTaken?.Invoke(job);
        }

        public bool HasJobAssigned()
        {
            return _brain.HasJobGoal();
        }

        public Job GetCurrentJob()
        {
            return _brain.GetCurrentJob();
        }

        public void CancelCurrentJob()
        {
            var job = _brain.GetCurrentJob();
            
            job.UnAssignPawn();
        }
    }
}
