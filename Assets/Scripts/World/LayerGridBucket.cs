using System;
using System.Collections.Generic;
using GoRogue;
using Graphics;
using Settings;
using UnityEngine;
using World.Things;

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

        public BaseObject[] BaseObjects { get; }

        public int Id { get; }

        public MapLayer Layer { get; }

        /// <summary>
        /// Dictionary of tile matrices indexed by id
        /// </summary>
        public Dictionary<int, List<Matrix4x4>> Matrices { get; protected set; }

        public Dictionary<int, Matrix4x4[]> MatricesArr { get; protected set; }

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

            BaseObjects = new BaseObject[Rect.Width * Rect.Height];

            Matrices = new Dictionary<int, List<Matrix4x4>>();

            MatricesArr = new Dictionary<int, Matrix4x4[]>();

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
            
            _visible = Rect.MinExtentX >= cameraMinExtent.x - Constants.BucketSize &&
                       Rect.MaxExtentX <= cameraMaxExtent.x + Constants.BucketSize &&
                       Rect.MinExtentY >= cameraMinExtent.y - Constants.BucketSize &&
                       Rect.MaxExtentY <= cameraMaxExtent.y + Constants.BucketSize;

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
            Matrices = new Dictionary<int, List<Matrix4x4>>();

            foreach (var tile in BaseObjects)
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

            MatricesArr = new Dictionary<int, Matrix4x4[]>();
            
            foreach (var kv in Matrices)
            {
                MatricesArr.Add(kv.Key, kv.Value.ToArray());
            }
        }

        public void DrawInstancedMeshes()
        {
            foreach (var tileMatrix in MatricesArr)
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

        public BaseObject GetBaseObjectAt(Coord position)
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

            return localPosition.Y < Rect.Height ? BaseObjects[localPosition.X + localPosition.Y * Rect.Width] : null;
        }

        public void AddBaseObject(BaseObject baseObject)
        {
            var localPosition = GetLocalPosition(baseObject.Position);
            
            var tileIndex = localPosition.X + localPosition.Y * Rect.Width;

            BaseObjects[tileIndex] = baseObject;

            baseObject.SetBucket(this);
            
            //todo onBucketChanged event
            
            //todo if tile has resources, add those resources to this bucket

            if (baseObject.HasInstancedGraphics)
            {
                AddMatrix(baseObject.MainGraphic.Uid, baseObject.GetMatrix(baseObject.MainGraphic.Uid));

                if (baseObject.AddGraphics != null)
                {
                    foreach (var instance in baseObject.AddGraphics.Values)
                    {
                        AddMatrix(instance.Uid, baseObject.GetMatrix(instance.Uid));
                    }
                }

                rebuildMatrices = true;
            }
        }

        public void AddTile(Tile tile)
        {
            var localPosition = GetLocalPosition(tile.Position);
            
            var tileIndex = localPosition.X + localPosition.Y * Rect.Width;

            BaseObjects[tileIndex] = tile;

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

            BaseObjects[tileIndex] = null;
            
            //todo onBucketChanged event

            if (tile.HasInstancedGraphics)
            {
                rebuildMatrices = true;
            }
        }

        public void AddThing(Thing thing)
        {
            var localPosition = GetLocalPosition(thing.Position);
            
            var tileIndex = localPosition.X + localPosition.Y * Rect.Width;

            BaseObjects[tileIndex] = thing;

            thing.SetBucket(this);
            
            //todo onBucketChanged event
            
            //todo if tile has resources, add those resources to this bucket

            if (thing.HasInstancedGraphics)
            {
                AddMatrix(thing.MainGraphic.Uid, thing.GetMatrix(thing.MainGraphic.Uid));

                if (thing.AddGraphics != null)
                {
                    foreach (var instance in thing.AddGraphics.Values)
                    {
                        AddMatrix(instance.Uid, thing.GetMatrix(instance.Uid));
                    }
                }

                rebuildMatrices = true;
            }
        }
        
        public void AddMatrix(int graphicID, Matrix4x4 matrix) 
        {
            if (!Matrices.ContainsKey(graphicID)) 
            {
                Matrices.Add(graphicID, new List<Matrix4x4>());
            }
            
            Matrices[graphicID].Add(matrix);
        }
    }
}