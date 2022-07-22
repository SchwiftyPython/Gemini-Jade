using UnityEngine;
using UnityEngine.UI;

namespace UI.Grid
{
    [RequireComponent(typeof(Button))]
    public class SelectGridObjectButton : MonoBehaviour
    {
        public static event System.Action<GameObject> OnObjectSelected;
        
        //[SerializeField] private GridObject gridObjectToSpawnPrefab;

        [SerializeField] private Sprite gridObjectTexture;

        private void Start()
        {
            var button = GetComponent<Button>();

            if (button != null)
            {
                button.onClick.AddListener(HandleButtonClicked);
            }
            
            // if (gridObjectToSpawnPrefab == null)
            // {
            //     Debug.LogError("Error. No prefab assigned to spawn on this selection option");
            //     return;
            // }
            //
            // gridObjectToSpawnPrefab.SetSpriteTexture(gridObjectTexture);
        }
        
        private void HandleButtonClicked()
        {
            //OnObjectSelected?.Invoke(gridObjectToSpawnPrefab.gameObject);
        }
    }
}
