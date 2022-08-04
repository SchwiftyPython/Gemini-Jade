using System;

namespace World
{
    [Flags]
    public enum BitMaskDirection 
    {
        NorthWest = 1 << 0,
        North = 1 << 1,
        NorthEast = 1 << 2,
        West = 1 << 3,
        East = 1 << 4,
        SouthWest = 1 << 5,
        South = 1 << 6,
        SouthEast = 1 << 7
    }
}
