using System.Collections.Generic;
using Graphics.GraphicTemplates;
using UnityEngine;

namespace Graphics
{
	public class GraphicInstance
	{
		public static Dictionary<int, GraphicInstance> instances = new Dictionary<int, GraphicInstance>();

		public int Uid { get; protected set; }

		public Material Material { get; protected set; }
		public Texture2D Texture { get; protected set; }
		public Color Color { get; protected set; }
		public GraphicTemplate Template { get; protected set; }
		public float Priority { get; protected set; }
		public Mesh Mesh { get; protected set; }

		/// Create a new graphic instance
		public GraphicInstance(
			int uid, 
			GraphicTemplate template, 
			Mesh mesh,
			Color color = default(Color), 
			Texture2D texture = null,
			float drawPriority = -42f
		) {
			this.Mesh = mesh;
			this.Template = template;
			this.Uid = uid;
			Priority = drawPriority / -100f;
			Material = new Material(template.material)
			{
				mainTexture = texture
			};
			this.Texture = texture;

			if (color != default) 
			{
				SetColor(color);
			}
		}

		/// Set the variable _Color of the material/shader
		private void SetColor(Color newColor)
		{
			Color = newColor;
			
			Material.SetColor("_Color", Color);
		}

		/// Get a new graphic instance (or an existing one)
		public static GraphicInstance GetNew(
			GraphicTemplate template, 
			Color color = default(Color), 
			Texture2D texture = null,
			float drawPriority = -42f,
			Mesh mesh = null
		) {
			Mesh _mesh = (mesh == null) ? MeshPool.GetPlaneMesh(template.size) : mesh;
			Color _color = (color == default(Color)) ? template.color : color;
			Texture2D _texture = (texture == null) ? template.texture : texture;
			float _priority = (drawPriority == -42f) ? template.drawPriority : drawPriority;

			int id = GetUid(template, _color, _texture, _priority, _mesh);
			if (instances.ContainsKey(id)) {
				return instances[id];
			}
			instances.Add(id, new GraphicInstance(id, template, _mesh, _color, _texture, _priority));
			return instances[id];
		}

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
