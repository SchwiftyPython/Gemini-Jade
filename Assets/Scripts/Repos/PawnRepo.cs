using System.Collections.Generic;
using Assets.Scripts.World.Pawns.Species;
using UnityEngine;
using World.Pawns;
using World.Pawns.Skills;

namespace Repos
{
    /// <summary>
    /// The pawn repo class
    /// </summary>
    /// <seealso cref="MonoBehaviour"/>
    public class PawnRepo : MonoBehaviour
    {
        /// <summary>
        /// The human template
        /// </summary>
        [SerializeField] private SpeciesTemplate humanTemplate;
        
        //todo list of SpeciesTemplates. We won't know what they are specifically.
        //Basically have faction templates store what species are valid and choose from that.
        //Can also just grab a random one from here

        /// <summary>
        /// The skills
        /// </summary>
        [SerializeField] private List<Skill> skills;

        /// <summary>
        /// Creates the pawn using the specified template
        /// </summary>
        /// <param name="template">The template</param>
        /// <returns>The pawn</returns>
        public static Pawn CreatePawn(SpeciesTemplate template)
        {
            return template.NewPawn();
        }

        /// <summary>
        /// Gets the human template
        /// </summary>
        /// <returns>The human template</returns>
        public SpeciesTemplate GetHumanTemplate()
        {
            //just for testing and debug
            
            return humanTemplate;
        }
        
        /// <summary>
        /// Gets the skills
        /// </summary>
        /// <returns>The skills</returns>
        public List<Skill> GetSkills()
        {
            return skills;
        }
    }
}
