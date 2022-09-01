using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GoRogue;
using Graphics;
using Utilities;

namespace World
{
    public class LayerGrid
    {
        private int _numBucketsX;

        private int _numBucketsY;
        
        private int _bucketCount;
        
        public Rectangle Rect { get; }
    
        public LayerGridBucket[] Buckets { get; protected set; }
    
        public Type RendererType { get; protected set; }
    
        public MapLayer Layer { get; }

        public Coord Size => Rect.Size;

        public LayerGrid(Coord size, MapLayer layer)
        {
            Layer = layer;

            Rect = new Rectangle(0, 0, size.X, size.Y);

            RendererType = typeof(BucketRenderer);

            _numBucketsX = Mathf.CeilToInt(Size.X / (float)Game.BucketSize);
            
            _numBucketsY = Mathf.CeilToInt(Size.Y / (float)Game.BucketSize);

            _bucketCount = _numBucketsX * _numBucketsY;
        }

        public void AddTile(Tile tile)
        {
            var bucket = GetBucketAt(tile.Position);

            bucket?.AddTile(tile);
        }

        public List<Tile> GetTiles()
        {
            return (from bucket in Buckets from tile in bucket.Tiles where tile != null select tile).ToList();
        }

        public Tile GetTileAt(Coord position)
        {
            var bucket = GetBucketAt(position);

            if (bucket == null)
            {
                return null;
            }

            return bucket.GetTileAt(position);
        }

        public LayerGridBucket GetBucketAt(Coord position)
        {
            var bucketIndex = position.X / Game.BucketSize + position.Y / Game.BucketSize * _numBucketsX;

            if (bucketIndex >= 0 && bucketIndex < Buckets.Length)
            {
                return Buckets[bucketIndex];
            }

            Debug.LogError($"Tried to get bucket at out of bounds location: {position} Bucket Index: {bucketIndex}");

            return null;
        }

        public void BuildStaticMeshes()
        {
            if (RendererType == null)
            {
                return;
            }

            foreach (var bucket in Buckets)
            {
                bucket.BuildStaticMeshes();
            }
        }

        public void CheckMatricesUpdates()
        {
            foreach (var bucket in Buckets)
            {
                if (bucket.IsVisible())
                {
                    bucket.CheckMatricesUpdate();
                }
            }
        }

        public void DrawBuckets()
        {
            foreach (var bucket in Buckets)
            {
                if (bucket.IsVisible())
                {
                    if (RendererType != null)
                    {
                        bucket.DrawStatics();
                    }

                    bucket.DrawInstancedMeshes();
                }
            }
        }

        protected void GenerateBuckets()
        {
            Buckets = new LayerGridBucket[_bucketCount];

            for (var x = 0; x < Size.X; x += Game.BucketSize)
            {
                for (var y = 0; y < Size.Y; y += Game.BucketSize) 
                {
                    var bucketRect = new Rectangle(x, y, Game.BucketSize, Game.BucketSize);

                    bucketRect = bucketRect.Clip(Rect);

                    var bucketId = x / Game.BucketSize + y / Game.BucketSize * _numBucketsX;

                    Buckets[bucketId] = new LayerGridBucket(bucketId, bucketRect, Layer, RendererType);
                }
            }
        }
    }
}
