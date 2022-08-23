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
    /// <summary>
    /// The thing template class
    /// </summary>
    /// <seealso cref="Template"/>
    [CreateAssetMenu(menuName = "Templates/Thing Template")]
    public class ThingTemplate : Template
    {
        /// <summary>
        /// The parent
        /// </summary>
        [SerializeField] private ThingTemplate parent;
        
        /// <summary>
        /// The thing
        /// </summary>
        [SerializeField] private Type thingClass;

        /// <summary>
        /// The category
        /// </summary>
        [SerializeField] private ThingCategoryTemplate _category = null;

        /// <summary>
        /// The undefined
        /// </summary>
        [SerializeField] private MapLayer _layer = MapLayer.Undefined; 

        /// <summary>
        /// The ticker type
        /// </summary>
        [SerializeField] private TickerType _tickerType = null;

        /// <summary>
        /// The stack limit
        /// </summary>
        [SerializeField] private int _stackLimit = -1;
        
        //stats start. May need a separate class for this.
        
        /// <summary>
        /// The max hit points
        /// </summary>
        [SerializeField] private int _maxHitPoints = -1;
        
        /// <summary>
        /// The flammability
        /// </summary>
        [SerializeField] private float _flammability = -1.0f;
        
        /// <summary>
        /// The min value
        /// </summary>
        [SerializeField] private int _beauty = int.MinValue;
        
        /// <summary>
        /// The mass
        /// </summary>
        [SerializeField] private int _mass = -1;

        /// <summary>
        /// The parts
        /// </summary>
        [SerializeField] private List<PartTemplate> _parts;
        
        //stats end 

        /// <summary>
        /// The prefab
        /// </summary>
        [SerializeField] private Transform prefab;
        
        /// <summary>
        /// Gets the value of the prefab
        /// </summary>
        public Transform Prefab => prefab == null ? parent.Prefab : prefab;

        /// <summary>
        /// The sprite sheet
        /// </summary>
        public Sprite spriteSheet;

        /// <summary>
        /// The destroyable
        /// </summary>
        public bool destroyable = true;

        /// <summary>
        /// The rotatable
        /// </summary>
        public bool rotatable = true;

        /// <summary>
        /// The use hit points
        /// </summary>
        public bool useHitPoints = true;
        
        /// <summary>
        /// The selectable
        /// </summary>
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

        /// <summary>
        /// The is static
        /// </summary>
        public bool isStatic;

        //todo components and component properties

        /// <summary>
        /// Gets the value of the thing class
        /// </summary>
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
        
        /// <summary>
        /// Gets the value of the category
        /// </summary>
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
        
        /// <summary>
        /// Gets the value of the map layer
        /// </summary>
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
        
        /// <summary>
        /// Gets the value of the ticker type
        /// </summary>
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
        
        /// <summary>
        /// Gets the value of the stack limit
        /// </summary>
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
        
        /// <summary>
        /// Gets the value of the max hit points
        /// </summary>
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
        
        /// <summary>
        /// Gets the value of the flammability
        /// </summary>
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
        
        /// <summary>
        /// Gets the value of the beauty
        /// </summary>
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
        
        /// <summary>
        /// Gets the value of the mass
        /// </summary>
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
        
        /// <summary>
        /// Gets the value of the parts
        /// </summary>
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

        /// <summary>
        /// Makes the thing
        /// </summary>
        /// <returns>The thing</returns>
        public Thing MakeThing()
        {
            //todo make with different materials
            
            var thing = (Thing) Activator.CreateInstance(ThingClass);
            
            thing.template = this;

            return thing;
        }
    }
}
