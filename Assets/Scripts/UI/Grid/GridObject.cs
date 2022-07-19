using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.Grid
{
    public class GridObject : MonoBehaviour
    {
        public delegate void OnObjectSelectedEvent(GameObject gameObject);
    
        public static event OnObjectSelectedEvent OnObjectSelected;

        public delegate void OnConfirmPlacementEvent();
    
        public static event OnConfirmPlacementEvent OnConfirmPlacement;

        public void OnObjectInstantiated()
        {
            OnObjectSelected?.Invoke(gameObject);
        }
    
        public void OnPointerDown(PointerEventData eventData)
        {
            OnConfirmPlacement?.Invoke();
        }
    }
}
