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

    /// <summary>
    /// Represents a region of the <see cref="LocalMap"/> on a specific <see cref="MapLayer"/> 
    /// </summary>
    public class LayerGridBucket
    {
        private readonly BucketRenderer _staticRenderer;

        private bool _visible;

        public bool rebuildMatrices;

        public BucketProperties Properties { get; }

        public Rectangle Rect { get; }

        public Tile[] Tiles { get; }

        public int Id { get; }

        public MapLayer Layer { get; }

        /// <summary>
        /// Dictionary of tile matrices indexed by id
        /// </summary>
        public Dictionary<int, List<Matrix4x4>> TileMatrices { get; protected set; }

        public Dictionary<int, Matrix4x4[]> TileMatricesArr { get; protected set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id"></param>
        /// <param name="rect"></param>
        /// <param name="layer"></param>
        /// <param name="renderer"></param>
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

        /// <summary>
        /// Sets if bucket is visible
        /// </summary>
        /// <param name="visible"></param>
        public void SetVisible(bool visible)
        {
            _visible = visible;
        }

        /// <summary>
        /// Indicates if bucket is visible
        /// </summary>
        /// <returns></returns>
        public bool IsVisible()
        {
            return _visible;
        }

        /// <summary>
        /// Draws statics
        /// </summary>
        public void DrawStatics()
        {
            _staticRenderer.Draw();
        }

        /// <summary>
        /// Calculates if bucket is visible based on what camera sees
        /// </summary>
        /// <returns></returns>
        public bool CalcVisible()
        {
            var cameraMinExtent =
                new Vector2Int(
                    Mathf.FloorToInt(Camera.main.transform.position.x -
                                     Camera.main.orthographicSize * Camera.main.aspect),
                    Mathf.FloorToInt(Camera.main.transform.position.y - Camera.main.orthographicSize));
            
            var cameraMaxExtent = new Vector2Int(
                Mathf.FloorToInt(Camera.main.transform.position.x +
                                 Camera.main.orthographicSize * Camera.main.aspect),
                Mathf.FloorToInt(Camera.main.transform.position.y + Camera.main.orthographicSize));
            
            _visible = Rect.MinExtentX >= cameraMinExtent.x - Game.BucketSize &&
                       Rect.MaxExtentX <= cameraMaxExtent.x + Game.BucketSize &&
                       Rect.MinExtentY >= cameraMinExtent.y - Game.BucketSize &&
                       Rect.MaxExtentY <= cameraMaxExtent.y + Game.BucketSize;

            return _visible;
        }

        /// <summary>
        /// Check if Matrices need an update.
        /// Updates them if necessary.
        /// </summary>
        public void CheckMatricesUpdate()
        {
            if (rebuildMatrices && IsVisible())
            {
                UpdateMatrices();

                rebuildMatrices = false;
            }
        }

        /// <summary>
        /// Updates matrices
        /// </summary>
        private void UpdateMatrices()
        {
            TileMatrices = new Dictionary<int, List<Matrix4x4>>();
            
            foreach (var tile in Tiles)
            {
                if (tile == null)
                {
                    continue;
                }

                if (!tile.HasInstancedGraphics)
                {
                    continue;
                }

                AddMatrix(tile.MainGraphic.Uid, tile.GetMatrix(tile.MainGraphic.Uid));

                if (tile.AddGraphics == null)
                {
                    continue;
                }

                foreach (var graphicInstance in tile.AddGraphics.Values)
                {
                    AddMatrix(graphicInstance.Uid, tile.GetMatrix(graphicInstance.Uid));
                }
            }

            TileMatricesArr = new Dictionary<int, Matrix4x4[]>();
            
            foreach (var kv in TileMatrices)
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

        public Coord GetLocalPosition(Coord globalPosition)
        {
            return new Coord(globalPosition.X - Rect.MinExtentX, globalPosition.Y - Rect.MinExtentY);
        }

        public Tile GetTileAt(Coord position)
        {
            var localPosition = GetLocalPosition(position);
            
            if (localPosition.X < 0)
            {
                return null;
            }

            if (localPosition.Y < 0)
            {
                return null;
            }

            if (localPosition.X >= Rect.Width)
            {
                return null;
            }

            return localPosition.Y < Rect.Height ? Tiles[localPosition.X + localPosition.Y * Rect.Width] : null;
        }

        public void AddTile(Tile tile)
        {
            var localPosition = GetLocalPosition(tile.Position);
            
            var tileIndex = localPosition.X + localPosition.Y * Rect.Width;

            Tiles[tileIndex] = tile;

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
            
            var localPosition = GetLocalPosition(tile.Position);
            
            var tileIndex = localPosition.X + localPosition.Y * Rect.Width;

            Tiles[tileIndex] = null;
            
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