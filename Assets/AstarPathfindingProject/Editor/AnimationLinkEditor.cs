using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Pathfinding {
	/// <summary>
	/// The animation link editor class
	/// </summary>
	/// <seealso cref="Editor"/>
	[CustomEditor(typeof(AnimationLink))]
	public class AnimationLinkEditor : Editor {
		/// <summary>
		/// Ons the inspector gui
		/// </summary>
		public override void OnInspectorGUI () {
			DrawDefaultInspector();

			var script = target as AnimationLink;

			EditorGUI.BeginDisabledGroup(script.EndTransform == null);
			if (GUILayout.Button("Autoposition Endpoint")) {
				List<Vector3> buffer = Pathfinding.Util.ListPool<Vector3>.Claim();
				Vector3 endpos;
				script.CalculateOffsets(buffer, out endpos);
				script.EndTransform.position = endpos;
				Pathfinding.Util.ListPool<Vector3>.Release(buffer);
			}
			EditorGUI.EndDisabledGroup();
		}
	}
}
