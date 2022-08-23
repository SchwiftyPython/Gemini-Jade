using UnityEngine;
using UnityEditor;

namespace Pathfinding {
	/// <summary>
	/// The smooth modifier editor class
	/// </summary>
	/// <seealso cref="EditorBase"/>
	[CustomEditor(typeof(SimpleSmoothModifier))]
	[CanEditMultipleObjects]
	public class SmoothModifierEditor : EditorBase {
		/// <summary>
		/// Inspectors this instance
		/// </summary>
		protected override void Inspector () {
			var smoothType = FindProperty("smoothType");

			PropertyField("smoothType");

			if (!smoothType.hasMultipleDifferentValues) {
				switch ((SimpleSmoothModifier.SmoothType)smoothType.enumValueIndex) {
				case SimpleSmoothModifier.SmoothType.Simple:
					if (PropertyField("uniformLength")) {
						FloatField("maxSegmentLength", min: 0.005f);
					} else {
						IntSlider("subdivisions", 0, 6);
					}

					PropertyField("iterations");
					ClampInt("iterations", 0);

					PropertyField("strength");
					break;
				case SimpleSmoothModifier.SmoothType.OffsetSimple:
					PropertyField("iterations");
					ClampInt("iterations", 0);

					FloatField("offset", min: 0f);
					break;
				case SimpleSmoothModifier.SmoothType.Bezier:
					IntSlider("subdivisions", 0, 6);
					PropertyField("bezierTangentLength");
					break;
				case SimpleSmoothModifier.SmoothType.CurvedNonuniform:
					FloatField("maxSegmentLength", min: 0.005f);
					PropertyField("factor");
					break;
				}
			}
		}
	}
}
