using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GoRogue;
using Graphics;
using Settings;
using Utilities;
using World.Things;

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

            _numBucketsX = Mathf.CeilToInt(Size.X / (float)Constants.BucketSize);
            
            _numBucketsY = Mathf.CeilToInt(Size.Y / (float)Constants.BucketSize);

            _bucketCount = _numBucketsX * _numBucketsY;
            
            //GenerateBuckets();
        }

        public void AddThing(Thing thing)
        {
            var bucket = GetBucketAt(thing.Position);
            
            bucket?.AddThing(thing);
        }

        public void AddTile(Tile tile)
        {
            var bucket = GetBucketAt(tile.Position);

            bucket?.AddTile(tile);
        }

        public void AddBaseObject(BaseObject baseObject)
        {
            var bucket = GetBucketAt(baseObject.Position);
            
            bucket?.AddBaseObject(baseObject);
        }

        public List<BaseObject> GetBaseObjects()
        {
            return (from bucket in Buckets
                from baseObject in bucket.BaseObjects
                where baseObject != null
                select baseObject).ToList();
        }

        public BaseObject GetBaseObjectAt(Coord position)
        {
            var bucket = GetBucketAt(position);

            if (bucket == null)
            {
                return null;
            }

            return bucket.GetBaseObjectAt(position);
        }

        public LayerGridBucket GetBucketAt(Coord position)
        {
            var bucketIndex = position.X / Constants.BucketSize + position.Y / Constants.BucketSize * _numBucketsX;

            if (bucketIndex >= 0 && bucketIndex < Buckets.Length)
            {
                var bucket = Buckets[bucketIndex];

                return bucket;
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
            if (RendererType == null)
            {
                foreach (var bucket in Buckets)
                {
                    if (bucket.IsVisible())
                    {
                        bucket.DrawInstancedMeshes();
                    }
                }
                
                return;
            }
            
            foreach (var bucket in Buckets)
            {
                if (bucket.IsVisible())
                {
                    bucket.DrawStatics();

                    bucket.DrawInstancedMeshes();
                }
            }
        }

        protected void GenerateBuckets()
        {
            Buckets = new LayerGridBucket[_bucketCount];

            for (var x = 0; x < Size.X; x += Constants.BucketSize)
            {
                for (var y = 0; y < Size.Y; y += Constants.BucketSize) 
                {
                    var bucketRect = new Rectangle(x, y, Constants.BucketSize, Constants.BucketSize);

                    bucketRect = bucketRect.Clip(Rect);

                    var bucketId = x / Constants.BucketSize + y / Constants.BucketSize * _numBucketsX;

                    Buckets[bucketId] = new LayerGridBucket(bucketId, bucketRect, Layer, RendererType);
                }
            }
        }
    }
}
