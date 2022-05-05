using System;
using Assets.Scripts.World.Things.ThingCategories;
using TickerTypes;
using UnityEngine;

namespace Assets.Scripts.World.Things
{
    [CreateAssetMenu(menuName = "Templates/ThingTemplate")]
    public class ThingTemplate : Template
    {
        public Type thingType;

        public ThingCategoryTemplate category;

        public TickerType tickerType;

        public int stackLimit = 1;

        public bool destroyable = true;

        public bool rotatable = true;

        public bool useHitPoints = true;
    }
}
