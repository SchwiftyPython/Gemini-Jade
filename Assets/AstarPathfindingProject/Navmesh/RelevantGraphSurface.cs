using UnityEngine;

namespace Pathfinding {
	/// <summary>
	/// Pruning of recast navmesh regions.
	/// A RelevantGraphSurface component placed in the scene specifies that
	/// the navmesh region it is inside should be included in the navmesh.
	///
	/// See: Pathfinding.RecastGraph.relevantGraphSurfaceMode
	/// </summary>
	[AddComponentMenu("Pathfinding/Navmesh/RelevantGraphSurface")]
	[HelpURL("http://arongranberg.com/astar/docs/class_pathfinding_1_1_relevant_graph_surface.php")]
	public class RelevantGraphSurface : VersionedMonoBehaviour {
		/// <summary>
		/// The root
		/// </summary>
		private static RelevantGraphSurface root;

		/// <summary>
		/// The max range
		/// </summary>
		public float maxRange = 1;

		/// <summary>
		/// The prev
		/// </summary>
		private RelevantGraphSurface prev;
		/// <summary>
		/// The next
		/// </summary>
		private RelevantGraphSurface next;
		/// <summary>
		/// The position
		/// </summary>
		private Vector3 position;

		/// <summary>
		/// Gets the value of the position
		/// </summary>
		public Vector3 Position {
			get { return position; }
		}

		/// <summary>
		/// Gets the value of the next
		/// </summary>
		public RelevantGraphSurface Next {
			get { return next; }
		}

		/// <summary>
		/// Gets the value of the prev
		/// </summary>
		public RelevantGraphSurface Prev {
			get { return prev; }
		}

		/// <summary>
		/// Gets the value of the root
		/// </summary>
		public static RelevantGraphSurface Root {
			get { return root; }
		}

		/// <summary>
		/// Updates the position
		/// </summary>
		public void UpdatePosition () {
			position = transform.position;
		}

		/// <summary>
		/// Ons the enable
		/// </summary>
		void OnEnable () {
			UpdatePosition();
			if (root == null) {
				root = this;
			} else {
				next = root;
				root.prev = this;
				root = this;
			}
		}

		/// <summary>
		/// Ons the disable
		/// </summary>
		void OnDisable () {
			if (root == this) {
				root = next;
				if (root != null) root.prev = null;
			} else {
				if (prev != null) prev.next = next;
				if (next != null) next.prev = prev;
			}
			prev = null;
			next = null;
		}

		/// <summary>
		/// Updates the positions of all relevant graph surface components.
		/// Required to be able to use the position property reliably.
		/// </summary>
		public static void UpdateAllPositions () {
			RelevantGraphSurface c = root;

			while (c != null) { c.UpdatePosition(); c = c.Next; }
		}

		/// <summary>
		/// Finds the all graph surfaces
		/// </summary>
		public static void FindAllGraphSurfaces () {
			var srf = GameObject.FindObjectsOfType(typeof(RelevantGraphSurface)) as RelevantGraphSurface[];

			for (int i = 0; i < srf.Length; i++) {
				srf[i].OnDisable();
				srf[i].OnEnable();
			}
		}

		/// <summary>
		/// Ons the draw gizmos
		/// </summary>
		public void OnDrawGizmos () {
			Gizmos.color = new Color(57/255f, 211/255f, 46/255f, 0.4f);
			Gizmos.DrawLine(transform.position - Vector3.up*maxRange, transform.position + Vector3.up*maxRange);
		}

		/// <summary>
		/// Ons the draw gizmos selected
		/// </summary>
		public void OnDrawGizmosSelected () {
			Gizmos.color = new Color(57/255f, 211/255f, 46/255f);
			Gizmos.DrawLine(transform.position - Vector3.up*maxRange, transform.position + Vector3.up*maxRange);
		}
	}
}
