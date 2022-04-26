using Assets.Scripts.World.Pawns.BodyTemplates;
using UnityEngine;
using World.Pawns;
using World.Pawns.Health;

namespace Assets.Scripts.World.Pawns.Species
{
    [CreateAssetMenu(menuName = "Templates/SpeciesTemplate")]
    public class SpeciesTemplate : Template
    {
        public int minTemp;
        public int maxTemp;

        //todo Tools

        public bool hasGenders = true;

        public bool needsRest = true;

        public BodyTemplate bodyTemplate;

        public float lifeExpectancy = 10f;

        public float baseBodySize = 1f;

        public float baseHealthScale = 1f;

        public float baseHungerRate = 1f;
        
        public FleshType fleshType = FleshType.Organic;

        public Intellect intellect;

        public Pawn NewPawn()
        {
            return new Pawn(this);
        }
    }
}
