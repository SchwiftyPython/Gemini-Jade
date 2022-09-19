using GoRogue;
using Settings;
using Time;
using UnityEngine;
using Utilities;

namespace World.Things.Plants
{
    public class Plant : Thing
    {
        private float _ageTicks;

        private float _lifespanTicks;

        private float _ticksPerState;
        
        //todo growth states

        public Plant(PlantTemplate template, Coord position) : base(template)
        {
            _ageTicks = 0;

            _lifespanTicks = template.daysToMaturity * template.lifeSpanMultiplier * Constants.TicksPerDay;

            //todo _ticksPerState = _lifespanTicks / template.numGrowthStates;
            
            Position = position;
            
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
        }

        public void Harvest()
        {
            //todo
        }

        protected sealed override void UpdateGraphics()
        {
            //todo
        }

        private void CalculateGrowthState()
        {
            //todo
        }
    }
}
