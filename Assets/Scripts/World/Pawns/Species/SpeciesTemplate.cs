using System.Collections.Generic;
using Assets.Scripts.World.Pawns.BodyTemplates;
using UnityEngine;
using World.Pawns;
using World.Pawns.Health;
using World.Pawns.Health.HealthModifierAdderSetTemplates;
using World.Things;

namespace Assets.Scripts.World.Pawns.Species
{
    /// <summary>
    /// The species template class
    /// </summary>
    /// <seealso cref="ThingTemplate"/>
    [CreateAssetMenu(menuName = "Templates/SpeciesTemplate")]
    public class SpeciesTemplate : ThingTemplate
    {
        /// <summary>
        /// The min temp
        /// </summary>
        public int minTemp;
        /// <summary>
        /// The max temp
        /// </summary>
        public int maxTemp;

        //todo Tools

        /// <summary>
        /// The has genders
        /// </summary>
        public bool hasGenders = true;

        /// <summary>
        /// The needs rest
        /// </summary>
        public bool needsRest = true;

        /// <summary>
        /// The body template
        /// </summary>
        public BodyTemplate bodyTemplate;

        /// <summary>
        /// The life expectancy
        /// </summary>
        public float lifeExpectancy = 10f;

        /// <summary>
        /// The health mod adder sets
        /// </summary>
        public List<HealthModAdderSetTemplate> healthModAdderSets;

        /// <summary>
        /// The base body size
        /// </summary>
        public float baseBodySize = 1f;

        /// <summary>
        /// The base health scale
        /// </summary>
        public float baseHealthScale = 1f;

        /// <summary>
        /// The base hunger rate
        /// </summary>
        public float baseHungerRate = 1f;
        
        /// <summary>
        /// The organic
        /// </summary>
        public FleshType fleshType = FleshType.Organic;

        /// <summary>
        /// The intellect
        /// </summary>
        public Intellect intellect;
        
        /// <summary>
        /// The base speed
        /// </summary>
        public float baseSpeed = 1f;

        /// <summary>
        /// News the pawn
        /// </summary>
        /// <returns>The pawn</returns>
        public Pawn NewPawn()
        {
            return new Pawn(this);
        }
    }
}
