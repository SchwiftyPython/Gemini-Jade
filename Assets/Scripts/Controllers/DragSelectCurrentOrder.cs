using System;
using System.Collections.Generic;
using System.Linq;
using GoRogue;
using Repos;
using UI;
using UI.Orders;
using UnityEngine;
using UnityEngine.EventSystems;
using Utilities;
using World;
using World.Pawns.Jobs;
using World.Things.Plants;

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

        public Action<OrderTemplate> onOrderSelected;

        public Action onOrderDeselected;

        private List<GameObject> _highlightedTiles;

        private SkillRepo _skillRepo;

        private GraphicsRepo _graphicsRepo;

        private void Start()
        {
            isDragging = false;

            _skillRepo = FindObjectOfType<SkillRepo>();

            _graphicsRepo = FindObjectOfType<GraphicsRepo>();

            _currentMap = FindObjectOfType<Game>().CurrentLocalMap;
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

            HighlightValidTilesInSelectionArea();
        }

        private void Reset()
        {
            origin = Vector2.zero;

            isDragging = false;

            ClearHighlights();
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

            var topLeft = Vector2.Min(new Vector2(rectOrigin.x, Screen.height - rectOrigin.y),
                new Vector2(mousePosition.x, Screen.height - mousePosition.y));

            var bottomRight = Vector2.Max(new Vector2(rectOrigin.x, Screen.height - rectOrigin.y),
                new Vector2(mousePosition.x, Screen.height - mousePosition.y));

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

            currentMapSelection = new Rectangle(new Coord((int) (start.x + 1), (int) (start.y + 1)),
                new Coord((int) (end.x + 1), (int) (end.y + 1)));
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

            onOrderSelected?.Invoke(_currentOrder);
        }

        public void OnOrderDeselected()
        {
            _currentOrder = null;

            onOrderDeselected?.Invoke();
        }

        private void HighlightValidTilesInSelectionArea()
        {
            if (_currentOrder == null)
            {
                return;
            }

            if (!isDragging)
            {
                return;
            }

            ClearHighlights();

            _highlightedTiles = new List<GameObject>();

            if (_currentOrder.skillNeeded == _skillRepo.harvest)
            {
                var plants = GetAllHarvestablePlantsInSelection();

                foreach (var plant in plants)
                {
                    var parentObject = new GameObject($"{plant.id} Highlight Parent");

                    parentObject.transform.position = plant.Position.ToVector3();

                    UnityUtils.CreateWorldSprite(parentObject.transform, $"{plant.id} Highlight",
                        _graphicsRepo.whiteSquare, Vector3.zero, Vector3.one, 0, new Color(1, 0.95f, 0.75f, 0.125f));

                    _highlightedTiles.Add(parentObject);
                }
            }
        }

        private void ClearHighlights()
        {
            if (_highlightedTiles == null)
            {
                return;
            }

            foreach (var highlight in _highlightedTiles.ToArray())
            {
                Destroy(highlight);
            }

            _highlightedTiles = null;
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

            //todo might be able to have a Dictionary <Skill, Action or delegate> but could be over engineering at this point

            if (_currentOrder.skillNeeded == _skillRepo.harvest)
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
            var plants = GetAllHarvestablePlantsInSelection();

            foreach (var plant in plants)
            {
                var job = new Job(plant.Position, plant.SkillNeeded, plant.MinSkillToHarvest);

                _currentMap.onJobAdded?.Invoke(job);

                plant.UpdateOrderGraphics(_currentOrder.graphics);
            }
        }

        private List<Plant> GetAllHarvestablePlantsInSelection()
        {
            return currentMapSelection.Positions().Select(position => _currentMap.GetPlantAt(position))
                .Where(plant => plant is {CanBeHarvested: true}).ToList();
        }
    }
}