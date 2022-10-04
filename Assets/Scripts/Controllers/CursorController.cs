using UI.Orders;
using UnityEngine;

namespace Controllers
{
    public class CursorController : MonoBehaviour
    {
        private DragSelectCurrentOrder _dragSelect;
        
        public Texture2D defaultCursor;
    
        private void Start()
        {
            SetCursorTexture(defaultCursor);

            _dragSelect = FindObjectOfType<DragSelectCurrentOrder>();

            _dragSelect.onOrderSelected += OnOrderSelected;
        }

        private void OnOrderSelected(OrderTemplate orderTemplate)
        {
            _dragSelect.onOrderDeselected += OnOrderDeselected;
            
            SetCursorTexture(orderTemplate.graphics.texture);
        }

        private void OnOrderDeselected()
        {
            _dragSelect.onOrderDeselected -= OnOrderDeselected;
            
            SetCursorTexture(defaultCursor);
        }

        public void SetCursorTexture(Texture2D texture)
        {
            //todo we might need different offsets for different cursors
        
            Cursor.SetCursor(texture, new Vector2(10, 10), CursorMode.Auto);
        }
    }
}
