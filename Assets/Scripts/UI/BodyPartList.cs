using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Utilities;
using Assets.Scripts.World.Pawns;
using UnityEngine;
using World.Pawns;
using World.Pawns.Health.HealthModifiers;

namespace UI
{
    public class BodyPartList : MonoBehaviour
    {
        [SerializeField] private BodyPartStatus bodyPartPrefab;

        private Pawn _currentPawn;

        private void Start()
        {
            HealthDebug.OnPawnSelected += HealthDebug_OnPawnSelected;
            HealthDebug.OnBodyChanged += HealthDebug_OnBodyChanged;
        }

        private void SetCurrentPawn(Pawn pawn)
        {
            _currentPawn = pawn;
        }

        private void Draw()
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }

            if (_currentPawn == null)
            {
                return;
            }
            
            //todo either we use an ordered collection or just pull out the Whole Body part first and then process the rest

            var healthModsToDraw = GetHealthModsToDraw();

            if (healthModsToDraw.Count < 1)
            {
                return;
            }
            
            //draw Whole Body first if it exists
            foreach (var healthMod in healthModsToDraw.ToArray())
            {
                if (healthMod.Key.template != null)
                {
                    continue;
                }
                
                var partUi = Instantiate(bodyPartPrefab, transform);
                partUi.Setup(healthMod.Key, healthMod.Value);

                healthModsToDraw.Remove(healthMod.Key);
            }

            foreach (var healthMod in healthModsToDraw)
            {
                var partUi = Instantiate(bodyPartPrefab, transform);
                partUi.Setup(healthMod.Key, healthMod.Value);
            }
        }

        private void DrawAll()
        {
            //todo have a toggle to show all parts
        }

        private Dictionary<BodyPart, List<HealthMod>> GetHealthModsToDraw() //todo ui probably shouldn't handle this 
        {
            //wonder if we could do a group by with linq for this
            
            var modsToDraw = new Dictionary<BodyPart, List<HealthMod>>();

            var allMods = _currentPawn.health.GetHealthMods();

            if (allMods == null)
            {
                return modsToDraw;
            }

            if (allMods.Count < 1)
            {
                return modsToDraw;
            }

            var wholeBodyPart = new BodyPart();

            foreach (var healthMod in allMods.ToArray())
            {
                if (healthMod.Part == null)
                {
                    if (modsToDraw.ContainsKey(wholeBodyPart))
                    {
                        modsToDraw[wholeBodyPart].Add(healthMod);
                    }
                    else
                    {
                        modsToDraw.Add(wholeBodyPart, new List<HealthMod>{healthMod});
                    }
                }
                else
                {
                    if (modsToDraw.ContainsKey(healthMod.Part))
                    {
                        modsToDraw[healthMod.Part].Add(healthMod);
                    }

                    if (_currentPawn.health.BodyPartIsMissing(healthMod.Part))
                    {
                        if (healthMod.Part.parent == null || _currentPawn.health.BodyPartIsMissing(healthMod.Part.parent))
                        {
                            continue;
                        }
                    }
                
                    modsToDraw.Add(healthMod.Part, new List<HealthMod>{healthMod});
                }
            }

            return modsToDraw;
        }

        private void HealthDebug_OnPawnSelected(Pawn pawn)
        {
            SetCurrentPawn(pawn);
            Draw();
        }

        private void HealthDebug_OnBodyChanged()
        {
            Draw();
        }
    }
}
