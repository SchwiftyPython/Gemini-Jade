using System;
using GoRogue;
using Settings;
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
            
            //rectOrigin.y = Screen.height - rectOrigin.y;

            //mousePosition.y = Screen.height - mousePosition.y;

            var topLeft = Vector2.Min(new Vector2(rectOrigin.x, Screen.height - rectOrigin.y), new Vector2(mousePosition.x, Screen.height - mousePosition.y));

            var bottomRight = Vector2.Max(new Vector2(rectOrigin.x, Screen.height - rectOrigin.y), new Vector2(mousePosition.x, Screen.height - mousePosition.y));

            currentSelection = Rect.MinMaxRect(topLeft.x, topLeft.y, bottomRight.x, bottomRight.y);
            
            UpdateMapSelection(rectOrigin, mousePosition);
            
            //todo highlight valid tiles
            

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

            //currentMapSelection = new Rectangle(new Coord(Mathf.FloorToInt(start.x + Constants.MeshOffset.Item1), Mathf.FloorToInt(start.y + Constants.MeshOffset.Item2)),
                //new Coord(Mathf.FloorToInt(end.x + + Constants.MeshOffset.Item1), Mathf.FloorToInt(end.y + + Constants.MeshOffset.Item2)));
            
            Debug.Log($"Rect start {start} Rect end {end}");
            
            currentMapSelection = new Rectangle(new Coord((int)(start.x + 1), (int)(start.y + 1)),
                new Coord((int)(end.x + 1), (int)(end.y + 1)));
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

        private void HighlightValidTiles()
        {
            //todo
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

            //todo might be able to have a Dictionary <Skill, Action or delegate> but could be over engineering at this point
            if (_currentOrder.skillNeeded.templateName.Equals("harvest", StringComparison.OrdinalIgnoreCase))
            {
                HarvestSelectedPlants();
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

        private void HarvestSelectedPlants()
        {
            foreach (var position in currentMapSelection.Positions())
            {
                var plant = _currentMap.GetPlantAt(position);

                if (plant == null || !plant.CanBeHarvested)
                {
                    continue;
                }
                
                Debug.Log($"Adding job to {plant.ID} at position {position}");

                var job = new Job(position, plant.SkillNeeded, plant.MinSkillToHarvest);
                
                _currentMap.onJobAdded?.Invoke(job);
                
                plant.UpdateOrderGraphics(_currentOrder.graphics);
            }
        }
    }
}
