using Assets.Scripts.World.Pawns;
using Assets.Scripts.World.Pawns.BodyPartDepth;
using Assets.Scripts.World.Pawns.BodyPartGroupTemplates;
using Assets.Scripts.World.Pawns.BodyPartHeight;
using UnityEngine;
using Utilities;
using World.Pawns.Health.DamageTemplates;
using World.Pawns.Health.HealthModifiers;
using World.Things;

namespace World.Pawns.Health.DamageWorkers
{
    /// <summary>
    /// The damage info class
    /// </summary>
    public class DamageInfo
    {
        //todo ThingTemplate _weapon;

        /// <summary>
        /// Gets or sets the value of the attacker
        /// </summary>
        public Thing Attacker { get; set; }
        
        /// <summary>
        /// Gets the value of the target
        /// </summary>
        public Thing Target { get; }

        /// <summary>
        /// Gets or sets the value of the template
        /// </summary>
        public DamageTemplate Template { get; set; }

        /// <summary>
        /// Gets the value of the armor penetration
        /// </summary>
        public float ArmorPenetration { get; }

        /// <summary>
        /// Gets or sets the value of the ignore armor
        /// </summary>
        public bool IgnoreArmor { get; set; }

        /// <summary>
        /// Gets or sets the value of the hit part
        /// </summary>
        public BodyPart HitPart { get; set; }

        /// <summary>
        /// Gets or sets the value of the height
        /// </summary>
        public BodyPartHeight Height { get; set; }

        /// <summary>
        /// Gets or sets the value of the depth
        /// </summary>
        public BodyPartDepth Depth { get; set; }

        /// <summary>
        /// Gets or sets the value of the weapon body part group
        /// </summary>
        public BodyPartGroupTemplate WeaponBodyPartGroup { get; set; }

        /// <summary>
        /// Gets or sets the value of the weapon health mod
        /// </summary>
        public HealthModTemplate WeaponHealthMod { get; set; }

        /// <summary>
        /// Gets or sets the value of the allow damage to spread
        /// </summary>
        public bool AllowDamageToSpread { get; set; }

        /// <summary>
        /// Gets or sets the value of the amount
        /// </summary>
        public float Amount { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DamageInfo"/> class
        /// </summary>
        /// <param name="template">The template</param>
        /// <param name="amount">The amount</param>
        /// <param name="armorPenetration">The armor penetration</param>
        /// <param name="hitPart">The hit part</param>
        /// <param name="attacker">The attacker</param>
        /// <param name="target">The target</param>
        public DamageInfo(DamageTemplate template, float amount, float armorPenetration = 0f, BodyPart hitPart = null, Thing attacker = null, Thing target = null) //todo weaponTemplate
        {
            Template = template;
            Amount = amount;
            ArmorPenetration = armorPenetration;
            HitPart = hitPart;

            var healthUtils = Object.FindObjectOfType<HealthUtils>();

            Height = healthUtils.heightUndefined;
            Depth = healthUtils.depthUndefined;
            
            //todo weapon

            WeaponBodyPartGroup = null;
            WeaponHealthMod = null;
            AllowDamageToSpread = true;
            IgnoreArmor = false;
            Attacker = attacker;
            Target = target;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DamageInfo"/> class
        /// </summary>
        /// <param name="damageInfo">The damage info</param>
        public DamageInfo(DamageInfo damageInfo)
        {
            Template = damageInfo.Template;
            Amount = damageInfo.Amount;
            ArmorPenetration = damageInfo.ArmorPenetration;
            HitPart = damageInfo.HitPart;
            Height = damageInfo.Height;
            Depth = damageInfo.Depth;
            WeaponBodyPartGroup = damageInfo.WeaponBodyPartGroup;
            WeaponHealthMod = damageInfo.WeaponHealthMod;
            AllowDamageToSpread = damageInfo.AllowDamageToSpread;
            IgnoreArmor = damageInfo.IgnoreArmor;
            Attacker = damageInfo.Attacker;
            Target = damageInfo.Target;
        }

        /// <summary>
        /// Sets the body area using the specified height
        /// </summary>
        /// <param name="height">The height</param>
        /// <param name="depth">The depth</param>
        public void SetBodyArea(BodyPartHeight height, BodyPartDepth depth)
        {
            Height = height;
            Depth = depth;
        }

        /// <summary>
        /// Applies the target
        /// </summary>
        /// <param name="target">The target</param>
        /// <returns>The damage result</returns>
        public DamageResult Apply(Thing target)
        {
            return Template.Worker.Apply(this, target);
        }
    }
}
