using System.Collections.Generic;
using Assets.Scripts.Utilities;
using Assets.Scripts.World.Pawns;
using UnityEngine;
using World.Pawns;

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

            var partsToDraw = GetPartsToDraw();

            foreach (var bodyPart in partsToDraw)
            {
                if (!bodyPart.HasHealthMods())
                {
                    continue;
                }

                var partUi = Instantiate(bodyPartPrefab, transform);
                partUi.Setup(bodyPart);
            }
        }

        private void DrawAll()
        {
            //todo have a toggle to show all parts
        }

        private List<BodyPart> GetPartsToDraw()
        {
            var partsToDraw = new List<BodyPart>();

            foreach (var bodyPart in _currentPawn.GetBody())
            {
                if (!bodyPart.HasHealthMods() || partsToDraw.Contains(bodyPart))
                {
                    continue;
                }

                if (bodyPart.IsMissing())
                {
                    if (bodyPart.parent == null || bodyPart.parent.IsMissing())
                    {
                        //todo parent being null means this is core part and they are real dead

                        continue;
                    }
                }

                partsToDraw.Add(bodyPart);
            }

            return partsToDraw;
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
