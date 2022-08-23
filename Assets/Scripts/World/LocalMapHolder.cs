using GoRogue;
using GoRogue.GameFramework;
using UnityEngine;
using Utilities;
using GameObject = UnityEngine.GameObject;

namespace World
{
    /// <summary>
    /// The local map holder class
    /// </summary>
    /// <seealso cref="MonoBehaviour"/>
    public class LocalMapHolder : MonoBehaviour
    {
        /// <summary>
        /// The local map
        /// </summary>
        private LocalMap _localMap;
        
        /// <summary>
        /// The terrain slot prefab
        /// </summary>
        public GameObject terrainSlotPrefab;
    
        /// <summary>
        /// The pawn holder
        /// </summary>
        public Transform pawnHolder;

        /// <summary>
        /// The terrain holder
        /// </summary>
        public Transform terrainHolder;

        /// <summary>
        /// Builds the map
        /// </summary>
        /// <param name="map">The map</param>
        public void Build(LocalMap map)
        {
            Clear();

            _localMap = map;
            
            PlaceTiles();
            
            PlacePawns();
        }

        /// <summary>
        /// Places the tiles
        /// </summary>
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

        /// <summary>
        /// Places the pawns
        /// </summary>
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

        /// <summary>
        /// Clears this instance
        /// </summary>
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
