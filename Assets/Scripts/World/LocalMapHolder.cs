using GoRogue;
using GoRogue.GameFramework;
using UnityEngine;
using Utilities;
using GameObject = UnityEngine.GameObject;

namespace World
{
    public class LocalMapHolder : MonoBehaviour
    {
        private LocalMap _localMap;
        
        public GameObject terrainSlotPrefab;
    
        public Transform pawnHolder;

        public Transform terrainHolder;

        public void Build(LocalMap map)
        {
            Clear();

            _localMap = map;
            
            PlaceTiles();
            
            PlacePawns();
        }

        private void PlaceTiles()
        {
            for (var currentColumn = 0; currentColumn < _localMap.Width; currentColumn++)
            {
                for (var currentRow = 0; currentRow < _localMap.Height; currentRow++)
                {
                    var coord = new Coord(currentColumn, currentRow);

                    var tile = _localMap.GetTerrain<Tile>(coord);

                    var tileInstance = Instantiate(terrainSlotPrefab, new Vector2(currentColumn, currentRow),
                        Quaternion.identity, terrainHolder);

                    tileInstance.GetComponentInChildren<SpriteRenderer>().sprite = tile.Texture;

                    tile.SetSpriteInstance(tileInstance);
                }
            }
        }

        private void PlacePawns()
        {
            var pawns = _localMap.GetAllPawns();

            foreach (var pawn in pawns)
            {
                var pawnInstance = Instantiate(pawn.species.Prefab, new Vector2(pawn.Position.X, pawn.Position.Y),
                    Quaternion.identity, pawnHolder);
                
                pawn.SetSpriteInstance(pawnInstance.gameObject);

                UnityUtils.AddPathfindingTo(pawn, pawnInstance.gameObject);
                
                pawn.UpdateSpriteFacing(Direction.DOWN);

                pawn.spawned = true;
            }
        }

        private void Clear()
        {
            terrainHolder.gameObject.DestroyAllChildren();

            pawnHolder.gameObject.DestroyAllChildren();

            if (_localMap == null)
            {
                return;
            }
            
            var pawns = _localMap.GetAllPawns();

            foreach (var pawn in pawns.ToArray())
            {
                pawn.spawned = false;

                pawns.Remove(pawn);
                
                //todo need to hangout in a world pawn collection or something if they are needed later
            }
        }
    }
}
