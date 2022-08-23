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
    /// <summary>
    /// Represents any living <see cref="Thing"/> in the world.
    /// </summary>
    public class Pawn : Thing
    {
        /// <summary>
        /// SkillSet component
        /// </summary>
        private SkillSet _skillSet;
        
        /// <summary>
        /// Movement component
        /// </summary>
        private PawnMovement _movement;
        
        /// <summary>
        /// Gets Movement component
        /// </summary>
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
        }

        /// <summary>
        /// <see cref="SpeciesTemplate"/> component
        /// </summary>
        public readonly SpeciesTemplate species;

        /// <summary>
        /// <see cref="Pawn"/>'s name
        /// </summary>
        /// <remarks> Can be empty. </remarks>
        private string _name;

        /// <summary>
        /// <see cref="Brain"/> component
        /// </summary>
        private readonly Brain _brain;

        /// <summary>
        /// <see cref="Health"/> component
        /// </summary>
        public readonly Health.Health health;

        /// <summary>
        /// Pawn dead status
        /// </summary>
        /// <returns>True if Pawn is dead</returns>
        public bool Dead => health.Dead;

        /// <summary>
        /// Pawn ability to wakeup
        /// </summary>
        /// <returns>True if can wake up</returns>
        public bool CanWakeUp => health.CanWakeUp;

        /// <summary>
        /// Gets the value of the is organic
        /// </summary>
        public bool IsOrganic => species.fleshType != FleshType.Machine;

        /// <summary>
        /// Gets the value of the is humanoid
        /// </summary>
        public bool IsHumanoid => species.intellect >= Intellect.Humanoid;

        /// <summary>
        /// Gets the value of the tool user
        /// </summary>
        public bool ToolUser => species.intellect >= Intellect.ToolUser;

        /// <summary>
        /// Gets the value of the is animal
        /// </summary>
        public bool IsAnimal => !ToolUser && IsOrganic;

        /// <summary>
        /// The on job taken
        /// </summary>
        public Action<Job> onJobTaken;

        /// <summary>
        /// The on pawn moved
        /// </summary>
        public Action onPawnMoved;

        /// <summary>
        /// Initializes a new instance of the <see cref="Pawn"/> class
        /// </summary>
        /// <param name="speciesTemplate">The species template</param>
        public Pawn(SpeciesTemplate speciesTemplate) : base(speciesTemplate)
        {
            species = speciesTemplate;

            _name = "Testy McTestes";

            health = new Health.Health(this);
            
            _skillSet = new SkillSet(this);

            _brain = new Brain(this);
            
            GenerateId();
        }

        /// <summary>
        /// Gets the body
        /// </summary>
        /// <returns>A list of body part</returns>
        public List<BodyPart> GetBody()
        {
            return health.GetBody();
        }

        /// <summary>
        /// Dies this instance
        /// </summary>
        public void Die()
        {
            //todo a ton of shit happens here. We'll revisit often.
            
            health.KillPawn();
        }

        /// <summary>
        /// Describes whether this instance is immobile
        /// </summary>
        /// <returns>The bool</returns>
        public bool IsImmobile()
        {
            return health.Downed; //todo or sleeping, etc
        }

        /// <summary>
        /// Ticks this instance
        /// </summary>
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

        /// <summary>
        /// Ticks the rare
        /// </summary>
        public override void TickRare()
        {
            base.TickRare();
            
            //todo 
        }

        /// <summary>
        /// Moves the to local using the specified position
        /// </summary>
        /// <param name="position">The position</param>
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

        /// <summary>
        /// Faces the toward using the specified position
        /// </summary>
        /// <param name="position">The position</param>
        public void FaceToward(Coord position)
        {
            Movement.UpdateFacing(position);
        }

        /// <summary>
        /// Adds the job using the specified job
        /// </summary>
        /// <param name="job">The job</param>
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

        /// <summary>
        /// Describes whether this instance has job assigned
        /// </summary>
        /// <returns>The bool</returns>
        public bool HasJobAssigned()
        {
            return _brain.HasJobGoal();
        }

        /// <summary>
        /// Gets the current job
        /// </summary>
        /// <returns>The job</returns>
        public Job GetCurrentJob()
        {
            return _brain.GetCurrentJob();
        }

        /// <summary>
        /// Cancels the current job
        /// </summary>
        public void CancelCurrentJob()
        {
            var job = _brain.GetCurrentJob();
            
            job.UnAssignPawn();
        }
    }
}
