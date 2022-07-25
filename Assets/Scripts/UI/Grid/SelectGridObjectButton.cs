using UnityEngine;
using UnityEngine.UI;
using World.PlacedObjectTypes;

namespace UI.Grid
{
    [RequireComponent(typeof(Button))]
    public class SelectGridObjectButton : MonoBehaviour
    {
        public static event System.Action<PlacedObjectType> OnObjectSelected;
        
        [SerializeField] private PlacedObjectType placedObjectType;

        private void Start()
        {
            var button = GetComponent<Button>();

            if (button != null)
            {
                button.onClick.AddListener(HandleButtonClicked);
            }
            
            if (placedObjectType == null)
            {
                Debug.LogError("Error. No placed object type assigned to spawn on this selection option");
                return;
            }
        }
        
        private void HandleButtonClicked()
        {
            OnObjectSelected?.Invoke(placedObjectType);
        }
    }
}
