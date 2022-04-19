using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.World.Pawns;
using Assets.Scripts.World.Pawns.Species;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Utilities
{
    public class HealthDebug : MonoBehaviour
    {
        private Pawn _currentPawn;

        private Dictionary<string, BodyPart> _bodyPartsDict;

        //todo move these to a more central location -- probably some ui handler 
        public delegate void SelectPawn(Pawn pawn);
        public static event SelectPawn OnPawnSelected;

        public SpeciesTemplate humanTemplate;

        public Dropdown bodyPartsDropdown;

        //todo add health mods to call or we can assign health mods directly to buttons and not worry about it here

        private void Start()
        {
            OnPawnSelected += HealthDebug_OnPawnSelected;
        }

        public void CreatePawn()
        {
            var pawn = humanTemplate.NewPawn();

            OnPawnSelected?.Invoke(pawn);
        }

        public void RemoveBodyPart()
        {
            //todo select a valid non missing body part

            //todo apply missing body part health mod
        }

        private void PopulateBodyPartDropdown()
        {
            bodyPartsDropdown.ClearOptions();

            if (_currentPawn == null)
            {
                Debug.LogError("Can't populate body parts list! No pawn selected!");
                return;
            }

            var parts = _currentPawn.GetBody();

            _bodyPartsDict = new Dictionary<string, BodyPart>();

            foreach (var part in parts)
            {
                if (_bodyPartsDict.ContainsKey(part.LabelCapitalized) || part.IsMissing())
                {
                    continue;
                }

                _bodyPartsDict.Add(part.LabelCapitalized, part);
            }

            bodyPartsDropdown.AddOptions(_bodyPartsDict.Keys.ToList());
        }

        private void HealthDebug_OnPawnSelected(Pawn pawn)
        {
            _currentPawn = pawn;

            PopulateBodyPartDropdown();
        }
    }
}
