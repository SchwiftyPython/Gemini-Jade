using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.World.Pawns;
using Assets.Scripts.World.Pawns.Species;
using UI;
using UnityEngine;
using UnityEngine.UI;
using Utilities;
using World.Pawns;
using World.Pawns.Health.DamageTemplates;
using World.Pawns.Health.DamageWorkers;
using World.Pawns.Health.HealthFunctions;
using World.Pawns.Health.HealthModifiers;
using World.Things;

namespace Assets.Scripts.Utilities
{
    /// <summary>
    /// The health debug class
    /// </summary>
    /// <seealso cref="MonoBehaviour"/>
    public class HealthDebug : MonoBehaviour
    {
        /// <summary>
        /// The current pawn
        /// </summary>
        private Pawn _currentPawn;

        /// <summary>
        /// The body parts dict
        /// </summary>
        private Dictionary<string, BodyPart> _bodyPartsDict;

        //todo move these to a more central location -- probably some ui handler 
        /// <summary>
        /// The select pawn
        /// </summary>
        public delegate void SelectPawn(Pawn pawn);
        public static event SelectPawn OnPawnSelected;

        /// <summary>
        /// The body changed
        /// </summary>
        public delegate void BodyChanged();
        public static event BodyChanged OnBodyChanged;

        /// <summary>
        /// The human template
        /// </summary>
        public SpeciesTemplate humanTemplate;

        /// <summary>
        /// The remove body part template
        /// </summary>
        public HealthModTemplate removeBodyPartTemplate;
        /// <summary>
        /// The cut body part template
        /// </summary>
        public HealthModTemplate cutBodyPartTemplate;

        /// <summary>
        /// The body parts dropdown
        /// </summary>
        public Dropdown bodyPartsDropdown;

        //todo add health mods to call or we can assign health mods directly to buttons and not worry about it here

        /// <summary>
        /// Starts this instance
        /// </summary>
        private void Start()
        {
            OnPawnSelected += HealthDebug_OnPawnSelected;
            OnBodyChanged += HealthDebug_OnBodyChanged;
        }

        /// <summary>
        /// Creates the pawn
        /// </summary>
        public void CreatePawn()
        {
            var pawn = humanTemplate.NewPawn();

            OnPawnSelected?.Invoke(pawn);
        }

        /// <summary>
        /// Notifies the body changed
        /// </summary>
        public static void NotifyBodyChanged()
        {
            OnBodyChanged?.Invoke();
        }

        /// <summary>
        /// Removes the body part
        /// </summary>
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

            if (_currentPawn.health.BodyPartIsMissing(partToRemove))
            {
                Debug.LogError($"Can't remove {partName}! Body Part is already missing!");
                return;
            }

            //todo get health mod from Damage Info

            var removePartMod = HealthModMaker.MakeHealthMod(removeBodyPartTemplate, _currentPawn, partToRemove);

            _currentPawn.health.AddHealthMod(removePartMod, partToRemove);
            
            OnBodyChanged?.Invoke();
            
            PopulateBodyPartDropdown();
        }

        /// <summary>
        /// Cuts the pawn
        /// </summary>
        public void CutPawn()
        {
            var damageTemplateRepo = FindObjectOfType<DamageTemplateRepo>();

            var cutTemplate = damageTemplateRepo.cutDamageTemplate;
            
            //todo weapon/source of damage

            var damageAmount = cutTemplate.baseDamage;
            
            damageAmount = (int) Random.Range(damageAmount * .04f, damageAmount * 1.6f);

            var attacker = humanTemplate.NewPawn();

            var damageInfo = new DamageInfo(cutTemplate, damageAmount, cutTemplate.baseArmorPen, null, attacker, _currentPawn);

            var healthUtils = FindObjectOfType<HealthUtils>();
            
            damageInfo.SetBodyArea(healthUtils.heightUndefined, healthUtils.outside);
            
            var damageResult = _currentPawn.TakeDamage(damageInfo);

            Debug.Log($"Successfully cut pawn!");
            
            Debug.Log(damageResult.ToString());

            OnBodyChanged?.Invoke();
            
            PopulateBodyPartDropdown();
        }

        /// <summary>
        /// Attacks the with weapon
        /// </summary>
        public void AttackWithWeapon()
        {
            //todo have a weapon select dropdown
            
            var knifeTemplate = FindObjectOfType<ThingTemplateRepo>().knifeTemplateTest;
            
            var knife = knifeTemplate.MakeThing();

            var parts = knife.template.Parts;
            
            var partToUse = parts[Random.Range(0, parts.Count)];

            var partAction = partToUse.action;

            partAction.target = _currentPawn;

            var success = partAction.TryAction();
            
            Debug.Log($"Attempt to attack with {knifeTemplate.label} {partToUse.label}: {success}");
        }

        /// <summary>
        /// Populates the body part dropdown
        /// </summary>
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
                if (_bodyPartsDict.ContainsKey(part.LabelCapitalized) || _currentPawn.health.BodyPartIsMissing(part))
                {
                    continue;
                }

                _bodyPartsDict.Add(part.LabelCapitalized, part);
            }

            bodyPartsDropdown.AddOptions(_bodyPartsDict.Keys.ToList());
        }

        /// <summary>
        /// Draws the health summary
        /// </summary>
        private void DrawHealthSummary()
        {
            var healthSummary = Object.FindObjectOfType<HealthSummary>();

            var functionLevels = _currentPawn.health.GetFunctionValues();

            var functionPercentages = new Dictionary<HealthFunctionTemplate, int>();

            foreach (var function in functionLevels.Keys) //todo stick conversion to percentage part in textutils 
            {
                var value = functionLevels[function];

                var percent = (int) (value * 100);
                
                functionPercentages.Add(function, percent);
            }
            
            healthSummary.Draw(functionPercentages);
        }

        /// <summary>
        /// Healths the debug on pawn selected using the specified pawn
        /// </summary>
        /// <param name="pawn">The pawn</param>
        private void HealthDebug_OnPawnSelected(Pawn pawn)
        {
            _currentPawn = pawn;

            PopulateBodyPartDropdown();
        }

        /// <summary>
        /// Healths the debug on body changed
        /// </summary>
        public void HealthDebug_OnBodyChanged()
        {
            DrawHealthSummary();
        }
    }
}
