using Assets.Scripts.World.Pawns.Species;
using UnityEngine;
using World.Pawns;

namespace Repos
{
    public class PawnRepo : MonoBehaviour
    {
        [SerializeField] private SpeciesTemplate humanTemplate;
        
        //todo list of SpeciesTemplates. We won't know what they are specifically.
        //Basically have faction templates store what species are valid and choose from that.
        //Can also just grab a random one from here

        public static Pawn CreatePawn(SpeciesTemplate template)
        {
            return template.NewPawn();
        }

        public SpeciesTemplate GetHumanTemplate()
        {
            //just for testing and debug
            
            return humanTemplate;
        }
    }
}
