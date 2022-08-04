using System.Collections.Generic;
using GoRogue;
using UnityEngine;
using World;
using World.TileTypes;

namespace Repos
{
    /// <summary>
    /// Stores all Buildable Tile Types.
    /// </summary>
    /// <remarks>Functions like a Singleton. Use FindObjectOfType&lt;BuildablesRepo&gt;() to get a reference.</remarks>
    public class BuildablesRepo : MonoBehaviour
    {
        private const int NumWallSprites = 47;
        
        [SerializeField] private TileType wall;

        [SerializeField] private Sprite testWallSpriteSheet;

        private List<Sprite> _testWallSprites;

        private void Start()
        {
            LoadWallSprites();
        }

        /// <summary>
        /// Get a new Wall <see cref="Tile"/> at given position.
        /// </summary>
        /// <param name="position">Given position.</param>
        /// <returns>A Ground <see cref="Tile"/> at given position.</returns>
        /// <remarks>Just for testing out grid system. Might work if we pass in wall type as a param.</remarks>
        public Tile WallTile(Coord position)
        {
            return wall.NewTile(position);
        }
        
        public Sprite GetWallSpriteAt(int index)
        {
            return _testWallSprites[index];
        }

        private void LoadWallSprites()
        {
            _testWallSprites = new List<Sprite>();

            int width = 32;

            int height = 32;

            var numPerRow = 8;

            var rowIndex = 0;

            for (int i = 0; i < NumWallSprites; i++)
            {
                if (rowIndex >= numPerRow)
                {
                    rowIndex = 0;
                }
                
                var x = rowIndex * width;
                
                var sprite = Sprite.Create(testWallSpriteSheet.texture, new Rect(x, 0, width, height), new Vector2(0.0f, 0.0f), 32);
                
                _testWallSprites.Add(sprite);

                rowIndex++;
            }
        }
    }
}
