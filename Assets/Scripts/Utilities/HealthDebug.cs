using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.World.Pawns;
using Assets.Scripts.World.Pawns.Health.HealthModifiers;
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

        public delegate void BodyChanged();
        public static event BodyChanged OnBodyChanged;

        public SpeciesTemplate humanTemplate;

        public HealthModTemplate removeBodyPartTemplate;

        public Dropdown bodyPartsDropdown;

        //todo add health mods to call or we can assign health mods directly to buttons and not worry about it here

        private void Start()
        {
            OnPawnSelected += HealthDebug_OnPawnSelected;
            OnBodyChanged += HealthDebug_OnBodyChanged;
        }

        public void CreatePawn()
        {
            var pawn = humanTemplate.NewPawn();

            OnPawnSelected?.Invoke(pawn);
        }

        public void RemoveBodyPart()
        {
            var partName = bodyPartsDropdown.options[bodyPartsDropdown.value].text;

            if (string.IsNullOrEmpty(partName))
            {
                Debug.LogError("Can't remove body part! No part selected!");
                return;
            }

            if (!_bodyPartsDict.ContainsKey(partName))
            {
                Debug.LogError($"Can't remove {partName}! Doesn't exist in Body Parts Dictionary!");
                return;
            }

            //todo make some equivalent to Damage Worker class and Add Injury Subclass or method

            var partToRemove = _bodyPartsDict[partName];

            if (partToRemove.IsMissing())
            {
                Debug.LogError($"Can't remove {partName}! Body Part is already missing!");
                return;
            }

            //todo get health mod from Damage Info

            var removePartMod = HealthModMaker.MakeHealthMod(removeBodyPartTemplate, _currentPawn, partToRemove);

            //todo this might be why they have HeDiffSet in Pawn. Adding the injury to the pawn rather than body part directly.
            //todo we can do similar thing by using a method in pawn as a wrapper then adding mod to the part.

            partToRemove.AddHealthMod(removePartMod);

            OnBodyChanged?.Invoke();
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

        private void HealthDebug_OnBodyChanged()
        {
            PopulateBodyPartDropdown();
        }
    }
}
