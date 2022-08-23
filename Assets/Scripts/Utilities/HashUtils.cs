using System;
using Time;
using UnityEngine;
using World.Things;
using Object = UnityEngine.Object;

namespace Utilities
{
    /// <summary>
    /// The hash utils class
    /// </summary>
    public static class HashUtils
    {
        /// <summary>
        /// Hashes the combine using the specified seed
        /// </summary>
        /// <param name="seed">The seed</param>
        /// <param name="value">The value</param>
        /// <returns>The int</returns>
        public static int HashCombine(int seed, int value)
        {
            return HashCode.Combine(seed, value);
        }

        /// <summary>
        /// Hashes the offset using the specified base int
        /// </summary>
        /// <param name="baseInt">The base int</param>
        /// <returns>The int</returns>
        public static int HashOffset(this int baseInt)
        {
            return HashCombine(baseInt, 169495093); //some magic number.
        }

        /// <summary>
        /// Hashes the offset ticks using the specified thing
        /// </summary>
        /// <param name="thing">The thing</param>
        /// <returns>The int</returns>
        public static int HashOffsetTicks(this Thing thing)
        {
            var tickController = Object.FindObjectOfType<TickController>();

            return tickController.NumTicks + thing.id.HashOffset();
        }

        /// <summary>
        /// Describes whether is hash interval tick
        /// </summary>
        /// <param name="thing">The thing</param>
        /// <param name="interval">The interval</param>
        /// <returns>The bool</returns>
        public static bool IsHashIntervalTick(this Thing thing, int interval)
        {
            var hashOffsetTicks = thing.HashOffsetTicks();
            
            Debug.Log($"Pawn Hash Offset Ticks: {hashOffsetTicks}");
            
            return hashOffsetTicks % interval == 0;
        }
    }
}
