using UnityEditor;

namespace Pathfinding.Legacy {
	/// <summary>
	/// The legacy ai path editor class
	/// </summary>
	/// <seealso cref="BaseAIEditor"/>
	[CustomEditor(typeof(LegacyAIPath))]
	[CanEditMultipleObjects]
	public class LegacyAIPathEditor : BaseAIEditor {
		/// <summary>
		/// Inspectors this instance
		/// </summary>
		protected override void Inspector () {
			base.Inspector();
			var gravity = FindProperty("gravity");
			if (!gravity.hasMultipleDifferentValues && !float.IsNaN(gravity.vector3Value.x)) {
				gravity.vector3Value = new UnityEngine.Vector3(float.NaN, float.NaN, float.NaN);
				serializedObject.ApplyModifiedPropertiesWithoutUndo();
			}
			LegacyEditorHelper.UpgradeDialog(targets, typeof(AIPath));
		}
	}
}
