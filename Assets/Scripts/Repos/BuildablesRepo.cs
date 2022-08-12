using UnityEngine;
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
        
        private const int Width = 32;

        private const int Height = 32;

        private const int NumPerRow = 8;
        
        [SerializeField] private TileType wall;

        [SerializeField] private Sprite testWallSpriteSheet; //todo need a collection of these for each material

        private Sprite[] _testWallSprites;

        private void Start()
        {
            LoadWallSprites();
        }

        public Sprite GetWallSpriteAt(int index)
        {
            return _testWallSprites[index];
        }

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
