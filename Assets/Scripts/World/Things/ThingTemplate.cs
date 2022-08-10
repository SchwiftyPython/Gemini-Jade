using System;
using System.Collections.Generic;
using Assets.Scripts.World;
using GoRogue.GameFramework;
using Time.TickerTypes;
using UnityEngine;
using World.Things.Parts;
using World.Things.ThingCategories;

namespace World.Things
{
    [CreateAssetMenu(menuName = "Templates/Thing Template")]
    public class ThingTemplate : Template
    {
        [SerializeField] private ThingTemplate parent;
        
        [SerializeField] private Type thingClass;

        [SerializeField] private ThingCategoryTemplate _category = null;

        [SerializeField] private MapLayer _layer = MapLayer.Undefined; 

        [SerializeField] private TickerType _tickerType = null;

        [SerializeField] private int _stackLimit = -1;
        
        //stats start. May need a separate class for this.
        
        [SerializeField] private int _maxHitPoints = -1;
        
        [SerializeField] private float _flammability = -1.0f;
        
        [SerializeField] private int _beauty = int.MinValue;
        
        [SerializeField] private int _mass = -1;

        [SerializeField] private List<PartTemplate> _parts;
        
        //stats end 

        [SerializeField] private Transform prefab;
        
        public Transform Prefab => prefab == null ? parent.Prefab : prefab;

        public Sprite spriteSheet;

        public bool destroyable = true;

        public bool rotatable = true;

        public bool useHitPoints = true;
        
        public bool selectable = true;
        
        /// <summary>
        /// Whether or not the object is considered "transparent", eg. whether or not light passes through it.
        /// </summary>
        public bool transparent;

        /// <summary>
        /// Whether or not the object is to be considered "walkable", eg. whether or not the square it resides
        /// on can be traversed by other, non-walkable objects on the same <see cref="Map"/>.  Effectively, whether or not this
        /// object collides.
        /// </summary>
        public bool walkable;

        public bool isStatic;

        //todo components and component properties

        public Type ThingClass
        {
            get
            {
                if (thingClass != null)
                {
                    return thingClass;
                }

                return parent != null ? parent.ThingClass : thingClass;
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
                
                return parent != null ? parent.Category : _category;
            }
        }
        
        public MapLayer MapLayer
        {
            get
            {
                if (_layer != MapLayer.Undefined)
                {
                    return _layer;
                }
                
                return parent != null ? parent.MapLayer : _layer;
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
                
                return parent != null ? parent.TickerType : _tickerType;
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
                
                return parent != null ? parent.StackLimit : _stackLimit;
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
                
                return parent != null ? parent.MaxHitPoints : _maxHitPoints;
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
                
                return parent != null ? parent.Flammability : _flammability;
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
                
                return parent != null ? parent.Beauty : _beauty;
            }
        }
        
        public int Mass
        {
            get
            {
                if (_mass != -1)
                {
                    return _mass;
                }
                
                return parent != null ? parent.Mass : _mass;
            }
        }
        
        public List<PartTemplate> Parts
        {
            get
            {
                if (_parts != null)
                {
                    return _parts;
                }
                
                return parent != null ? parent.Parts : _parts;
            }
        }

        public Thing MakeThing()
        {
            //todo make with different materials
            
            var thing = (Thing) Activator.CreateInstance(ThingClass);
            
            thing.template = this;

            return thing;
        }
    }
}
