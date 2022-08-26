using System;
using System.Collections.Generic;
using GoRogue;
using Graphics;
using UnityEngine;

namespace World
{
    public class BucketProperties
    {
        public float foodQuantity;
        public float fertility;
        public int woodQuantity;

        public BucketProperties()
        {
            foodQuantity = 0f;
            fertility = 0f;
            woodQuantity = 0;
        }
    }

    public class LayerGridBucket
    {
        public const int BucketSize = 32;

        private BucketRenderer _staticRenderer;

        private bool _visible;

        public bool rebuildMatrices;

        public BucketProperties Properties { get; protected set; }

        public Rectangle Rect { get; protected set; }

        public Tile[] Tiles { get; protected set; }

        public int Id { get; protected set; }

        public MapLayer Layer { get; protected set; }

        /// <summary>
        /// Dictionary of tile matrices indexed by id
        /// </summary>
        public Dictionary<int, List<Matrix4x4>> TileMatrices { get; protected set; }

        public Dictionary<int, Matrix4x4[]> TileMatricesArr { get; protected set; }

        public LayerGridBucket(int id, Rectangle rect, MapLayer layer, Type renderer)
        {
            Id = id;
            Rect = rect;
            Layer = layer;

            Tiles = new Tile[Rect.Width * Rect.Height];

            TileMatrices = new Dictionary<int, List<Matrix4x4>>();

            TileMatricesArr = new Dictionary<int, Matrix4x4[]>();

            Properties = new BucketProperties();

            if (renderer != null)
            {
                _staticRenderer = (BucketRenderer) Activator.CreateInstance(renderer, this, layer);
            }
        }

        public void SetVisible(bool visible)
        {
            _visible = visible;
        }

        public bool IsVisible()
        {
            return _visible;
        }

        public void DrawStatics()
        {
            _staticRenderer.Draw();
        }

        public bool CalcVisible()
        {
            _visible = Rect.MinExtentX >= Camera.main.rect.min.x - BucketSize &&
                       Rect.MaxExtentX <= Camera.main.rect.max.x + BucketSize &&
                       Rect.MinExtentY >= Camera.main.rect.min.y - BucketSize &&
                       Rect.MaxExtentY <= Camera.main.rect.max.y + BucketSize;

            return _visible;
        }

        public void CheckMatricesUpdate()
        {
            if (rebuildMatrices && IsVisible())
            {
                UpdateMatrices();

                rebuildMatrices = false;
            }
        }

        public void UpdateMatrices()
        {
            TileMatrices = new Dictionary<int, List<Matrix4x4>>();
            
            foreach (Tile tile in Tiles)
            {
                if (tile != null) //todo  && tile.isInstanced
                {
                    AddMatrix(tile.MainGraphic.Uid, tile.GetMatrix(tile.MainGraphic.Uid));
                    
                    if (tile.AddGraphics != null)
                    {
                        foreach (GraphicInstance graphicInstance in tile.AddGraphics.Values)
                        {
                            AddMatrix(graphicInstance.Uid, tile.GetMatrix(graphicInstance.Uid));
                        }
                    }
                }
            }

            TileMatricesArr = new Dictionary<int, Matrix4x4[]>();
            foreach (KeyValuePair<int, List<Matrix4x4>> kv in TileMatrices)
            {
                TileMatricesArr.Add(kv.Key, kv.Value.ToArray());
            }
        }
        
        public void AddMatrix(int graphicID, Matrix4x4 matrix) 
        {
            if (!TileMatrices.ContainsKey(graphicID)) 
            {
                TileMatrices.Add(graphicID, new List<Matrix4x4>());
            }
            
            TileMatrices[graphicID].Add(matrix);
        }
    }
}