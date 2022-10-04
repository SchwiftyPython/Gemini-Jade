using Controllers;
using UnityEngine;

namespace UI.Orders
{
    public class OrderButton : MonoBehaviour
    {
        private DragSelectCurrentOrder _dragSelector;

        public OrderTemplate orderTemplate;

        private void Start()
        {
            _dragSelector = FindObjectOfType<DragSelectCurrentOrder>();
        }

        public void OnClick()
        {
            if (orderTemplate.selectionType == Selection.Area)
            {
                _dragSelector.OnNewOrderSelected(orderTemplate);
            }
        }
    }
}
