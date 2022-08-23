using System.Collections.Generic;
using Assets.Scripts.World.Pawns.BodyPartTags;
using UnityEngine;
using World.Pawns;
using World.Pawns.Health.DamageTemplates;

namespace Assets.Scripts.World.Pawns.BodyPartTemplates
{
    /// <summary>
    /// The body part template class
    /// </summary>
    /// <seealso cref="Template"/>
    [CreateAssetMenu(menuName = "Templates/BodyPartTemplate")]
    public class BodyPartTemplate : Template
    {
        /// <summary>
        /// The body part tag template
        /// </summary>
        public List<BodyPartTagTemplate> tags = new List<BodyPartTagTemplate>();

        /// <summary>
        /// The hit points
        /// </summary>
        public int hitPoints = 10;

		/// <summary>
		/// The permanent injury chance factor
		/// </summary>
		public float permanentInjuryChanceFactor = 1f;

        /// <summary>
        /// The bleed rate
        /// </summary>
        public float bleedRate = 1f;

        /// <summary>
        /// The frostbite vulnerability
        /// </summary>
        public float frostbiteVulnerability;

        /// <summary>
        /// The skin covered
        /// </summary>
        public bool skinCovered;

        /// <summary>
        /// The solid
        /// </summary>
        public bool solid;

        /// <summary>
        /// The alive
        /// </summary>
        public bool alive = true;

        /// <summary>
        /// The can scarify
        /// </summary>
        public bool canScarify;

        /// <summary>
        /// The beauty related
        /// </summary>
        public bool beautyRelated;

        /// <summary>
        /// The conceptual
        /// </summary>
        public bool conceptual;

        /// <summary>
        /// The delicate
        /// </summary>
        public bool delicate;

        /// <summary>
        /// The pawn generator can amputate
        /// </summary>
        public bool pawnGeneratorCanAmputate;

        /// <summary>
        /// The destroyable by damage
        /// </summary>
        public bool destroyableByDamage = true;

        /// <summary>
        /// Gets the max health using the specified pawn
        /// </summary>
        /// <param name="pawn">The pawn</param>
        /// <returns>The float</returns>
        public float GetMaxHealth(Pawn pawn)
        {
            return Mathf.CeilToInt((float)hitPoints); //todo multiply by pawn health scale
        }
    }
}
