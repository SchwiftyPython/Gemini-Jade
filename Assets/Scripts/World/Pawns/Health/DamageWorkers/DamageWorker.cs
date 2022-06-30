using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.World.Pawns;
using UnityEngine;
using Utilities;
using World.Pawns.BodyPartGroupTemplates;
using World.Pawns.Health.DamageTemplates;
using World.Pawns.Health.HealthModifiers;
using World.Things;

namespace World.Pawns.Health.DamageWorkers
{
    public class DamageWorker
    {
        public DamageTemplate template;

        public virtual DamageResult Apply(DamageInfo damageInfo, Thing target)
        {
            if (target is Pawn pawn)
            {
                return ApplyToPawn(damageInfo, pawn);
            }
            
            var damageResult = new DamageResult();
            
            //todo check if target is spawned -- then play impact sound if true

            if (!target.template.useHitPoints)
            {
                return damageResult;
            }

            if (!damageInfo.Template.damagesHp)
            {
                return damageResult;
            }

            var damageAmount = damageInfo.Amount;
                
            //todo check if target is building or plant

            damageResult.totalDamage = Mathf.Min(target.HitPoints, Mathf.RoundToInt(damageAmount));

            target.HitPoints -= Mathf.RoundToInt(damageResult.totalDamage);

            if (target.HitPoints > 0)
            {
                return damageResult;
            }

            target.HitPoints = 0;
                    
            target.Kill(damageInfo);

            return damageResult;
        }

        protected virtual BodyPart ChooseHitPart(DamageInfo damageInfo, Pawn target)
        {
            return target.health.GetRandomBodyPart(damageInfo.Height, damageInfo.Depth);
        }

        protected void AddInjury(Pawn target, float totalDamage, DamageInfo damageInfo, DamageResult damageResult)
        {
            if (target.health.BodyPartIsMissing(damageInfo.HitPart))
            {
                return;
            }
            
            var healthModTemplate = damageInfo.Template.healthModTemplate;

            if (healthModTemplate == null)
            {
                return;
            }
            
            var injury = (Injury)HealthModMaker.MakeHealthMod(healthModTemplate, target, damageInfo.HitPart);
            
            //todo injury.source = damageInfo.Weapon;

            //todo injury.sourceBodyPartGroup = damageInfo.WeaponBodyPartGroup;

            //todo injury.sourceHealthMod = damageInfo.WeaponHealthMod;

            injury.Severity = totalDamage;

            AddInjury(target, injury, damageInfo, damageResult);
        }
        
        protected void AddInjury(Pawn target, Injury injury, DamageInfo damageInfo, DamageResult damageResult)
        {
            injury.GetComp<HealthModifierComponents.GetsPermanent>()?.PreFinalizeInjury();
            
            target.health.AddHealthMod(injury, injury.Part);

            damageResult.wounded = true;
            
            damageResult.AddBodyPart(target, injury.Part);
            
            damageResult.AddHealthMod(injury);
        }

        private DamageResult ApplyToPawn(DamageInfo damageInfo, Pawn target)
        {
            var damageResult = new DamageResult();

            if (damageInfo.Amount <= 0f)
            {
                return damageResult;
            }
            
            //todo get pawn's map for sound location

            ApplyDamageToPart(damageInfo, target, damageResult);

            if (damageInfo.AllowDamageToSpread)
            {
                var numPartsToSpreadDamageTo = Random.Range(1, 3);
                
                for(var i = 0; i < numPartsToSpreadDamageTo; i++)
                {
                    SpreadDamage(damageInfo, target, damageResult);
                }
            }

            if (damageResult.wounded)
            {
                //todo play wounded sound
                
                //todo damage special effects
            }

            if (damageResult.headshot && target.spawned) 
            {
                //todo log headshot in attacker's stats
            }

            if ((damageResult.deflected || damageResult.reduced) && target.spawned)
            {
                //todo deflected special effects
                
                //todo play deflected sound
            }

            return damageResult;
        }

        private void ApplyDamageToPart(DamageInfo damageInfo, Pawn target, DamageResult damageResult)
        {
            damageInfo.HitPart = GetPartFromDamageInfo(damageInfo, target);  
            
            if (damageInfo.HitPart == null)
            {
                return;
            }

            var damage = damageInfo.Amount;

            if (!damageInfo.IgnoreArmor)
            {
                var damageTemplate = damageInfo.Template;
                
                //todo get damage after armor reduction
                
                //todo if reduced damage is less than damage, set result.reduced to true
            }

            if (damageInfo.Template.isAttack)
            {
                //todo damage *= pawn's incoming damage modifier
            }

            if (damage <= 0)
            {
                damageResult.AddBodyPart(target, damageInfo.HitPart);
                
                damageResult.deflected = true;
                
                return;
            }

            if (IsHeadshot(damageInfo))
            {
                damageResult.headshot = true;
            }
            
            var healthModTemplate = damageInfo.Template.healthModTemplate;

            if (healthModTemplate == null)
            {
                return;
            }

            if (healthModTemplate.GetCompPropsFor(typeof(World.Pawns.Health.HealthModifierComponents.GetsPermanent)) == null)
            {
                return;
            }

            if (damageInfo.HitPart.template.permanentInjuryChanceFactor > 0f)
            {
                AddInjury(target, damage, damageInfo, damageResult);
            }
        }

        private BodyPart GetPartFromDamageInfo(DamageInfo damageInfo, Pawn pawn)
        {
            if (damageInfo.HitPart == null)
            {
                return ChooseHitPart(damageInfo, pawn);
            }

            return pawn.health.GetExistingParts().Any(part => part == damageInfo.HitPart) ? damageInfo.HitPart : null;
        }

        private void SpreadDamage(DamageInfo damageInfo, Pawn target, DamageResult damageResult)
        {
            if (damageInfo.HitPart == null)
            {
                return;
            }

            if (!damageInfo.Template.damagesHp)
            {
                return;
            }

            if (damageInfo.HitPart.parent == null)
            {
                if (!damageInfo.HitPart.HasChildParts())
                {
                    return;
                }
            }

            if (!(damageInfo.Amount >= 10f))
            {
                return;
            }

            var hitPart = damageInfo.HitPart;
                
            if(hitPart.parent != null)
            {
                hitPart = hitPart.parent;
            }
            else
            {
                var potentialHitParts = new List<BodyPart>();

                var existingParts = target.health.GetExistingParts();
                
                foreach(var childPart in hitPart.GetAllChildren())
                {
                    if (!existingParts.Contains(childPart))
                    {
                        continue;
                    }
                    
                    potentialHitParts.Add(childPart);
                }
                
                var hitPartIndex = Random.Range(0, potentialHitParts.Count);
                
                hitPart = potentialHitParts[hitPartIndex];
            }

            var damageInfoCopy = new DamageInfo(damageInfo)
            {
                HitPart = hitPart
            };

            ApplyDamageToPart(damageInfoCopy, target, damageResult);
        }

        private static bool IsHeadshot(DamageInfo damageInfo)
        {
            var bodyPartGroupRepo = Object.FindObjectOfType<BodyPartGroupRepo>();

            if (damageInfo.HitPart.IsInGroup(bodyPartGroupRepo.fullHead))
            {
                return true; //todo check if damage template is not area of effect
            }

            return false;
        }
    }
}
