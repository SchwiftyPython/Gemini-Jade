using GoRogue;
using UI;
using UI.Orders;
using UnityEngine;
using UnityEngine.EventSystems;
using Utilities;
using World;
using World.Pawns.Jobs;

namespace Controllers
{
    public class DragSelectCurrentOrder : MonoBehaviour 
    {
        public bool isDragging;

        public Vector2 origin;

        public Rect currentSelection;

        public Rectangle currentMapSelection;

        private OrderTemplate _currentOrder;

        private LocalMap _currentMap;

        private void Start()
        {
            // _currentOrder = ScriptableObject.CreateInstance<OrderTemplate>();
            //
            // _currentOrder.selectionType = Selection.Area;
            
            isDragging = false;
        }

        private void Update()
        {
            if (_currentOrder != null)
            {
                if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1))
                {
                    OnOrderDeselected();
                }
            }

            BeginSelection();

            UpdateCurrentSelection(origin);
        }

        private void Reset()
        {
            origin = Vector2.zero;
            isDragging = false;
        }

        private void OnGUI()
        {
            UpdateDrawRect();
        }

        public void UpdateCurrentSelection(Vector2 rectOrigin)
        {
            if (!isDragging)
            {
                return;
            }
            
            var mousePosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            
            rectOrigin.y = Screen.height - rectOrigin.y;

            mousePosition.y = Screen.height - mousePosition.y;

            var topLeft = Vector2.Min(rectOrigin, mousePosition);

            var bottomRight = Vector2.Max(rectOrigin, mousePosition);

            currentSelection = Rect.MinMaxRect(topLeft.x, topLeft.y, bottomRight.x, bottomRight.y);
            
            UpdateMapSelection(rectOrigin, mousePosition);

            if (Input.GetMouseButtonUp(0) && _currentOrder.selectionType != Selection.Single &&
                !EventSystem.current.IsPointerOverGameObject())
            {
                AddJobs();
            }
        }

        public void UpdateMapSelection(Vector2 rectOrigin, Vector2 mousePosition)
        {
            var start = Camera.main.ScreenToWorldPoint(rectOrigin);

            var end = Camera.main.ScreenToWorldPoint(mousePosition);

            if (end.x < start.x)
            {
                (end.x, start.x) = (start.x, end.x);
            }

            if (end.y < start.y)
            {
                (end.y, start.y) = (start.y, end.y);
            }

            currentMapSelection = new Rectangle(new Coord(Mathf.FloorToInt(start.x), Mathf.FloorToInt(start.y)),
                new Coord(Mathf.FloorToInt(end.x), Mathf.FloorToInt(end.y)));
        }

        public void DrawScreenRect(Rect rect, Color color)
        {
            GUI.DrawTexture(rect, UnityUtils.GetColoredTexture(color));
        }

        public void DrawScreenRectBorder(Rect rect, Color color, float thickness)
        {
            DrawScreenRect(new Rect(rect.xMin, rect.yMin, rect.width, thickness), color);
            
            DrawScreenRect(new Rect(rect.xMin, rect.yMin, thickness, rect.height), color);
            
            DrawScreenRect(new Rect(rect.xMax - thickness, rect.yMin, thickness, rect.height), color);
            
            DrawScreenRect(new Rect(rect.xMin, rect.yMax - thickness, rect.width, thickness), color);
        }

        public void OnNewOrderSelected(OrderTemplate order)
        {
            if (order == null || order.selectionType != Selection.Area)
            {
                return;
            }
            
            _currentOrder = order;
        }

        public void OnOrderDeselected()
        {
            _currentOrder = null;
        }

        private void BeginSelection()
        {
            if (_currentOrder == null)
            {
                return;
            }

            if (!isDragging && Input.GetMouseButtonDown(0) && _currentOrder.selectionType != Selection.Single &&
                !EventSystem.current.IsPointerOverGameObject())
            {
                origin = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

                isDragging = true;
            }
        }
        
        private void AddJobs()
        {
            if (_currentOrder == null)
            {
                return;
            }

            if (_currentMap == null)
            {
                _currentMap = FindObjectOfType<Game>().CurrentLocalMap;
            }

            foreach (var position in currentMapSelection.Positions())
            {
                var plant = _currentMap.GetPlantAt(position);

                if (plant == null || !plant.CanBeHarvested)
                {
                    continue;
                }

                var job = new Job(position, plant.SkillNeeded, plant.MinSkillToHarvest);
                
                _currentMap.onJobAdded?.Invoke(job);
            }
            
            Reset();
        }
        
        private void UpdateDrawRect()
        {
            if (_currentOrder == null || !isDragging)
            {
                return;
            }

            if (_currentOrder.selectionType == Selection.Area)
            {
                DrawScreenRectBorder(currentSelection, new Color(1f, 1f, 1f, .5f), 3f);
            }
        }
    }
}
