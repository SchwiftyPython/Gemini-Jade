using System;
using Time.TickerTypes;
using UnityEngine;
using World.Things.CraftableThings;
using World.Things.ThingCategories;

namespace World.Things
{
    [CreateAssetMenu(menuName = "Templates/ThingTemplate")]
    public class ThingTemplate : CraftableTemplate
    {
        [SerializeField] private ThingTemplate _parent;
        
        [SerializeField] private Type _thingClass;

        [SerializeField] private ThingCategoryTemplate _category = null;

        [SerializeField] private Layer _layer = Layer.Undefined; 

        [SerializeField] private TickerType _tickerType = null;

        [SerializeField] private int _stackLimit = -1;
        
        //stats start here. May need a separate class for this.
        
        [SerializeField] private int _maxHitPoints = -1;
        
        [SerializeField] private float _flammability = -1.0f;
        
        [SerializeField] private int _beauty = int.MinValue;
        
        //stats end here.

        public bool destroyable = true;

        public bool rotatable = true;

        public bool useHitPoints = true;
        
        public bool selectable = true;

        //todo components and component properties

        public Type ThingClass
        {
            get
            {
                if (_thingClass != null)
                {
                    return _thingClass;
                }

                return _parent != null ? _parent.ThingClass : _thingClass;
            }
        }
        
        public ThingCategoryTemplate Category
        {
            get
            {
                if (_category != null)
                {
                    return _category;
                }
                
                return _parent != null ? _parent.Category : _category;
            }
        }
        
        public Layer Layer
        {
            get
            {
                if (_layer != Layer.Undefined)
                {
                    return _layer;
                }
                
                return _parent != null ? _parent.Layer : _layer;
            }
        }
        
        public TickerType TickerType
        {
            get
            {
                if (_tickerType != null)
                {
                    return _tickerType;
                }
                
                return _parent != null ? _parent.TickerType : _tickerType;
            }
        }
        
        public int StackLimit
        {
            get
            {
                if (_stackLimit != -1)
                {
                    return _stackLimit;
                }
                
                return _parent != null ? _parent.StackLimit : _stackLimit;
            }
        }
        
        public int MaxHitPoints
        {
            get
            {
                if (_maxHitPoints != -1)
                {
                    return _maxHitPoints;
                }
                
                return _parent != null ? _parent.MaxHitPoints : _maxHitPoints;
            }
        }
        
        public float Flammability
        {
            get
            {
                if (_flammability != -1.0f)
                {
                    return _flammability;
                }
                
                return _parent != null ? _parent.Flammability : _flammability;
            }
        }
        
        public int Beauty
        {
            get
            {
                if (_beauty != int.MinValue)
                {
                    return _beauty;
                }
                
                return _parent != null ? _parent.Beauty : _beauty;
            }
        }
        
        
        
        
    }
}
