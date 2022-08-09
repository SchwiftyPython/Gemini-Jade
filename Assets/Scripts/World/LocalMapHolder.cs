using GoRogue;
using GoRogue.GameFramework;
using UnityEngine;
using Utilities;
using GameObject = UnityEngine.GameObject;

namespace World
{
    public class LocalMapHolder : MonoBehaviour
    {
        public GameObject terrainSlotPrefab;
    
        public Transform pawnHolder;

        public Transform terrainHolder;

        public void Build(LocalMap map)
        {
            Clear();
            
            PlaceTiles(map);
            
            PlacePawns(map);
        }

        private void PlaceTiles(Map map)
        {
            for (var currentColumn = 0; currentColumn < map.Width; currentColumn++)
            {
                for (var currentRow = 0; currentRow < map.Height; currentRow++)
                {
                    var coord = new Coord(currentColumn, currentRow);

                    var tile = map.GetTerrain<Tile>(coord);

                    var tileInstance = Instantiate(terrainSlotPrefab, new Vector2(currentColumn, currentRow),
                        Quaternion.identity, terrainHolder);

                    tileInstance.GetComponentInChildren<SpriteRenderer>().sprite = tile.Texture;

                    tile.SetSpriteInstance(tileInstance);
                }
            }
        }

        private void PlacePawns(LocalMap map)
        {
            var pawns = map.GetAllPawns();

            foreach (var pawn in pawns)
            {
                var pawnInstance = Instantiate(pawn.species.prefab, new Vector2(pawn.Position.X, pawn.Position.Y),
                    Quaternion.identity, pawnHolder);

                UnityUtils.AddPathfindingTo(pawn, pawnInstance);
            }
        }

        private void Clear()
        {
            terrainHolder.gameObject.DestroyAllChildren();
            
            pawnHolder.gameObject.DestroyAllChildren();
        }
    }
}
