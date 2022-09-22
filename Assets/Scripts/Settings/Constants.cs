using UnityEngine;

namespace Settings
{
    /// <summary>
    /// Static class to get various constants
    /// </summary>
    public static class Constants
    {
        public const int BucketSize = 31;

        public const int TicksPerDay = 84000;

        public static readonly (float, float) MeshOffset = (-0.5f, -0.5f);
    }
}
