using System.Collections.Generic;
using Graphics.GraphicTemplates;
using UnityEngine;

namespace Graphics
{
    /// <summary>
    /// Graphic instance
    /// </summary>
    public class GraphicInstance
    {
        public static Dictionary<int, GraphicInstance> instances = new();

        public int Uid { get; }

        public Material Material { get; }

        public Texture2D Texture { get; }

        public Color Color { get; protected set; }

        public GraphicTemplate Template { get; }

        public float Priority { get; }

        public Mesh Mesh { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="template"></param>
        /// <param name="mesh"></param>
        /// <param name="color"></param>
        /// <param name="texture"></param>
        /// <param name="drawPriority"></param>
        public GraphicInstance(
            int uid,
            GraphicTemplate template,
            Mesh mesh,
            Color color = default(Color),
            Texture2D texture = null,
            float drawPriority = -42f
        )
        {
            Mesh = mesh;
            Template = template;
            Uid = uid;
            Priority = drawPriority / -100f;
            Material = new Material(template.material)
            {
                mainTexture = texture
            };
            Texture = texture;

            if (color != default)
            {
                SetColor(color);
            }
        }

        /// <summary>
        /// Set the variable _Color of the material/shader
        /// </summary>
        /// <param name="newColor"></param>
        private void SetColor(Color newColor)
        {
            Color = newColor;

            Material.SetColor("_Color", Color);
        }

        /// <summary>
        /// Get a new graphic instance (or an existing one)
        /// </summary>
        /// <param name="template"></param>
        /// <param name="color"></param>
        /// <param name="texture"></param>
        /// <param name="drawPriority"></param>
        /// <param name="mesh"></param>
        /// <returns></returns>
        public static GraphicInstance GetNew(
            GraphicTemplate template,
            Color color = default(Color),
            Texture2D texture = null,
            float drawPriority = -42f,
            Mesh mesh = null
        )
        {
            var _mesh = mesh == null ? MeshPool.GetPlaneMesh(template.size) : mesh;
            var _color = color == default ? template.color : color;
            var _texture = texture == null ? template.texture : texture;
            var _priority = drawPriority == -42f ? template.drawPriority : drawPriority;

            var id = GetUid(template, _color, _texture, _priority, _mesh);
            if (instances.ContainsKey(id))
            {
                return instances[id];
            }

            instances.Add(id, new GraphicInstance(id, template, _mesh, _color, _texture, _priority));
            return instances[id];
        }

        /// <summary>
        /// Gets hash code
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return Uid;
        }

        /// <summary>
        /// Unique id generator for GraphicInstance
        /// </summary>
        /// <param name="template"></param>
        /// <param name="color"></param>
        /// <param name="texture"></param>
        /// <param name="drawPriority"></param>
        /// <param name="mesh"></param>
        /// <returns></returns>
        public static int GetUid(
            GraphicTemplate template,
            Color color,
            Texture2D texture,
            float drawPriority,
            Mesh mesh
        )
        {
            return template.material.name.GetHashCode() + texture.GetHashCode() + color.GetHashCode() +
                   drawPriority.GetHashCode() + mesh.GetHashCode();
        }

        public override string ToString()
        {
            return
                $"GraphicInstance(template={Template}, uid={Uid}, priority={Priority}, mat={Material}, texture={Texture}, mesh={Mesh})";
        }
    }
}