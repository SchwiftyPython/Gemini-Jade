using GoRogue;

namespace World
{
    public class InstancedGrid : LayerGrid
    {
        public InstancedGrid(Coord size, MapLayer layer) : base(size, layer)
        {
            RendererType = null;
            
            GenerateBuckets();
        }
    }
}
