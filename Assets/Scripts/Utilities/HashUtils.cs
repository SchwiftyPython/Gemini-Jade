using System;
using Time;
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
            return HashCombine(baseInt, 134217727); //some magic number. I did int.max / 16
        }

        public static int HashOffsetTicks(this Thing thing)
        {
            var tickController = Object.FindObjectOfType<TickController>();

            return tickController.NumTicks + thing.id.HashOffset();
        }

        public static bool IsHashIntervalTick(this Thing thing, int interval)
        {
            return thing.HashOffsetTicks() % interval == 0;
        }
    }
}
