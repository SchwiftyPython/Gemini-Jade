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
    public class DamageInfo
    {
        //todo ThingTemplate _weapon;

        public Thing Attacker { get; set; }
        
        public Thing Target { get; }

        public DamageTemplate Template { get; set; }

        public float ArmorPenetration { get; }

        public bool IgnoreArmor { get; set; }

        public BodyPart HitPart { get; set; }

        public BodyPartHeight Height { get; set; }

        public BodyPartDepth Depth { get; set; }

        public BodyPartGroupTemplate WeaponBodyPartGroup { get; set; }

        public HealthModTemplate WeaponHealthMod { get; set; }

        public bool AllowDamageToSpread { get; set; }

        public float Amount { get; set; }

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

        public void SetBodyArea(BodyPartHeight height, BodyPartDepth depth)
        {
            Height = height;
            Depth = depth;
        }
    }
}