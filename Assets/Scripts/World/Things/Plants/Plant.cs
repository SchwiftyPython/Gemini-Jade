using System;
using GoRogue;
using Graphics;
using Settings;
using Time;
using UnityEngine;
using Utilities;
using World.Pawns;
using World.Pawns.Jobs;
using World.Pawns.Skills;
using World.Things.Food;
using Object = UnityEngine.Object;

namespace World.Things.Plants
{
    public class Plant : Thing
    {
        private JobProgressTracker _progressTracker;
        
        private float _ageTicks;

        private float _lifespanTicks;

        private float _ticksPerState;

        public Action onJobFinished;

        public bool ReadyToHarvest => CanBeHarvested && _ageTicks >= ((PlantTemplate) template).daysToMaturity * Constants.TicksPerDay;

        public bool CanBeHarvested => ((PlantTemplate) template).canHarvest;

        public int MinSkillToHarvest => ((PlantTemplate) template).minSkillLevel;

        public Skill SkillNeeded => ((PlantTemplate) template).skill;

        public PlantTemplate PlantTemplate => (PlantTemplate) template;
        
        //todo growth states

        public Plant(PlantTemplate plantTemplate, Coord position) : base(position.ToVector3(), MapLayer.Plant, plantTemplate.isStatic, plantTemplate.walkable, plantTemplate.transparent)
        {
            template = plantTemplate;
            
            _ageTicks = 0;

            _lifespanTicks = plantTemplate.daysToMaturity * plantTemplate.lifeSpanMultiplier * Constants.TicksPerDay;

            //todo _ticksPerState = _lifespanTicks / template.numGrowthStates;
            
            Position = position;

            graphicTemplate = plantTemplate.graphics;

            UpdateGraphics();
            
            var tickController = Object.FindObjectOfType<TickController>();
            
            tickController.RegisterTicksFor(this);
        }

        public override void Tick()
        {
            _ageTicks++;
            
            CalculateGrowthState();

            if (_ageTicks > _lifespanTicks)
            {
                //todo dead plant state
                //can only cut
            }
        }

        public void Cut()
        {
            //todo
            //if plant is ready to harvest, drops some amount of loot, but never max.
            
        }

        public void Harvest(Job job, Pawn worker)
        {
            if (_progressTracker == null)
            {
                var go = new GameObject($"{template.label} {id} Harvest Progress Tracker");

                _progressTracker = go.AddComponent<JobProgressTracker>();
            }
            
            //todo get worker's harvest skill
            
            _progressTracker.WorkOn(job, worker, 16, PlantTemplate.workToHarvest);

            _progressTracker.onJobComplete += FinishHarvest;
        }

        protected sealed override void UpdateGraphics()
        {
            //todo plant colors
            
            MainGraphic = GraphicInstance.GetNew(template.graphics);
        }

        private void CalculateGrowthState()
        {
            //todo
        }

        private void FinishHarvest()
        {
            onJobFinished?.Invoke();

            DropLoot();
            
            CurrentLocalMap.RemoveBaseObject(this);
        }

        private void DropLoot()
        {
            Thing loot;
            
            if (PlantTemplate.numThingHarvested > 1)
            {
                //todo vary number of thing harvested depending on current growth state, pawn harvest skill, and if cutting or harvesting
                //probably call it float harvestModifier
                
                Debug.Log($"Harvested {PlantTemplate.numThingHarvested} {PlantTemplate.thingHarvested.label} from {PlantTemplate.label} at {Position}");

                if (PlantTemplate.thingHarvested is FoodTemplate foodTemplate)
                {
                    loot = new Food.Food(foodTemplate, Position, PlantTemplate.numThingHarvested);
                }
                else
                {
                    loot = new StackThing(PlantTemplate.thingHarvested, Position, PlantTemplate.numThingHarvested);
                }
            }
            else
            {
                loot = new Thing(PlantTemplate.thingHarvested);
            }
            
            CurrentLocalMap.AddBaseObject(loot, Position, true);
        }
    }
}
