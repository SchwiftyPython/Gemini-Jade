using GoRogue;
using Graphics;

namespace World
{
    public class GroundGrid : LayerGrid
    {
        public GroundGrid(Coord size) : base(size, MapLayer.Terrain)
        {
            RendererType = typeof(BucketRenderer);
            
            GenerateBuckets();
        }
    }
}
