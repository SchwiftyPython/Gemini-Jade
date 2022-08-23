using UnityEngine;

namespace Pathfinding.Util {
	/// <summary>
	/// The graph gizmo helper class
	/// </summary>
	/// <seealso cref="IAstarPooledObject"/>
	/// <seealso cref="System.IDisposable"/>
	public class GraphGizmoHelper : IAstarPooledObject, System.IDisposable {
		/// <summary>
		/// Gets or sets the value of the hasher
		/// </summary>
		public RetainedGizmos.Hasher hasher { get; private set; }
		/// <summary>
		/// The gizmos
		/// </summary>
		Pathfinding.Util.RetainedGizmos gizmos;
		/// <summary>
		/// The debug data
		/// </summary>
		PathHandler debugData;
		/// <summary>
		/// The debug path id
		/// </summary>
		ushort debugPathID;
		/// <summary>
		/// The debug mode
		/// </summary>
		GraphDebugMode debugMode;
		/// <summary>
		/// The show search tree
		/// </summary>
		bool showSearchTree;
		/// <summary>
		/// The debug floor
		/// </summary>
		float debugFloor;
		/// <summary>
		/// The debug roof
		/// </summary>
		float debugRoof;
		/// <summary>
		/// Gets or sets the value of the builder
		/// </summary>
		public RetainedGizmos.Builder builder { get; private set; }
		/// <summary>
		/// The draw connection start
		/// </summary>
		Vector3 drawConnectionStart;
		/// <summary>
		/// The draw connection color
		/// </summary>
		Color drawConnectionColor;
		/// <summary>
		/// The draw connection
		/// </summary>
		readonly System.Action<GraphNode> drawConnection;

		/// <summary>
		/// Initializes a new instance of the <see cref="GraphGizmoHelper"/> class
		/// </summary>
		public GraphGizmoHelper () {
			// Cache a delegate to avoid allocating memory for it every time
			drawConnection = DrawConnection;
		}

		/// <summary>
		/// Inits the active
		/// </summary>
		/// <param name="active">The active</param>
		/// <param name="hasher">The hasher</param>
		/// <param name="gizmos">The gizmos</param>
		public void Init (AstarPath active, RetainedGizmos.Hasher hasher, RetainedGizmos gizmos) {
			if (active != null) {
				debugData = active.debugPathData;
				debugPathID = active.debugPathID;
				debugMode = active.debugMode;
				debugFloor = active.debugFloor;
				debugRoof = active.debugRoof;
				showSearchTree = active.showSearchTree && debugData != null;
			}
			this.gizmos = gizmos;
			this.hasher = hasher;
			builder = ObjectPool<RetainedGizmos.Builder>.Claim();
		}

		/// <summary>
		/// Ons the enter pool
		/// </summary>
		public void OnEnterPool () {
			// Will cause pretty much all calls to throw null ref exceptions until Init is called
			var bld = builder;

			ObjectPool<RetainedGizmos.Builder>.Release(ref bld);
			builder = null;
			debugData = null;
		}

		/// <summary>
		/// Draws the connections using the specified node
		/// </summary>
		/// <param name="node">The node</param>
		public void DrawConnections (GraphNode node) {
			if (showSearchTree) {
				if (InSearchTree(node, debugData, debugPathID)) {
					var pnode = debugData.GetPathNode(node);
					if (pnode.parent != null) {
						builder.DrawLine((Vector3)node.position, (Vector3)debugData.GetPathNode(node).parent.node.position, NodeColor(node));
					}
				}
			} else {
				// Calculate which color to use for drawing the node
				// based on the settings specified in the editor
				drawConnectionColor = NodeColor(node);
				// Get the node position
				// Cast it here to avoid doing it for every neighbour
				drawConnectionStart = (Vector3)node.position;
				node.GetConnections(drawConnection);
			}
		}

		/// <summary>
		/// Draws the connection using the specified other
		/// </summary>
		/// <param name="other">The other</param>
		void DrawConnection (GraphNode other) {
			builder.DrawLine(drawConnectionStart, Vector3.Lerp((Vector3)other.position, drawConnectionStart, 0.5f), drawConnectionColor);
		}

		/// <summary>
		/// Color to use for gizmos.
		/// Returns a color to be used for the specified node with the current debug settings (editor only).
		///
		/// Version: Since 3.6.1 this method will not handle null nodes
		/// </summary>
		public Color NodeColor (GraphNode node) {
			if (showSearchTree && !InSearchTree(node, debugData, debugPathID)) return Color.clear;

			Color color;

			if (node.Walkable) {
				switch (debugMode) {
				case GraphDebugMode.Areas:
					color = AstarColor.GetAreaColor(node.Area);
					break;
				case GraphDebugMode.HierarchicalNode:
					color = AstarColor.GetTagColor((uint)node.HierarchicalNodeIndex);
					break;
				case GraphDebugMode.Penalty:
					color = Color.Lerp(AstarColor.ConnectionLowLerp, AstarColor.ConnectionHighLerp, ((float)node.Penalty-debugFloor) / (debugRoof-debugFloor));
					break;
				case GraphDebugMode.Tags:
					color = AstarColor.GetTagColor(node.Tag);
					break;
				case GraphDebugMode.SolidColor:
					color = AstarColor.SolidColor;
					break;
				default:
					if (debugData == null) {
						color = AstarColor.SolidColor;
						break;
					}

					PathNode pathNode = debugData.GetPathNode(node);
					float value;
					if (debugMode == GraphDebugMode.G) {
						value = pathNode.G;
					} else if (debugMode == GraphDebugMode.H) {
						value = pathNode.H;
					} else {
						// mode == F
						value = pathNode.F;
					}

					color = Color.Lerp(AstarColor.ConnectionLowLerp, AstarColor.ConnectionHighLerp, (value-debugFloor) / (debugRoof-debugFloor));
					break;
				}
			} else {
				color = AstarColor.UnwalkableNode;
			}

			return color;
		}

		/// <summary>
		/// Returns if the node is in the search tree of the path.
		/// Only guaranteed to be correct if path is the latest path calculated.
		/// Use for gizmo drawing only.
		/// </summary>
		public static bool InSearchTree (GraphNode node, PathHandler handler, ushort pathID) {
			return handler.GetPathNode(node).pathID == pathID;
		}

		/// <summary>
		/// Draws the wire triangle using the specified a
		/// </summary>
		/// <param name="a">The </param>
		/// <param name="b">The </param>
		/// <param name="c">The </param>
		/// <param name="color">The color</param>
		public void DrawWireTriangle (Vector3 a, Vector3 b, Vector3 c, Color color) {
			builder.DrawLine(a, b, color);
			builder.DrawLine(b, c, color);
			builder.DrawLine(c, a, color);
		}

		/// <summary>
		/// Draws the triangles using the specified vertices
		/// </summary>
		/// <param name="vertices">The vertices</param>
		/// <param name="colors">The colors</param>
		/// <param name="numTriangles">The num triangles</param>
		public void DrawTriangles (Vector3[] vertices, Color[] colors, int numTriangles) {
			var triangles = ListPool<int>.Claim(numTriangles);

			for (int i = 0; i < numTriangles*3; i++) triangles.Add(i);
			builder.DrawMesh(gizmos, vertices, triangles, colors);
			ListPool<int>.Release(ref triangles);
		}

		/// <summary>
		/// Draws the wire triangles using the specified vertices
		/// </summary>
		/// <param name="vertices">The vertices</param>
		/// <param name="colors">The colors</param>
		/// <param name="numTriangles">The num triangles</param>
		public void DrawWireTriangles (Vector3[] vertices, Color[] colors, int numTriangles) {
			for (int i = 0; i < numTriangles; i++) {
				DrawWireTriangle(vertices[i*3+0], vertices[i*3+1], vertices[i*3+2], colors[i*3+0]);
			}
		}

		/// <summary>
		/// Submits this instance
		/// </summary>
		public void Submit () {
			builder.Submit(gizmos, hasher);
		}

		/// <summary>
		/// Disposes this instance
		/// </summary>
		void System.IDisposable.Dispose () {
			var tmp = this;

			Submit();
			ObjectPool<GraphGizmoHelper>.Release(ref tmp);
		}
	}
}
