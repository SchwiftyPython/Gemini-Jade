using UnityEngine;
using System.Collections.Generic;

namespace Pathfinding {
	/// <summary>
	/// The alternative path class
	/// </summary>
	/// <seealso cref="MonoModifier"/>
	[AddComponentMenu("Pathfinding/Modifiers/Alternative Path")]
	[System.Serializable]
	/// <summary>
	/// Applies penalty to the paths it processes telling other units to avoid choosing the same path.
	///
	/// Note that this might not work properly if penalties are modified by other actions as well (e.g graph update objects which reset the penalty to zero).
	/// It will only work when all penalty modifications are relative, i.e adding or subtracting penalties, but not when setting penalties
	/// to specific values.
	///
	/// When destroyed, it will correctly remove any added penalty.
	///
	/// \ingroup modifiers
	/// </summary>
	[HelpURL("http://arongranberg.com/astar/docs/class_pathfinding_1_1_alternative_path.php")]
	public class AlternativePath : MonoModifier {
#if UNITY_EDITOR
		[UnityEditor.MenuItem("CONTEXT/Seeker/Add Alternative Path Modifier")]
		public static void AddComp (UnityEditor.MenuCommand command) {
			(command.context as Component).gameObject.AddComponent(typeof(AlternativePath));
		}
#endif

		/// <summary>
		/// Gets the value of the order
		/// </summary>
		public override int Order { get { return 10; } }

		/// <summary>How much penalty (weight) to apply to nodes</summary>
		public int penalty = 1000;

		/// <summary>Max number of nodes to skip in a row</summary>
		public int randomStep = 10;

		/// <summary>The previous path</summary>
		List<GraphNode> prevNodes = new List<GraphNode>();

		/// <summary>The previous penalty used. Stored just in case it changes during operation</summary>
		int prevPenalty;

		/// <summary>A random object</summary>
		readonly System.Random rnd = new System.Random();

		/// <summary>
		/// The destroyed
		/// </summary>
		bool destroyed;

		/// <summary>
		/// Applies the p
		/// </summary>
		/// <param name="p">The </param>
		public override void Apply (Path p) {
			if (this == null) return;

			ApplyNow(p.path);
		}

		/// <summary>
		/// Ons the destroy
		/// </summary>
		protected void OnDestroy () {
			destroyed = true;
			ClearOnDestroy();
		}

		/// <summary>
		/// Clears the on destroy
		/// </summary>
		void ClearOnDestroy () {
			InversePrevious();
		}

		/// <summary>
		/// Inverses the previous
		/// </summary>
		void InversePrevious () {
			// Remove previous penalty
			if (prevNodes != null) {
				bool warnPenalties = false;
				for (int i = 0; i < prevNodes.Count; i++) {
					if (prevNodes[i].Penalty < prevPenalty) {
						warnPenalties = true;
						prevNodes[i].Penalty = 0;
					} else {
						prevNodes[i].Penalty = (uint)(prevNodes[i].Penalty-prevPenalty);
					}
				}
				if (warnPenalties) {
					Debug.LogWarning("Penalty for some nodes has been reset while the AlternativePath modifier was active (possibly because of a graph update). Some penalties might be incorrect (they may be lower than expected for the affected nodes)");
				}
			}
		}

		/// <summary>
		/// Applies the now using the specified nodes
		/// </summary>
		/// <param name="nodes">The nodes</param>
		void ApplyNow (List<GraphNode> nodes) {
			InversePrevious();
			prevNodes.Clear();

			if (destroyed) return;

			if (nodes != null) {
				int rndStart = rnd.Next(randomStep);
				for (int i = rndStart; i < nodes.Count; i += rnd.Next(1, randomStep)) {
					nodes[i].Penalty = (uint)(nodes[i].Penalty+penalty);
					prevNodes.Add(nodes[i]);
				}
			}

			prevPenalty = penalty;
		}
	}
}
