using System.Linq;
using UnityEngine;
using World.Pawns.Health.DamageWorkers;
using World.Things;

namespace World.Pawns.Actions
{
    public class MeleeAttack : Action
    {
        private const float DamageModifierMin = 0.4f;

        private const float DamageModifierMax = 1.6f;
        
        public override bool TryAction()
        {
            var userPawn = user as Pawn;
            
            //todo check if spawned

            if (!CanHit(target))
            {
                return false;
            }
            
            //todo increase melee skill
            
            //todo notify target of melee threat

            if (AutoHits(target)) //todo or melee chance hits and target didn't dodge
            {
                //todo sounds and special effects
                
                //todo log combat event

                var damageResult = ApplyDamageTo(target);
                
                Debug.Log($"Damage dealt: {damageResult.totalDamage}");
                
                Debug.Log(
                    $"Parts hit: {string.Join(", ", damageResult.parts.Select(p => p.LabelCapitalized).ToArray())}");
                
                //todo check if any damage dealt
                
                //todo if none show deflected effects and sounds
            }
            else
            {
                //todo dodge sound

                return false;
            }

            return true;
        }

        protected virtual DamageResult ApplyDamageTo(Thing targetThing)
        {
            var damageAmount = damageTemplate.baseDamage;

            damageAmount = (int) Random.Range(damageAmount * DamageModifierMin, damageAmount * DamageModifierMax);

            var damageInfo = new DamageInfo(damageTemplate, damageAmount, damageTemplate.baseArmorPen, null, user,
                targetThing);

            damageInfo.SetBodyArea(damageTemplate.height, damageTemplate.depth);

            var damageResult = targetThing.TakeDamage(damageInfo);

            Debug.Log($"Successfully melee attacked pawn! Damage: {damageAmount}");

            return damageResult;
        }

        private static bool AutoHits(Thing targetThing)
        {
            return true;
            
            return TargetIsImmobile(targetThing);
        }

        private static bool TargetIsImmobile(Thing targetThing)
        {
            return targetThing is not Pawn targetPawn || targetPawn.IsImmobile();
        }
    }
}
