using Controllers;
using GoRogue;
using Graphics;
using UnityEngine;
using Utilities;

namespace World.Things
{
    public class StackThing : Thing
    {
        private int _count;
        public int Count
        {
            get => _count;

            set
            {
                //todo not sure how we can handle remainders here
                //so whoever is placing items on stack needs to handle that
                
                if (_count + value > StackLimit)
                {
                    _count = StackLimit;
                }
                else
                {
                    _count += value;
                }
            }
        }

        public int StackLimit => ((StackThingTemplate)template).StackLimit;

        public StackThing(ThingTemplate thingTemplate, Coord position, int count) : base(position.ToVector3(),
            MapLayer.GridObject, thingTemplate.isStatic, thingTemplate.walkable, thingTemplate.transparent)
        {
            template = thingTemplate;

            graphicTemplate = thingTemplate.graphics;

            Position = position;
            
            _count = count;
            
            UpdateGraphics();
            
            Object.FindObjectOfType<StackCountLabelController>().AddLabel(this);
        }
        
        protected sealed override void UpdateGraphics()
        {
            MainGraphic = GraphicInstance.GetNew(template.graphics);
        }
    }
}
