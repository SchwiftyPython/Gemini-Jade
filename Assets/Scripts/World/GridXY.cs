using System;
using UnityEngine;

namespace World
{
    public class GridXY<TGridObject>
    {
        public event EventHandler<OnGridObjectChangedEventArgs> OnGridObjectChanged;
        
        public class OnGridObjectChangedEventArgs : EventArgs 
        {
            public int x;
            public int y;
        }

        private readonly int _width;
        
        private readonly int _height;
        
        private readonly float _cellSize;
        
        private readonly Vector3 _originPosition;
        
        private readonly TGridObject[,] _gridArray;
        
        
        public GridXY(int width, int height, float cellSize, Vector3 originPosition, Func<GridXY<TGridObject>, int, int, TGridObject> createGridObject)
        {
            _width = width;
            
            _height = height;
            
            _cellSize = cellSize;
            
            _originPosition = originPosition;
            
            _gridArray = new TGridObject[_width, _height];
            
            for (var x = 0; x < _gridArray.GetLength(0); x++) 
            {
                for (var y = 0; y < _gridArray.GetLength(1); y++) 
                {
                    _gridArray[x, y] = createGridObject(this, x, y);
                }
            }
        }
        
        public int GetWidth() 
        {
            return _width;
        }

        public int GetHeight() 
        {
            return _height;
        }

        public float GetCellSize() 
        {
            return _cellSize;
        }

        public Vector3 GetWorldPosition(int x, int y) 
        {
            return new Vector3(x, y, 0) * _cellSize + _originPosition;
        }

        public void GetXY(Vector3 worldPosition, out int x, out int y)
        {
            x = Mathf.FloorToInt((worldPosition - _originPosition).x / _cellSize);
            
            y = Mathf.FloorToInt((worldPosition - _originPosition).y / _cellSize);
        }

        public void SetGridObject(int x, int y, TGridObject value)
        {
            if (x < 0)
            {
                return;
            }

            if (y < 0)
            {
                return;
            }

            if (x >= _width)
            {
                return;
            }

            if (y >= _height)
            {
                return;
            }

            _gridArray[x, y] = value;
                
            TriggerGridObjectChanged(x, y);
        }

        public void TriggerGridObjectChanged(int x, int y)
        {
            OnGridObjectChanged?.Invoke(this, new OnGridObjectChangedEventArgs {x = x, y = y});
        }

        public void SetGridObject(Vector3 worldPosition, TGridObject value)
        {
            GetXY(worldPosition, out var x, out var y);
            
            SetGridObject(x, y, value);
        }

        public TGridObject GetGridObject(int x, int y)
        {
            if (x < 0)
            {
                return default;
            }

            if (y < 0)
            {
                return default;
            }

            if (x >= _width)
            {
                return default;
            }

            return y < _height ? _gridArray[x, y] : default;
        }
        
        public TGridObject GetGridObject(Vector3 worldPosition) 
        {
            GetXY(worldPosition, out var x, out var y);
            
            return GetGridObject(x, y);
        }

        public Vector2Int ValidateGridPosition(Vector2Int gridPosition) 
        {
            return new Vector2Int
            (
                Mathf.Clamp(gridPosition.x, 0, _width - 1),
                Mathf.Clamp(gridPosition.y, 0, _height - 1)
            );
        }

        public bool IsValidGridPosition(Vector2Int gridPosition) 
        {
            var x = gridPosition.x;
            
            var y = gridPosition.y;

            return x >= 0 && y >= 0 && x < _width && y < _height;
        }

        public bool IsValidGridPositionWithPadding(Vector2Int gridPosition) 
        {
            var padding = new Vector2Int(2, 2);
            
            var x = gridPosition.x;
            
            var y = gridPosition.y;

            return x >= padding.x && y >= padding.y && x < _width - padding.x && y < _height - padding.y;
        }
    }
}
