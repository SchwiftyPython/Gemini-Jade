using GoRogue;
using GoRogue.GameFramework;

namespace World
{
    public class LocalMap : Map
    {
        public LocalMap(int width, int height) : base(width, height, 1, Distance.CHEBYSHEV)
        {
            Direction.YIncreasesUpward = true;
        }
        
        public bool OutOfBounds(Coord targetCoord)
        {
            var (x, y) = targetCoord;

            return x >= Width || x < 0 || y >= Height || y < 0;
        }
        
        public Tile GetTileAt(Coord position)
        {
            return OutOfBounds(position) ? null : GetTerrain<Tile>(position);
        }
    }
}
