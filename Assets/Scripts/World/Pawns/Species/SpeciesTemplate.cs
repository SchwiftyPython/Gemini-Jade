using System.Collections.Generic;
using Assets.Scripts.World.Pawns.BodyTemplates;
using UnityEngine;
using World.Pawns;
using World.Pawns.Health;
using World.Pawns.Health.HealthModifierAdderSetTemplates;
using World.Things;

namespace Assets.Scripts.World.Pawns.Species
{
    [CreateAssetMenu(menuName = "Templates/SpeciesTemplate")]
    public class SpeciesTemplate : ThingTemplate
    {
        public int minTemp;
        public int maxTemp;

        //todo Tools

        public bool hasGenders = true;

        public bool needsRest = true;

        public BodyTemplate bodyTemplate;

        public float lifeExpectancy = 10f;

        public List<HealthModAdderSetTemplate> healthModAdderSets;

        public float baseBodySize = 1f;

        public float baseHealthScale = 1f;

        public float baseHungerRate = 1f;
        
        public FleshType fleshType = FleshType.Organic;

        public Intellect intellect;
        
        public float baseSpeed = 1f;

        public Pawn NewPawn()
        {
            return new Pawn(this);
        }
    }
}
