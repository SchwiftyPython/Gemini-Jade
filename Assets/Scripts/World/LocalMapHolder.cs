using Generators;
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
    
        public Transform entityHolder;

        private void Start()
        {
            //testing map gen

            var mapGen = new LocalMapGenerator();
            
            var map = mapGen.GenerateMap(50, 50);
            
            Build(map);
        }

        public void Build(LocalMap map)
        {
            Clear();
            
            PlaceTiles(map);
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
                        Quaternion.identity);

                    tileInstance.GetComponentInChildren<SpriteRenderer>().sprite = tile.Texture;

                    tile.SetSpriteInstance(tileInstance);
                }
            }
        }

        private void Clear()
        {
            gameObject.DestroyAllChildren();
            
            entityHolder.gameObject.DestroyAllChildren();
        }
    }
}
