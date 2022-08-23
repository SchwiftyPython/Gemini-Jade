using System;

namespace World
{
    /// <summary>
    /// The bit mask direction enum
    /// </summary>
    [Flags]
    public enum BitMaskDirection 
    {
        /// <summary>
        /// The north west bit mask direction
        /// </summary>
        NorthWest = 1 << 0,
        /// <summary>
        /// The north bit mask direction
        /// </summary>
        North = 1 << 1,
        /// <summary>
        /// The north east bit mask direction
        /// </summary>
        NorthEast = 1 << 2,
        /// <summary>
        /// The west bit mask direction
        /// </summary>
        West = 1 << 3,
        /// <summary>
        /// The east bit mask direction
        /// </summary>
        East = 1 << 4,
        /// <summary>
        /// The south west bit mask direction
        /// </summary>
        SouthWest = 1 << 5,
        /// <summary>
        /// The south bit mask direction
        /// </summary>
        South = 1 << 6,
        /// <summary>
        /// The south east bit mask direction
        /// </summary>
        SouthEast = 1 << 7
    }
}
