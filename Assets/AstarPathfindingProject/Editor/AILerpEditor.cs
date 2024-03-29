using UnityEditor;
using UnityEngine;

namespace Pathfinding {
	/// <summary>
	/// The ai lerp editor class
	/// </summary>
	/// <seealso cref="BaseAIEditor"/>
	[CustomEditor(typeof(AILerp), true)]
	[CanEditMultipleObjects]
	public class AILerpEditor : BaseAIEditor {
		/// <summary>
		/// Inspectors this instance
		/// </summary>
		protected override void Inspector () {
			Section("Pathfinding");
			AutoRepathInspector();

			Section("Movement");
			FloatField("speed", min: 0f);
			PropertyField("canMove");
			if (PropertyField("enableRotation")) {
				EditorGUI.indentLevel++;
				Popup("orientation", new [] { new GUIContent("ZAxisForward (for 3D games)"), new GUIContent("YAxisForward (for 2D games)") });
				FloatField("rotationSpeed", min: 0f);
				EditorGUI.indentLevel--;
			}

			if (PropertyField("interpolatePathSwitches")) {
				EditorGUI.indentLevel++;
				FloatField("switchPathInterpolationSpeed", min: 0f);
				EditorGUI.indentLevel--;
			}

			DebugInspector();
		}
	}
}
