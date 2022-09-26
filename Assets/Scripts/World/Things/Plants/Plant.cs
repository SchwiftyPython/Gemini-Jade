using GoRogue;
using Graphics;
using Settings;
using Time;
using UnityEngine;
using Utilities;
using World.Pawns;
using World.Pawns.Jobs;

namespace World.Things.Plants
{
    public class Plant : Thing
    {
        private JobProgressTracker _progressTracker;
        
        private float _ageTicks;

        private float _lifespanTicks;

        private float _ticksPerState;
        
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
                var go = new GameObject($"{id} Harvest Progress Tracker");

                _progressTracker = go.AddComponent<JobProgressTracker>();
            }
            
            //todo get worker's harvest skill
            
            _progressTracker.WorkOn(job, worker, 16);
            
            //todo sub to job completing to drop loot
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
    }
}
