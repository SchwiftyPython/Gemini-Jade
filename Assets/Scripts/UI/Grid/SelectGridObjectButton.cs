using UnityEngine;
using UnityEngine.UI;
using World.Things.CraftableThings;

namespace UI.Grid
{
    /// <summary>
    /// The select grid object button class
    /// </summary>
    /// <seealso cref="MonoBehaviour"/>
    [RequireComponent(typeof(Button))]
    public class SelectGridObjectButton : MonoBehaviour
    {
        public static event System.Action<PlacedObjectTemplate> OnObjectSelected;
        
        /// <summary>
        /// The placed object type
        /// </summary>
        [SerializeField] private PlacedObjectTemplate placedObjectType;

        /// <summary>
        /// Starts this instance
        /// </summary>
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
        
        /// <summary>
        /// Handles the button clicked
        /// </summary>
        private void HandleButtonClicked()
        {
            OnObjectSelected?.Invoke(placedObjectType);
        }
    }
}
