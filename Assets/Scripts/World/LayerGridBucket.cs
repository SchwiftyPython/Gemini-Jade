using System;
using System.Collections.Generic;
using GoRogue;
using Graphics;
using UnityEngine;

namespace World
{
    public class BucketProperties
    {
        //todo not sure of purpose yet.
        //Could be used to seek out areas with high quantities of a resource?
        
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
            _visible = Rect.MinExtentX >= Camera.main.rect.min.x - Game.BucketSize &&
                       Rect.MaxExtentX <= Camera.main.rect.max.x + Game.BucketSize &&
                       Rect.MinExtentY >= Camera.main.rect.min.y - Game.BucketSize &&
                       Rect.MaxExtentY <= Camera.main.rect.max.y + Game.BucketSize;

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
                if (tile != null && tile.HasInstancedGraphics)
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

        public void DrawInstancedMeshes()
        {
            foreach (var tileMatrix in TileMatricesArr)
            {
                UnityEngine.Graphics.DrawMeshInstanced(GraphicInstance.instances[tileMatrix.Key].Mesh, 0,
                    GraphicInstance.instances[tileMatrix.Key].Material, tileMatrix.Value);
            }
        }

        public void BuildStaticMeshes()
        {
            _staticRenderer.BuildMeshes();
        }

        public Tile GetTileAt(Coord position)
        {
            if (position.X < 0)
            {
                return null;
            }

            if (position.Y < 0)
            {
                return null;
            }

            if (position.X >= Rect.Width)
            {
                return null;
            }

            return position.Y < Rect.Height ? Tiles[position.X + position.Y * Rect.Width] : null;
        }

        public void AddTile(Tile tile)
        {
            Tiles[tile.Position.X + tile.Position.Y * Rect.Width] = tile;

            tile.SetBucket(this);
            
            //todo onBucketChanged event
            
            //todo if tile has resources, add those resources to this bucket

            if (tile.HasInstancedGraphics)
            {
                AddMatrix(tile.MainGraphic.Uid, tile.GetMatrix(tile.MainGraphic.Uid));

                if (tile.AddGraphics != null)
                {
                    foreach (var instance in tile.AddGraphics.Values)
                    {
                        AddMatrix(instance.Uid, tile.GetMatrix(instance.Uid));
                    }
                }

                rebuildMatrices = true;
            }
        }

        public void DeleteTile(Tile tile)
        {
            //todo if tile has resources, subtract those resources from this bucket

            Tiles[tile.Position.X + tile.Position.Y * Rect.Width] = null;
            
            //todo onBucketChanged event

            if (tile.HasInstancedGraphics)
            {
                rebuildMatrices = true;
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