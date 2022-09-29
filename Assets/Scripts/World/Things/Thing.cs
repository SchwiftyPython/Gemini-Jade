using System.Collections.Generic;
using System.Linq;
using GoRogue;
using UnityEngine;
using World.Pawns.Health.DamageWorkers;

namespace World.Things
{
    /// <summary>
    /// The thing class
    /// </summary>
    /// <seealso cref="BaseObject"/>
    public class Thing : BaseObject
    {
        //todo total item counts -- adding, removing and updating counts of whatever is in stock pile
        
        //todo stockpile area
        
        /// <summary>
        /// The hit points
        /// </summary>
        private int _hitPoints;
        
        /// <summary>
        /// The directional sprites
        /// </summary>
        protected Dictionary<Direction, Sprite> directionalSprites;
        
        /// <summary>
        /// The sprite sheet
        /// </summary>
        protected Sprite spriteSheet; //todo only used by pawns atm. Pawns need to use graphics template as well.

        /// <summary>
        /// The template
        /// </summary>
        public ThingTemplate template;
        
        /// <summary>
        /// The destroyed
        /// </summary>
        public bool Destroyed;

        /// <summary>
        /// The id
        /// </summary>
        public int id = -1;
        
        /// <summary>
        /// The spawned
        /// </summary>
        public bool spawned;

        /// <summary>
        /// Initializes a new instance of the <see cref="Thing"/> class
        /// </summary>
        /// <param name="position">The position</param>
        /// <param name="layer">The layer</param>
        /// <param name="isStatic">The is static</param>
        /// <param name="isWalkable">The is walkable</param>
        /// <param name="isTransparent">The is transparent</param>
        protected Thing(Vector3 position, MapLayer layer, bool isStatic, bool isWalkable, bool isTransparent) : base(position,
            layer, isStatic, isWalkable, isTransparent)
        {
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="Thing"/> class
        /// </summary>
        /// <param name="layer">The layer</param>
        /// <param name="isStatic">The is static</param>
        /// <param name="isWalkable">The is walkable</param>
        /// <param name="isTransparent">The is transparent</param>
        protected Thing(MapLayer layer, bool isStatic, bool isWalkable, bool isTransparent) : base(layer, isStatic, isWalkable, isTransparent)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Thing"/> class
        /// </summary>
        /// <param name="template">The template</param>
        public Thing(ThingTemplate template) : base(template.MapLayer, template.isStatic, template.walkable, template.transparent)
        {
            this.template = template;
            
            spriteSheet = template.spriteSheet;

            graphicTemplate = template.graphics;
            
            PopulateSprites();
            
            //todo other template properties that need setting 
        }
       

        /// <summary>
        /// Initializes a new instance of the <see cref="Thing"/> class
        /// </summary>
        protected Thing()
        {
        }

        /// <summary>
        /// Gets or sets the value of the hit points
        /// </summary>
        public virtual int HitPoints
        {
            get
            {
                return _hitPoints;
            }
            set
            {
                _hitPoints = value;
            }
        }

        /// <summary>
        /// Takes the damage using the specified damage info
        /// </summary>
        /// <param name="damageInfo">The damage info</param>
        /// <returns>The damage result</returns>
        public DamageResult TakeDamage(DamageInfo damageInfo)
        {
            if (Destroyed)
            {
                return new DamageResult();
            }

            if (damageInfo.Amount <= 0f)
            {
                return new DamageResult();
            }
            
            PreTakeDamage(ref damageInfo, out var absorbed);

            if (absorbed)
            {
                return new DamageResult();
            }

            var damageResult = damageInfo.Apply(this);
            
            //todo notify damage taken

            if (damageInfo.Template.isAttack)
            {
                //todo splatter blood
                
                //todo record damage taken
            }
            
            PostTakeDamage(damageInfo, damageResult.totalDamage);

            return damageResult;
        }
        
        /// <summary>
        /// Pres the take damage using the specified damage info
        /// </summary>
        /// <param name="damageInfo">The damage info</param>
        /// <param name="absorbed">The absorbed</param>
        public virtual void PreTakeDamage(ref DamageInfo damageInfo, out bool absorbed)
        {
            absorbed = false;
        }
        
        /// <summary>
        /// Posts the take damage using the specified damage info
        /// </summary>
        /// <param name="damageInfo">The damage info</param>
        /// <param name="totalDamageDealt">The total damage dealt</param>
        public virtual void PostTakeDamage(DamageInfo damageInfo, float totalDamageDealt)
        {
        }

        /// <summary>
        /// Kills the damage info
        /// </summary>
        /// <param name="damageInfo">The damage info</param>
        public virtual void Kill(DamageInfo damageInfo)
        {
            Destroy();
        }

        /// <summary>
        /// Destroys this instance
        /// </summary>
        public virtual void Destroy() //todo destroy mode
        {
            //todo
        }

        /// <summary>
        /// Ticks this instance
        /// </summary>
        public virtual void Tick()
        {
            //todo
        }
        
        /// <summary>
        /// Ticks the rare
        /// </summary>
        public virtual void TickRare()
        {
            //todo
        }
        
        /// <summary>
        /// Ticks the long
        /// </summary>
        public virtual void TickLong()
        {
            //todo
        }

        /// <summary>
        /// Generates the id
        /// </summary>
        public void GenerateId()
        {
            //todo stick into thing maker and ensure unique ids

            id = 0;
        }
        
        /// <summary>
        /// Updates the sprite facing using the specified direction
        /// </summary>
        /// <param name="direction">The direction</param>
        public void UpdateSpriteFacing(Direction direction)
        {
            SpriteInstance.GetComponentInChildren<SpriteRenderer>().sprite = directionalSprites[direction];
        }
        
        /// <summary>
        /// Populates the sprites
        /// </summary>
        protected void PopulateSprites()
        {
            //for one sprite could just use same dictionary amd assign same sprite to all directions
            //determine number of sprites depending on sprite sheet size?

            if (spriteSheet == null)
            {
                return;
            }

            directionalSprites = new Dictionary<Direction, Sprite>
            {
                {Direction.DOWN, null},
                {Direction.UP, null},
                {Direction.LEFT, null},
                {Direction.RIGHT, null}
            };
            
            const int Width = 32;
            
            const int Height = 32;

            var colIndex = 0;

            foreach (var direction in directionalSprites.Keys.ToArray())
            {
                var x = colIndex * Width;
                
                var sprite = Sprite.Create(spriteSheet.texture, new Rect(x, 0, Width, Height), new Vector2(0.5f, 0.5f), 32);
                
                directionalSprites[direction] = sprite;

                colIndex++;
            }
        }

        protected virtual void UpdateGraphics() //todo add to an interface?
        {
        }
    }
}
