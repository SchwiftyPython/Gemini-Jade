using System.Collections;
using Hypertonic.GridPlacement;
using UnityEngine;

namespace UI.Grid
{
    public class GridController : MonoBehaviour
    {
        public delegate void GridControllerEvent();

        public static event GridControllerEvent OnObjectPlaced;

        // [SerializeField]
        // private GameObject cancelPlacementButton; todo I'd rather use esc and right click to cancel placement

        [SerializeField] private GameObject deleteObjectButton;

        private void Start()
        {
            //This is for moving objects around willy nilly. Will only be applicable to blueprints, not constructed objects.
            //GridObject.OnObjectSelected += OnGridObjectSelected; 

            SelectGridObjectButton.OnObjectSelected += OnGridObjectSelected;
            
            StartCoroutine(CheckForInput());
        }

        private void HandleConfirmPlacement()
        {
            var placed = GridManagerAccessor.GridManager.ConfirmPlacement();

            if (placed)
            {
                OnObjectPlaced?.Invoke();

                GridObject.OnConfirmPlacement -= HandleConfirmPlacement;
            }
        }

        private void OnGridObjectSelected(GameObject gridObject)
        {
            GridManagerAccessor.GridManager.StartPaintMode(gridObject);
        }

        private IEnumerator CheckForInput()
        {
            while (true)
            {
                yield return new WaitForEndOfFrame();

                if (!GridManagerAccessor.GridManager.IsPlacingGridObject)
                {
                    continue;
                }

                if (UnityEngine.InputSystem.Mouse.current.leftButton.isPressed)
                {
                    GridManagerAccessor.GridManager.ConfirmPlacement();
                }
            }
        }
    }
}
