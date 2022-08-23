using System.Linq;
using UnityEngine;
using World.Pawns.Health.DamageWorkers;
using World.Things;

namespace World.Pawns.Actions
{
    /// <summary>
    /// The melee attack class
    /// </summary>
    /// <seealso cref="Action"/>
    public class MeleeAttack : Action
    {
        /// <summary>
        /// The damage modifier min
        /// </summary>
        private const float DamageModifierMin = 0.4f;

        /// <summary>
        /// The damage modifier max
        /// </summary>
        private const float DamageModifierMax = 1.6f;
        
        /// <summary>
        /// Describes whether this instance try action
        /// </summary>
        /// <returns>The bool</returns>
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

        /// <summary>
        /// Applies the damage to using the specified target thing
        /// </summary>
        /// <param name="targetThing">The target thing</param>
        /// <returns>The damage result</returns>
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

        /// <summary>
        /// Describes whether auto hits
        /// </summary>
        /// <param name="targetThing">The target thing</param>
        /// <returns>The bool</returns>
        private static bool AutoHits(Thing targetThing)
        {
            return true;
            
            return TargetIsImmobile(targetThing);
        }

        /// <summary>
        /// Describes whether target is immobile
        /// </summary>
        /// <param name="targetThing">The target thing</param>
        /// <returns>The bool</returns>
        private static bool TargetIsImmobile(Thing targetThing)
        {
            return targetThing is not Pawn targetPawn || targetPawn.IsImmobile();
        }
    }
}
