using System.Collections.Generic;
using Assets.Scripts.World.Pawns.BodyPartTags;
using UnityEngine;
using World.Pawns;

namespace Assets.Scripts.World.Pawns.BodyPartTemplates
{
    [CreateAssetMenu(menuName = "Templates/BodyPartTemplate")]
    public class BodyPartTemplate : Template
    {
        public List<BodyPartTagTemplate> tags = new List<BodyPartTagTemplate>();

        public int hitPoints = 10;

		public float permanentInjuryChanceFactor = 1f;

        public float bleedRate = 1f;

        public float frostbiteVulnerability;

        public bool skinCovered;

        public bool solid;

        public bool alive = true;

        public bool canScarify;

        public bool beautyRelated;

        public bool conceptual;

        public bool pawnGeneratorCanAmputate;

        public bool destroyableByDamage = true;

        public float GetMaxHealth(Pawn pawn)
        {
            return Mathf.CeilToInt((float)hitPoints); //todo multiply by pawn health scale
        }

    }
}
