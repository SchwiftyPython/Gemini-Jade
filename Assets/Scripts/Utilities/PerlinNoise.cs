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
        private static TRandom _random = TRandom.New();

        private static int[] _permutation;
        
        private static Vector2[] _gradients;

        static PerlinNoise()
        {
            CalculatePermutation(out _permutation);
            
            CalculateGradients(out _gradients);
        }
        
        private static void CalculatePermutation(out int[] p)
        {
            p = Enumerable.Range(0, 256).ToArray();

            for (var i = 0; i < p.Length; i++)
            {
                var source = _random.Next(p.Length);

                (p[i], p[source]) = (p[source], p[i]);
            }
        }

        public static void Reseed()
        {
            CalculatePermutation(out _permutation);
        }

        private static void CalculateGradients(out Vector2[] grad)
        {
            grad = new Vector2[256];

            for (var i = 0; i < grad.Length; i++)
            {
                Vector2 gradient;

                do
                {
                    gradient = new Vector2((float)(_random.NextDouble() * 2 - 1), (float)(_random.NextDouble() * 2 - 1));
                }
                while (Mathf.Pow(gradient.magnitude, 2) >= 1);

                gradient.Normalize();

                grad[i] = gradient;
            }

        }

        private static float Drop(float t)
        {
            t = Math.Abs(t);
            
            return 1f - t * t * t * (t * (t * 6 - 15) + 10);
        }

        private static float Q(float u, float v)
        {
            return Drop(u) * Drop(v);
        }

        public static float Noise(float x, float y)
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

                var grad = _gradients[index % _gradients.Length];

                total += Q(uv.x, uv.y) * Vector2.Dot(grad, uv);
            }

            return Mathf.Clamp(total, 0.05f, 1);

            return Math.Min(Math.Max(total, 0f), 1f) * 10;
        }

        public static float[] GenerateNoiseMap(int width, int height, int octaves)
        {
            var noiseMap = new float[width * height];
            
            var min = float.MaxValue;
            
            var max = float.MinValue;
            
            Reseed();
            
            var frequency = 0.75f;
            
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
            
            Debug.Log($"Avg noise {noiseMap.AsQueryable().Average()}");

            return noiseMap;
        }

    }
}
