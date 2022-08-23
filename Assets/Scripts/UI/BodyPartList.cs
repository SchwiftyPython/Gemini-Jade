using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Utilities;
using Assets.Scripts.World.Pawns;
using UnityEngine;
using World.Pawns;
using World.Pawns.Health.HealthModifiers;

namespace UI
{
    /// <summary>
    /// The body part list class
    /// </summary>
    /// <seealso cref="MonoBehaviour"/>
    public class BodyPartList : MonoBehaviour
    {
        /// <summary>
        /// The body part prefab
        /// </summary>
        [SerializeField] private BodyPartStatus bodyPartPrefab;

        /// <summary>
        /// The current pawn
        /// </summary>
        private Pawn _currentPawn;

        /// <summary>
        /// Starts this instance
        /// </summary>
        private void Start()
        {
            HealthDebug.OnPawnSelected += HealthDebug_OnPawnSelected;
            HealthDebug.OnBodyChanged += HealthDebug_OnBodyChanged;
        }

        /// <summary>
        /// Sets the current pawn using the specified pawn
        /// </summary>
        /// <param name="pawn">The pawn</param>
        private void SetCurrentPawn(Pawn pawn)
        {
            _currentPawn = pawn;
        }

        /// <summary>
        /// Draws this instance
        /// </summary>
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

        /// <summary>
        /// Draws the all
        /// </summary>
        private void DrawAll()
        {
            //todo have a toggle to show all parts
        }

        /// <summary>
        /// Gets the health mods to draw
        /// </summary>
        /// <returns>The mods to draw</returns>
        private Dictionary<BodyPart, List<HealthMod>> GetHealthModsToDraw() //todo ui probably shouldn't handle this 
        {
            //wonder if we could do a group by with linq for this
            
            var modsToDraw = new Dictionary<BodyPart, List<HealthMod>>();

            var allMods = _currentPawn.health.GetHealthMods();

            if (allMods == null)
            {
                Debug.Log("All health mods null.");
                
                return modsToDraw;
            }

            if (allMods.Count < 1)
            {
                Debug.Log("All health mods is empty.");
                
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

        /// <summary>
        /// Healths the debug on pawn selected using the specified pawn
        /// </summary>
        /// <param name="pawn">The pawn</param>
        private void HealthDebug_OnPawnSelected(Pawn pawn)
        {
            SetCurrentPawn(pawn);
            Draw();
        }

        /// <summary>
        /// Healths the debug on body changed
        /// </summary>
        private void HealthDebug_OnBodyChanged()
        {
            Draw();
        }
    }
}
