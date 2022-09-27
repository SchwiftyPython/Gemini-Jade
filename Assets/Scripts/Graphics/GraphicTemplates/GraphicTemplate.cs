using Assets.Scripts.World;
using UnityEngine;

namespace Graphics.GraphicTemplates
{
    [CreateAssetMenu(menuName = "Create GraphicTemplate", fileName = "GraphicTemplate")]
    public class GraphicTemplate : Template
    {
        public Texture2D texture;
        
        public Material material;
        
        public Vector2 size = Vector2.one;
        
        public Color color = Color.white;
        
        public float drawPriority;
        
        public Vector2 pivot = new(0.5f, 0.5f);

        public bool isInstanced;
    }
}
