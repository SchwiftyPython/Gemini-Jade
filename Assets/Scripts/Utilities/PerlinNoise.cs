using System;
using System.Linq;
using Troschuetz.Random;
using UnityEngine;

namespace Utilities
{
    /// <summary>
    /// Generates some Perlin noise in 2d
    /// </summary>
    /// <remarks></remarks>
    public static class PerlinNoise
    {
        private static readonly TRandom Random = TRandom.New();

        private static int[] _permutation;
        
        private static readonly Vector2[] Gradients;

        /// <summary>
        /// Constructor
        /// </summary>
        static PerlinNoise()
        {
            CalculatePermutation(out _permutation);
            
            CalculateGradients(out Gradients);
        }
        
        /// <summary>
        /// Calculates permutation
        /// </summary>
        /// <param name="p"></param>
        private static void CalculatePermutation(out int[] p)
        {
            p = Enumerable.Range(0, 256).ToArray();

            for (var i = 0; i < p.Length; i++)
            {
                var source = Random.Next(p.Length);

                (p[i], p[source]) = (p[source], p[i]);
            }
        }

        /// <summary>
        /// Reseeds
        /// </summary>
        private static void Reseed()
        {
            CalculatePermutation(out _permutation);
        }
        
        /// <summary>
        /// Calculates gradients
        /// </summary>
        /// <param name="grad"></param>
        private static void CalculateGradients(out Vector2[] grad)
        {
            grad = new Vector2[256];

            for (var i = 0; i < grad.Length; i++)
            {
                Vector2 gradient;

                do
                {
                    gradient = new Vector2((float)(Random.NextDouble() * 2 - 1), (float)(Random.NextDouble() * 2 - 1));
                }
                while (Mathf.Pow(gradient.magnitude, 2) >= 1);

                gradient.Normalize();

                grad[i] = gradient;
            }

        }

        /// <summary>
        /// Drops
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        private static float Drop(float t)
        {
            t = Math.Abs(t);
            
            return 1f - t * t * t * (t * (t * 6 - 15) + 10);
        }

        /// <summary>
        /// Q things
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private static float Q(float x, float y)
        {
            return Drop(x) * Drop(y);
        }

        /// <summary>
        /// Generates a noise value
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>A noise value</returns>
        private static float Noise(float x, float y)
        {
            var cell = new Vector2((float)Math.Floor(x), (float)Math.Floor(y));

            var total = 0f;

            var corners = new[] { new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 0), new Vector2(1, 1) };

            foreach (var n in corners)
            {
                var ij = cell + n;
                
                var uv = new Vector2(x - ij.x, y - ij.y);

                var index = _permutation[(int)ij.x % _permutation.Length];
                
                index = _permutation[(index + (int)ij.y) % _permutation.Length];

                var grad = Gradients[index % Gradients.Length];

                total += Q(uv.x, uv.y) * Vector2.Dot(grad, uv);
            }

            return Mathf.Clamp(total, 0.05f, 1);
        }

        /// <summary>
        /// Generates a 2d noise map
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="octaves"></param>
        /// <returns>A 2d noise map represented by a 1d array</returns>
        public static float[] GenerateNoiseMap(int width, int height, int octaves)
        {
            var noiseMap = new float[width * height];
            
            var min = float.MaxValue;
            
            var max = float.MinValue;
            
            Reseed();
            
            var frequency = 0.5f;
            
            var amplitude = 1f;

            for (var octave = 0; octave < octaves; octave++)
            {
                for (var x = 0; x < width; x++)
                {
                    for (var y = 0; y < height; y++)
                    {
                        var noise = Noise(x*frequency*1f/width, y*frequency*1f/height);
                    
                        noise = noiseMap[x + y * width] += noise * amplitude;
                    
                        min = Math.Min(min, noise);
                    
                        max = Math.Max(max, noise);
                    }
                }
                
                frequency *= 2;
            
                amplitude /= 2;
            }
            
            //todo add to logging
            Debug.Log($"Avg noise {noiseMap.AsQueryable().Average()}");

            return noiseMap;
        }

    }
}
