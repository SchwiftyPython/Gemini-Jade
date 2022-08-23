using System.Collections.Generic;

namespace Pathfinding {
	using Pathfinding.Util;

	/// <summary>
	/// Represents a collection of GraphNodes.
	/// It allows for fast lookups of the closest node to a point.
	///
	/// See: https://en.wikipedia.org/wiki/K-d_tree
	/// </summary>
	public class PointKDTree {
		// TODO: Make constant
		/// <summary>
		/// The leaf size
		/// </summary>
		public const int LeafSize = 10;
		/// <summary>
		/// The leaf size
		/// </summary>
		public const int LeafArraySize = LeafSize*2 + 1;

		/// <summary>
		/// The node
		/// </summary>
		Node[] tree = new Node[16];

		/// <summary>
		/// The num nodes
		/// </summary>
		int numNodes = 0;

		/// <summary>
		/// The graph node
		/// </summary>
		readonly List<GraphNode> largeList = new List<GraphNode>();
		/// <summary>
		/// The graph node
		/// </summary>
		readonly Stack<GraphNode[]> arrayCache = new Stack<GraphNode[]>();
		/// <summary>
		/// The compare
		/// </summary>
		static readonly IComparer<GraphNode>[] comparers = new IComparer<GraphNode>[] { new CompareX(), new CompareY(), new CompareZ() };

		/// <summary>
		/// The node
		/// </summary>
		struct Node {
			/// <summary>Nodes in this leaf node (null if not a leaf node)</summary>
			public GraphNode[] data;
			/// <summary>Split point along the <see cref="splitAxis"/> if not a leaf node</summary>
			public int split;
			/// <summary>Number of non-null entries in <see cref="data"/></summary>
			public ushort count;
			/// <summary>Axis to split along if not a leaf node (x=0, y=1, z=2)</summary>
			public byte splitAxis;
		}

		// Pretty ugly with one class for each axis, but it has been verified to make the tree around 5% faster
		/// <summary>
		/// The compare class
		/// </summary>
		/// <seealso cref="IComparer{GraphNode}"/>
		class CompareX : IComparer<GraphNode> {
			/// <summary>
			/// Compares the lhs
			/// </summary>
			/// <param name="lhs">The lhs</param>
			/// <param name="rhs">The rhs</param>
			/// <returns>The int</returns>
			public int Compare (GraphNode lhs, GraphNode rhs) { return lhs.position.x.CompareTo(rhs.position.x); }
		}

		/// <summary>
		/// The compare class
		/// </summary>
		/// <seealso cref="IComparer{GraphNode}"/>
		class CompareY : IComparer<GraphNode> {
			/// <summary>
			/// Compares the lhs
			/// </summary>
			/// <param name="lhs">The lhs</param>
			/// <param name="rhs">The rhs</param>
			/// <returns>The int</returns>
			public int Compare (GraphNode lhs, GraphNode rhs) { return lhs.position.y.CompareTo(rhs.position.y); }
		}

		/// <summary>
		/// The compare class
		/// </summary>
		/// <seealso cref="IComparer{GraphNode}"/>
		class CompareZ : IComparer<GraphNode> {
			/// <summary>
			/// Compares the lhs
			/// </summary>
			/// <param name="lhs">The lhs</param>
			/// <param name="rhs">The rhs</param>
			/// <returns>The int</returns>
			public int Compare (GraphNode lhs, GraphNode rhs) { return lhs.position.z.CompareTo(rhs.position.z); }
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PointKDTree"/> class
		/// </summary>
		public PointKDTree() {
			tree[1] = new Node { data = GetOrCreateList() };
		}

		/// <summary>Add the node to the tree</summary>
		public void Add (GraphNode node) {
			numNodes++;
			Add(node, 1);
		}

		/// <summary>Rebuild the tree starting with all nodes in the array between index start (inclusive) and end (exclusive)</summary>
		public void Rebuild (GraphNode[] nodes, int start, int end) {
			if (start < 0 || end < start || end > nodes.Length)
				throw new System.ArgumentException();

			for (int i = 0; i < tree.Length; i++) {
				var data = tree[i].data;
				if (data != null) {
					for (int j = 0; j < LeafArraySize; j++) data[j] = null;
					arrayCache.Push(data);
					tree[i].data = null;
				}
			}

			numNodes = end - start;
			Build(1, new List<GraphNode>(nodes), start, end);
		}

		/// <summary>
		/// Gets the or create list
		/// </summary>
		/// <returns>The graph node array</returns>
		GraphNode[] GetOrCreateList () {
			// Note, the lists will never become larger than this initial capacity, so possibly they should be replaced by arrays
			return arrayCache.Count > 0 ? arrayCache.Pop() : new GraphNode[LeafArraySize];
		}

		/// <summary>
		/// Sizes the index
		/// </summary>
		/// <param name="index">The index</param>
		/// <returns>The int</returns>
		int Size (int index) {
			return tree[index].data != null ? tree[index].count : Size(2 * index) + Size(2 * index + 1);
		}

		/// <summary>
		/// Collects the and clear using the specified index
		/// </summary>
		/// <param name="index">The index</param>
		/// <param name="buffer">The buffer</param>
		void CollectAndClear (int index, List<GraphNode> buffer) {
			var nodes = tree[index].data;
			var count = tree[index].count;

			if (nodes != null) {
				tree[index] = new Node();
				for (int i = 0; i < count; i++) {
					buffer.Add(nodes[i]);
					nodes[i] = null;
				}
				arrayCache.Push(nodes);
			} else {
				CollectAndClear(index*2, buffer);
				CollectAndClear(index*2 + 1, buffer);
			}
		}

		/// <summary>
		/// Maxes the allowed size using the specified num nodes
		/// </summary>
		/// <param name="numNodes">The num nodes</param>
		/// <param name="depth">The depth</param>
		/// <returns>The int</returns>
		static int MaxAllowedSize (int numNodes, int depth) {
			// Allow a node to be 2.5 times as full as it should ideally be
			// but do not allow it to contain more than 3/4ths of the total number of nodes
			// (important to make sure nodes near the top of the tree also get rebalanced).
			// A node should ideally contain numNodes/(2^depth) nodes below it (^ is exponentiation, not xor)
			return System.Math.Min(((5 * numNodes) / 2) >> depth, (3 * numNodes) / 4);
		}

		/// <summary>
		/// Rebalances the index
		/// </summary>
		/// <param name="index">The index</param>
		void Rebalance (int index) {
			CollectAndClear(index, largeList);
			Build(index, largeList, 0, largeList.Count);
			largeList.ClearFast();
		}

		/// <summary>
		/// Ensures the size using the specified index
		/// </summary>
		/// <param name="index">The index</param>
		void EnsureSize (int index) {
			if (index >= tree.Length) {
				var newLeaves = new Node[System.Math.Max(index + 1, tree.Length*2)];
				tree.CopyTo(newLeaves, 0);
				tree = newLeaves;
			}
		}

		/// <summary>
		/// Builds the index
		/// </summary>
		/// <param name="index">The index</param>
		/// <param name="nodes">The nodes</param>
		/// <param name="start">The start</param>
		/// <param name="end">The end</param>
		void Build (int index, List<GraphNode> nodes, int start, int end) {
			EnsureSize(index);
			if (end - start <= LeafSize) {
				var leafData = tree[index].data = GetOrCreateList();
				tree[index].count = (ushort)(end - start);
				for (int i = start; i < end; i++)
					leafData[i - start] = nodes[i];
			} else {
				Int3 mn, mx;
				mn = mx = nodes[start].position;
				for (int i = start; i < end; i++) {
					var p = nodes[i].position;
					mn = new Int3(System.Math.Min(mn.x, p.x), System.Math.Min(mn.y, p.y), System.Math.Min(mn.z, p.z));
					mx = new Int3(System.Math.Max(mx.x, p.x), System.Math.Max(mx.y, p.y), System.Math.Max(mx.z, p.z));
				}
				Int3 diff = mx - mn;
				var axis = diff.x > diff.y ? (diff.x > diff.z ? 0 : 2) : (diff.y > diff.z ? 1 : 2);

				nodes.Sort(start, end - start, comparers[axis]);
				int mid = (start+end)/2;
				tree[index].split = (nodes[mid-1].position[axis] + nodes[mid].position[axis] + 1)/2;
				tree[index].splitAxis = (byte)axis;
				Build(index*2 + 0, nodes, start, mid);
				Build(index*2 + 1, nodes, mid, end);
			}
		}

		/// <summary>
		/// Adds the point
		/// </summary>
		/// <param name="point">The point</param>
		/// <param name="index">The index</param>
		/// <param name="depth">The depth</param>
		void Add (GraphNode point, int index, int depth = 0) {
			// Move down in the tree until the leaf node is found that this point is inside of
			while (tree[index].data == null) {
				index = 2 * index + (point.position[tree[index].splitAxis] < tree[index].split ? 0 : 1);
				depth++;
			}

			// Add the point to the leaf node
			tree[index].data[tree[index].count++] = point;

			// Check if the leaf node is large enough that we need to do some rebalancing
			if (tree[index].count >= LeafArraySize) {
				int levelsUp = 0;

				// Search upwards for nodes that are too large and should be rebalanced
				// Rebalance the node above the node that had a too large size so that it can
				// move children over to the sibling
				while (depth - levelsUp > 0 && Size(index >> levelsUp) > MaxAllowedSize(numNodes, depth-levelsUp)) {
					levelsUp++;
				}

				Rebalance(index >> levelsUp);
			}
		}

		/// <summary>Closest node to the point which satisfies the constraint</summary>
		public GraphNode GetNearest (Int3 point, NNConstraint constraint) {
			GraphNode best = null;
			long bestSqrDist = long.MaxValue;

			GetNearestInternal(1, point, constraint, ref best, ref bestSqrDist);
			return best;
		}

		/// <summary>
		/// Gets the nearest internal using the specified index
		/// </summary>
		/// <param name="index">The index</param>
		/// <param name="point">The point</param>
		/// <param name="constraint">The constraint</param>
		/// <param name="best">The best</param>
		/// <param name="bestSqrDist">The best sqr dist</param>
		void GetNearestInternal (int index, Int3 point, NNConstraint constraint, ref GraphNode best, ref long bestSqrDist) {
			var data = tree[index].data;

			if (data != null) {
				for (int i = tree[index].count - 1; i >= 0; i--) {
					var dist = (data[i].position - point).sqrMagnitudeLong;
					if (dist < bestSqrDist && (constraint == null || constraint.Suitable(data[i]))) {
						bestSqrDist = dist;
						best = data[i];
					}
				}
			} else {
				var dist = (long)(point[tree[index].splitAxis] - tree[index].split);
				var childIndex = 2 * index + (dist < 0 ? 0 : 1);
				GetNearestInternal(childIndex, point, constraint, ref best, ref bestSqrDist);

				// Try the other one if it is possible to find a valid node on the other side
				if (dist*dist < bestSqrDist) {
					// childIndex ^ 1 will flip the last bit, so if childIndex is odd, then childIndex ^ 1 will be even
					GetNearestInternal(childIndex ^ 0x1, point, constraint, ref best, ref bestSqrDist);
				}
			}
		}

		/// <summary>Closest node to the point which satisfies the constraint</summary>
		public GraphNode GetNearestConnection (Int3 point, NNConstraint constraint, long maximumSqrConnectionLength) {
			GraphNode best = null;
			long bestSqrDist = long.MaxValue;

			// Given a found point at a distance of r world units
			// then any node that has a connection on which a closer point lies must have a squared distance lower than
			// d^2 < (maximumConnectionLength/2)^2 + r^2
			// Note: (x/2)^2 = (x^2)/4
			// Note: (x+3)/4 to round up
			long offset = (maximumSqrConnectionLength+3)/4;

			GetNearestConnectionInternal(1, point, constraint, ref best, ref bestSqrDist, offset);
			return best;
		}

		/// <summary>
		/// Gets the nearest connection internal using the specified index
		/// </summary>
		/// <param name="index">The index</param>
		/// <param name="point">The point</param>
		/// <param name="constraint">The constraint</param>
		/// <param name="best">The best</param>
		/// <param name="bestSqrDist">The best sqr dist</param>
		/// <param name="distanceThresholdOffset">The distance threshold offset</param>
		void GetNearestConnectionInternal (int index, Int3 point, NNConstraint constraint, ref GraphNode best, ref long bestSqrDist, long distanceThresholdOffset) {
			var data = tree[index].data;

			if (data != null) {
				var pointv3 = (UnityEngine.Vector3)point;
				for (int i = tree[index].count - 1; i >= 0; i--) {
					var dist = (data[i].position - point).sqrMagnitudeLong;
					// Note: the subtraction is important. If we used an addition on the RHS instead the result might overflow as bestSqrDist starts as long.MaxValue
					if (dist - distanceThresholdOffset < bestSqrDist && (constraint == null || constraint.Suitable(data[i]))) {
						// This node may contains the closest connection
						// Check all connections
						var conns = (data[i] as PointNode).connections;
						if (conns != null) {
							var nodePos = (UnityEngine.Vector3)data[i].position;
							for (int j = 0; j < conns.Length; j++) {
								// Find the closest point on the connection, but only on this node's side of the connection
								// This ensures that we will find the closest node with the closest connection.
								var connectionMidpoint = ((UnityEngine.Vector3)conns[j].node.position + nodePos) * 0.5f;
								float sqrConnectionDistance = VectorMath.SqrDistancePointSegment(nodePos, connectionMidpoint, pointv3);
								// Convert to Int3 space
								long sqrConnectionDistanceInt = (long)(sqrConnectionDistance*Int3.FloatPrecision*Int3.FloatPrecision);
								if (sqrConnectionDistanceInt < bestSqrDist) {
									bestSqrDist = sqrConnectionDistanceInt;
									best = data[i];
								}
							}
						}

						// Also check if the node itself is close enough.
						// This is important if the node has no connections at all.
						if (dist < bestSqrDist) {
							bestSqrDist = dist;
							best = data[i];
						}
					}
				}
			} else {
				var dist = (long)(point[tree[index].splitAxis] - tree[index].split);
				var childIndex = 2 * index + (dist < 0 ? 0 : 1);
				GetNearestConnectionInternal(childIndex, point, constraint, ref best, ref bestSqrDist, distanceThresholdOffset);

				// Try the other one if it is possible to find a valid node on the other side
				// Note: the subtraction is important. If we used an addition on the RHS instead the result might overflow as bestSqrDist starts as long.MaxValue
				if (dist*dist - distanceThresholdOffset < bestSqrDist) {
					// childIndex ^ 1 will flip the last bit, so if childIndex is odd, then childIndex ^ 1 will be even
					GetNearestConnectionInternal(childIndex ^ 0x1, point, constraint, ref best, ref bestSqrDist, distanceThresholdOffset);
				}
			}
		}

		/// <summary>Add all nodes within a squared distance of the point to the buffer.</summary>
		/// <param name="point">Nodes around this point will be added to the buffer.</param>
		/// <param name="sqrRadius">squared maximum distance in Int3 space. If you are converting from world space you will need to multiply by Int3.Precision:
		/// <code> var sqrRadius = (worldSpaceRadius * Int3.Precision) * (worldSpaceRadius * Int3.Precision); </code></param>
		/// <param name="buffer">All nodes will be added to this list.</param>
		public void GetInRange (Int3 point, long sqrRadius, List<GraphNode> buffer) {
			GetInRangeInternal(1, point, sqrRadius, buffer);
		}

		/// <summary>
		/// Gets the in range internal using the specified index
		/// </summary>
		/// <param name="index">The index</param>
		/// <param name="point">The point</param>
		/// <param name="sqrRadius">The sqr radius</param>
		/// <param name="buffer">The buffer</param>
		void GetInRangeInternal (int index, Int3 point, long sqrRadius, List<GraphNode> buffer) {
			var data = tree[index].data;

			if (data != null) {
				for (int i = tree[index].count - 1; i >= 0; i--) {
					var dist = (data[i].position - point).sqrMagnitudeLong;
					if (dist < sqrRadius) {
						buffer.Add(data[i]);
					}
				}
			} else {
				var dist = (long)(point[tree[index].splitAxis] - tree[index].split);
				// Pick the first child to enter based on which side of the splitting line the point is
				var childIndex = 2 * index + (dist < 0 ? 0 : 1);
				GetInRangeInternal(childIndex, point, sqrRadius, buffer);

				// Try the other one if it is possible to find a valid node on the other side
				if (dist*dist < sqrRadius) {
					// childIndex ^ 1 will flip the last bit, so if childIndex is odd, then childIndex ^ 1 will be even
					GetInRangeInternal(childIndex ^ 0x1, point, sqrRadius, buffer);
				}
			}
		}
	}
}
