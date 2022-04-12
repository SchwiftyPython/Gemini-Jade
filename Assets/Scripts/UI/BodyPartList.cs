using Assets.Scripts.Utilities;
using Assets.Scripts.World.Pawns;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class BodyPartList : MonoBehaviour
    {
        [SerializeField] private BodyPartStatus bodyPartPrefab;

        private Pawn _currentPawn;

        private void Start()
        {
            HealthDebug.OnPawnSelected += HealthDebug_OnPawnSelected;
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

            foreach (var bodyPart in _currentPawn.GetBody())
            {
                var partUi = Instantiate(bodyPartPrefab, transform);
                partUi.Setup(bodyPart);
            }
        }

        private void HealthDebug_OnPawnSelected(Pawn pawn)
        {
            SetCurrentPawn(pawn);
            Draw();
        }
    }
}
