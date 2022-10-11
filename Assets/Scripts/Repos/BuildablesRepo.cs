using System.Collections.Generic;
using UnityEngine;
using World.Things.CraftableThings;
using World.TileTypes;

namespace Repos
{
    /// <summary>
    /// Stores all Buildable Tile Types.
    /// </summary>
    /// <remarks>Functions like a Singleton. Use FindObjectOfType&lt;BuildablesRepo&gt;() to get a reference.</remarks>
    public class BuildablesRepo : MonoBehaviour
    {
        /// <summary>
        /// The num wall sprites
        /// </summary>
        private const int NumWallSprites = 47;
        
        /// <summary>
        /// The width
        /// </summary>
        private const int Width = 32;

        /// <summary>
        /// The height
        /// </summary>
        private const int Height = 32;

        /// <summary>
        /// The num per row
        /// </summary>
        private const int NumPerRow = 8;
        
        /// <summary>
        /// The wall
        /// </summary>
        [SerializeField] private TileType wall;

        /// <summary>
        /// The test wall sprite sheet
        /// </summary>
        [SerializeField] private Sprite testWallSpriteSheet; //todo need a collection of these for each material -- have this be black and white and color at runtime

        /// <summary>
        /// The test wall sprites
        /// </summary>
        private Sprite[] _testWallSprites;

        public List<PlacedObjectTemplate> buildings;

        /// <summary>
        /// Starts this instance
        /// </summary>
        private void Start()
        {
            LoadWallSprites();
        }

        /// <summary>
        /// Gets the wall sprite at using the specified index
        /// </summary>
        /// <param name="index">The index</param>
        /// <returns>The sprite</returns>
        public Sprite GetWallSpriteAt(int index)
        {
            return _testWallSprites[index];
        }

        /// <summary>
        /// Loads the wall sprites
        /// </summary>
        private void LoadWallSprites()
        {
            _testWallSprites = new Sprite[NumWallSprites];

            var colIndex = 0;

            var rowIndex = 5;

            for (var i = 0; i < NumWallSprites; i++)
            {
                if (colIndex >= NumPerRow)
                {
                    colIndex = 0;

                    rowIndex--;
                }

                var x = colIndex * Width;

                var y = rowIndex * Height;

                var sprite = Sprite.Create(testWallSpriteSheet.texture, new Rect(x, y, Width, Height),
                    new Vector2(0.5f, 0.5f), 32);

                _testWallSprites[i] = sprite;

                colIndex++;
            }
        }
    }
}
