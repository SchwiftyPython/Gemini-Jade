using Assets.Scripts.World.Pawns;
using Assets.Scripts.World.Pawns.Species;
using UnityEngine;

namespace Assets.Scripts.Utilities
{
    public class HealthDebug : MonoBehaviour
    {
        //todo move these to a more central location -- probably some ui handler 
        public delegate void SelectPawn(Pawn pawn);
        public static event SelectPawn OnPawnSelected;

        public SpeciesTemplate humanTemplate;

        //todo add health mods to call or we can assign health mods directly to buttons and not worry about it here

        public void CreatePawn()
        {
            var pawn = humanTemplate.NewPawn();

            OnPawnSelected?.Invoke(pawn);
        }
    }
}
