using System;
using Time;
using UnityEngine;
using World.Things;
using Object = UnityEngine.Object;

namespace Utilities
{
    public static class HashUtils
    {
        public static int HashCombine(int seed, int value)
        {
            return HashCode.Combine(seed, value);
        }

        public static int HashOffset(this int baseInt)
        {
            return HashCombine(baseInt, 169495095); //some magic number.
        }

        public static int HashOffsetTicks(this Thing thing)
        {
            var tickController = Object.FindObjectOfType<TickController>();

            return tickController.NumTicks + thing.id.HashOffset();
        }

        public static bool IsHashIntervalTick(this Thing thing, int interval)
        {
            var hashOffsetTicks = thing.HashOffsetTicks();
            
            Debug.Log($"Pawn Hash Offset Ticks: {hashOffsetTicks}");
            
            return hashOffsetTicks % interval == 0;
        }
    }
}
