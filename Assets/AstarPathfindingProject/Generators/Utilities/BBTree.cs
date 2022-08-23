//#define ASTARDEBUG   //"BBTree Debug" If enables, some queries to the tree will show debug lines. Turn off multithreading when using this since DrawLine calls cannot be called from a different thread

using System;
using UnityEngine;

namespace Pathfinding {
	using Pathfinding.Util;

	/// <summary>
	/// Axis Aligned Bounding Box Tree.
	/// Holds a bounding box tree of triangles.
	/// </summary>
	public class BBTree : IAstarPooledObject {
		/// <summary>Holds all tree nodes</summary>
		BBTreeBox[] tree = null;
		/// <summary>
		/// The node lookup
		/// </summary>
		TriangleMeshNode[] nodeLookup = null;
		/// <summary>
		/// The count
		/// </summary>
		int count;
		/// <summary>
		/// The leaf nodes
		/// </summary>
		int leafNodes;

		/// <summary>
		/// The maximum leaf size
		/// </summary>
		const int MaximumLeafSize = 4;

		/// <summary>
		/// Gets the value of the size
		/// </summary>
		public Rect Size {
			get {
				if (count == 0) {
					return new Rect(0, 0, 0, 0);
				} else {
					var rect = tree[0].rect;
					return Rect.MinMaxRect(rect.xmin*Int3.PrecisionFactor, rect.ymin*Int3.PrecisionFactor, rect.xmax*Int3.PrecisionFactor, rect.ymax*Int3.PrecisionFactor);
				}
			}
		}

		/// <summary>
		/// Clear the tree.
		/// Note that references to old nodes will still be intact so the GC cannot immediately collect them.
		/// </summary>
		public void Clear () {
			count = 0;
			leafNodes = 0;
			if (tree != null) ArrayPool<BBTreeBox>.Release(ref tree);
			if (nodeLookup != null) {
				// Prevent memory leaks as the pool does not clear the array
				for (int i = 0; i < nodeLookup.Length; i++) nodeLookup[i] = null;
				ArrayPool<TriangleMeshNode>.Release(ref nodeLookup);
			}
			tree = ArrayPool<BBTreeBox>.Claim(0);
			nodeLookup = ArrayPool<TriangleMeshNode>.Claim(0);
		}

		/// <summary>
		/// Ons the enter pool
		/// </summary>
		void IAstarPooledObject.OnEnterPool () {
			Clear();
		}

		/// <summary>
		/// Ensures the capacity using the specified c
		/// </summary>
		/// <param name="c">The </param>
		void EnsureCapacity (int c) {
			if (c > tree.Length) {
				var newArr = ArrayPool<BBTreeBox>.Claim(c);
				tree.CopyTo(newArr, 0);
				ArrayPool<BBTreeBox>.Release(ref tree);
				tree = newArr;
			}
		}

		/// <summary>
		/// Ensures the node capacity using the specified c
		/// </summary>
		/// <param name="c">The </param>
		void EnsureNodeCapacity (int c) {
			if (c > nodeLookup.Length) {
				var newArr = ArrayPool<TriangleMeshNode>.Claim(c);
				nodeLookup.CopyTo(newArr, 0);
				ArrayPool<TriangleMeshNode>.Release(ref nodeLookup);
				nodeLookup = newArr;
			}
		}

		/// <summary>
		/// Gets the box using the specified rect
		/// </summary>
		/// <param name="rect">The rect</param>
		/// <returns>The int</returns>
		int GetBox (IntRect rect) {
			if (count >= tree.Length) EnsureCapacity(count+1);

			tree[count] = new BBTreeBox(rect);
			count++;
			return count-1;
		}

		/// <summary>Rebuilds the tree using the specified nodes</summary>
		public void RebuildFrom (TriangleMeshNode[] nodes) {
			Clear();

			if (nodes.Length == 0) return;

			// We will use approximately 2N tree nodes
			EnsureCapacity(Mathf.CeilToInt(nodes.Length * 2.1f));
			// We will use approximately N node references
			EnsureNodeCapacity(Mathf.CeilToInt(nodes.Length * 1.1f));

			// This will store the order of the nodes while the tree is being built
			// It turns out that it is a lot faster to do this than to actually modify
			// the nodes and nodeBounds arrays (presumably since that involves shuffling
			// around 20 bytes of memory (sizeof(pointer) + sizeof(IntRect)) per node
			// instead of 4 bytes (sizeof(int)).
			// It also means we don't have to make a copy of the nodes array since
			// we do not modify it
			var permutation = ArrayPool<int>.Claim(nodes.Length);
			for (int i = 0; i < nodes.Length; i++) {
				permutation[i] = i;
			}

			// Precalculate the bounds of the nodes in XZ space.
			// It turns out that calculating the bounds is a bottleneck and precalculating
			// the bounds makes it around 3 times faster to build a tree
			var nodeBounds = ArrayPool<IntRect>.Claim(nodes.Length);
			for (int i = 0; i < nodes.Length; i++) {
				Int3 v0, v1, v2;
				nodes[i].GetVertices(out v0, out v1, out v2);

				var rect = new IntRect(v0.x, v0.z, v0.x, v0.z);
				rect = rect.ExpandToContain(v1.x, v1.z);
				rect = rect.ExpandToContain(v2.x, v2.z);
				nodeBounds[i] = rect;
			}

			RebuildFromInternal(nodes, permutation, nodeBounds, 0, nodes.Length, false);

			ArrayPool<int>.Release(ref permutation);
			ArrayPool<IntRect>.Release(ref nodeBounds);
		}

		/// <summary>
		/// Splits the by x using the specified nodes
		/// </summary>
		/// <param name="nodes">The nodes</param>
		/// <param name="permutation">The permutation</param>
		/// <param name="from">The from</param>
		/// <param name="to">The to</param>
		/// <param name="divider">The divider</param>
		/// <returns>The mx</returns>
		static int SplitByX (TriangleMeshNode[] nodes, int[] permutation, int from, int to, int divider) {
			int mx = to;

			for (int i = from; i < mx; i++) {
				if (nodes[permutation[i]].position.x > divider) {
					mx--;
					// Swap items i and mx
					var tmp = permutation[mx];
					permutation[mx] = permutation[i];
					permutation[i] = tmp;
					i--;
				}
			}
			return mx;
		}

		/// <summary>
		/// Splits the by z using the specified nodes
		/// </summary>
		/// <param name="nodes">The nodes</param>
		/// <param name="permutation">The permutation</param>
		/// <param name="from">The from</param>
		/// <param name="to">The to</param>
		/// <param name="divider">The divider</param>
		/// <returns>The mx</returns>
		static int SplitByZ (TriangleMeshNode[] nodes, int[] permutation, int from, int to, int divider) {
			int mx = to;

			for (int i = from; i < mx; i++) {
				if (nodes[permutation[i]].position.z > divider) {
					mx--;
					// Swap items i and mx
					var tmp = permutation[mx];
					permutation[mx] = permutation[i];
					permutation[i] = tmp;
					i--;
				}
			}
			return mx;
		}

		/// <summary>
		/// Rebuilds the from internal using the specified nodes
		/// </summary>
		/// <param name="nodes">The nodes</param>
		/// <param name="permutation">The permutation</param>
		/// <param name="nodeBounds">The node bounds</param>
		/// <param name="from">The from</param>
		/// <param name="to">The to</param>
		/// <param name="odd">The odd</param>
		/// <returns>The box</returns>
		int RebuildFromInternal (TriangleMeshNode[] nodes, int[] permutation, IntRect[] nodeBounds, int from, int to, bool odd) {
			var rect = NodeBounds(permutation, nodeBounds, from, to);
			int box = GetBox(rect);

			if (to - from <= MaximumLeafSize) {
				var nodeOffset = tree[box].nodeOffset = leafNodes*MaximumLeafSize;
				EnsureNodeCapacity(nodeOffset + MaximumLeafSize);
				leafNodes++;
				// Assign all nodes to the array. Note that we also need clear unused slots as the array from the pool may contain any information
				for (int i = 0; i < MaximumLeafSize; i++) {
					nodeLookup[nodeOffset + i] = i < to - from ? nodes[permutation[from + i]] : null;
				}
				return box;
			}

			int splitIndex;
			if (odd) {
				// X
				int divider = (rect.xmin + rect.xmax)/2;
				splitIndex = SplitByX(nodes, permutation, from, to, divider);
			} else {
				// Y/Z
				int divider = (rect.ymin + rect.ymax)/2;
				splitIndex = SplitByZ(nodes, permutation, from, to, divider);
			}

			if (splitIndex == from || splitIndex == to) {
				// All nodes were on one side of the divider
				// Try to split along the other axis

				if (!odd) {
					// X
					int divider = (rect.xmin + rect.xmax)/2;
					splitIndex = SplitByX(nodes, permutation, from, to, divider);
				} else {
					// Y/Z
					int divider = (rect.ymin + rect.ymax)/2;
					splitIndex = SplitByZ(nodes, permutation, from, to, divider);
				}

				if (splitIndex == from || splitIndex == to) {
					// All nodes were on one side of the divider
					// Just pick one half
					splitIndex = (from+to)/2;
				}
			}

			tree[box].left = RebuildFromInternal(nodes, permutation, nodeBounds, from, splitIndex, !odd);
			tree[box].right = RebuildFromInternal(nodes, permutation, nodeBounds, splitIndex, to, !odd);

			return box;
		}

		/// <summary>Calculates the bounding box in XZ space of all nodes between from (inclusive) and to (exclusive)</summary>
		static IntRect NodeBounds (int[] permutation, IntRect[] nodeBounds, int from, int to) {
			var rect = nodeBounds[permutation[from]];

			for (int j = from + 1; j < to; j++) {
				var otherRect = nodeBounds[permutation[j]];

				// Equivalent to rect = IntRect.Union(rect, otherRect)
				// but manually inlining is approximately
				// 25% faster when building an entire tree.
				// This code is hot when using navmesh cutting.
				rect.xmin = Math.Min(rect.xmin, otherRect.xmin);
				rect.ymin = Math.Min(rect.ymin, otherRect.ymin);
				rect.xmax = Math.Max(rect.xmax, otherRect.xmax);
				rect.ymax = Math.Max(rect.ymax, otherRect.ymax);
			}

			return rect;
		}

		/// <summary>
		/// Draws the debug rect using the specified rect
		/// </summary>
		/// <param name="rect">The rect</param>
		[System.Diagnostics.Conditional("ASTARDEBUG")]
		static void DrawDebugRect (IntRect rect) {
			Debug.DrawLine(new Vector3(rect.xmin, 0, rect.ymin), new Vector3(rect.xmax, 0, rect.ymin), Color.white);
			Debug.DrawLine(new Vector3(rect.xmin, 0, rect.ymax), new Vector3(rect.xmax, 0, rect.ymax), Color.white);
			Debug.DrawLine(new Vector3(rect.xmin, 0, rect.ymin), new Vector3(rect.xmin, 0, rect.ymax), Color.white);
			Debug.DrawLine(new Vector3(rect.xmax, 0, rect.ymin), new Vector3(rect.xmax, 0, rect.ymax), Color.white);
		}

		/// <summary>
		/// Draws the debug node using the specified node
		/// </summary>
		/// <param name="node">The node</param>
		/// <param name="yoffset">The yoffset</param>
		/// <param name="color">The color</param>
		[System.Diagnostics.Conditional("ASTARDEBUG")]
		static void DrawDebugNode (TriangleMeshNode node, float yoffset, Color color) {
			Debug.DrawLine((Vector3)node.GetVertex(1) + Vector3.up*yoffset, (Vector3)node.GetVertex(2) + Vector3.up*yoffset, color);
			Debug.DrawLine((Vector3)node.GetVertex(0) + Vector3.up*yoffset, (Vector3)node.GetVertex(1) + Vector3.up*yoffset, color);
			Debug.DrawLine((Vector3)node.GetVertex(2) + Vector3.up*yoffset, (Vector3)node.GetVertex(0) + Vector3.up*yoffset, color);
		}

		/// <summary>
		/// Queries the tree for the closest node to p constrained by the NNConstraint.
		/// Note that this function will only fill in the constrained node.
		/// If you want a node not constrained by any NNConstraint, do an additional search with constraint = NNConstraint.None
		/// </summary>
		public NNInfoInternal QueryClosest (Vector3 p, NNConstraint constraint, out float distance) {
			distance = float.PositiveInfinity;
			return QueryClosest(p, constraint, ref distance, new NNInfoInternal(null));
		}

		/// <summary>
		/// Queries the tree for the closest node to p constrained by the NNConstraint trying to improve an existing solution.
		/// Note that this function will only fill in the constrained node.
		/// If you want a node not constrained by any NNConstraint, do an additional search with constraint = NNConstraint.None
		///
		/// This method will completely ignore any Y-axis differences in positions.
		/// </summary>
		/// <param name="p">Point to search around</param>
		/// <param name="constraint">Optionally set to constrain which nodes to return</param>
		/// <param name="distance">The best distance for the previous solution. Will be updated with the best distance
		/// after this search. Will be positive infinity if no node could be found.
		/// Set to positive infinity if there was no previous solution.</param>
		/// <param name="previous">This search will start from the previous NNInfo and improve it if possible.
		/// Even if the search fails on this call, the solution will never be worse than previous.
		/// Note that the distance parameter need to be configured with the distance for the previous result
		/// otherwise it may get overwritten even though it was actually closer.</param>
		public NNInfoInternal QueryClosestXZ (Vector3 p, NNConstraint constraint, ref float distance, NNInfoInternal previous) {
			var sqrDistance = distance*distance;
			var origSqrDistance = sqrDistance;

			if (count > 0 && SquaredRectPointDistance(tree[0].rect, p) < sqrDistance) {
				SearchBoxClosestXZ(0, p, ref sqrDistance, constraint, ref previous);
				// Only update the distance if the squared distance changed as otherwise #distance
				// might change due to rounding errors even if no better solution was found
				if (sqrDistance < origSqrDistance) distance = Mathf.Sqrt(sqrDistance);
			}
			return previous;
		}

		/// <summary>
		/// Searches the box closest xz using the specified boxi
		/// </summary>
		/// <param name="boxi">The boxi</param>
		/// <param name="p">The </param>
		/// <param name="closestSqrDist">The closest sqr dist</param>
		/// <param name="constraint">The constraint</param>
		/// <param name="nnInfo">The nn info</param>
		void SearchBoxClosestXZ (int boxi, Vector3 p, ref float closestSqrDist, NNConstraint constraint, ref NNInfoInternal nnInfo) {
			BBTreeBox box = tree[boxi];

			if (box.IsLeaf) {
				var nodes = nodeLookup;
				for (int i = 0; i < MaximumLeafSize && nodes[box.nodeOffset+i] != null; i++) {
					var node = nodes[box.nodeOffset+i];
					// Update the NNInfo
					DrawDebugNode(node, 0.2f, Color.red);

					if (constraint == null || constraint.Suitable(node)) {
						Vector3 closest = node.ClosestPointOnNodeXZ(p);
						// XZ squared distance
						float dist = (closest.x-p.x)*(closest.x-p.x)+(closest.z-p.z)*(closest.z-p.z);

						// There's a theoretical case when the closest point is on the edge of a node which may cause the
						// closest point's xz coordinates to not line up perfectly with p's xz coordinates even though they should
						// (because floating point errors are annoying). So use a tiny margin to cover most of those cases.
						const float fuzziness = 0.000001f;
						if (nnInfo.constrainedNode == null || dist < closestSqrDist - fuzziness || (dist <= closestSqrDist + fuzziness && Mathf.Abs(closest.y - p.y) < Mathf.Abs(nnInfo.constClampedPosition.y - p.y))) {
							nnInfo.constrainedNode = node;
							nnInfo.constClampedPosition = closest;
							closestSqrDist = dist;
						}
					}
				}
			} else {
				DrawDebugRect(box.rect);

				int first = box.left, second = box.right;
				float firstDist, secondDist;
				GetOrderedChildren(ref first, ref second, out firstDist, out secondDist, p);

				// Search children (closest box first to improve performance)
				if (firstDist <= closestSqrDist) {
					SearchBoxClosestXZ(first, p, ref closestSqrDist, constraint, ref nnInfo);
				}

				if (secondDist <= closestSqrDist) {
					SearchBoxClosestXZ(second, p, ref closestSqrDist, constraint, ref nnInfo);
				}
			}
		}

		/// <summary>
		/// Queries the tree for the closest node to p constrained by the NNConstraint trying to improve an existing solution.
		/// Note that this function will only fill in the constrained node.
		/// If you want a node not constrained by any NNConstraint, do an additional search with constraint = NNConstraint.None
		/// </summary>
		/// <param name="p">Point to search around</param>
		/// <param name="constraint">Optionally set to constrain which nodes to return</param>
		/// <param name="distance">The best distance for the previous solution. Will be updated with the best distance
		/// after this search. Will be positive infinity if no node could be found.
		/// Set to positive infinity if there was no previous solution.</param>
		/// <param name="previous">This search will start from the previous NNInfo and improve it if possible.
		/// Even if the search fails on this call, the solution will never be worse than previous.</param>
		public NNInfoInternal QueryClosest (Vector3 p, NNConstraint constraint, ref float distance, NNInfoInternal previous) {
			var sqrDistance = distance*distance;
			var origSqrDistance = sqrDistance;

			if (count > 0 && SquaredRectPointDistance(tree[0].rect, p) < sqrDistance) {
				SearchBoxClosest(0, p, ref sqrDistance, constraint, ref previous);
				// Only update the distance if the squared distance changed as otherwise #distance
				// might change due to rounding errors even if no better solution was found
				if (sqrDistance < origSqrDistance) distance = Mathf.Sqrt(sqrDistance);
			}
			return previous;
		}

		/// <summary>
		/// Searches the box closest using the specified boxi
		/// </summary>
		/// <param name="boxi">The boxi</param>
		/// <param name="p">The </param>
		/// <param name="closestSqrDist">The closest sqr dist</param>
		/// <param name="constraint">The constraint</param>
		/// <param name="nnInfo">The nn info</param>
		void SearchBoxClosest (int boxi, Vector3 p, ref float closestSqrDist, NNConstraint constraint, ref NNInfoInternal nnInfo) {
			BBTreeBox box = tree[boxi];

			if (box.IsLeaf) {
				var nodes = nodeLookup;
				for (int i = 0; i < MaximumLeafSize && nodes[box.nodeOffset+i] != null; i++) {
					var node = nodes[box.nodeOffset+i];
					Vector3 closest = node.ClosestPointOnNode(p);
					float dist = (closest-p).sqrMagnitude;
					if (dist < closestSqrDist) {
						DrawDebugNode(node, 0.2f, Color.red);

						if (constraint == null || constraint.Suitable(node)) {
							// Update the NNInfo
							nnInfo.constrainedNode = node;
							nnInfo.constClampedPosition = closest;
							closestSqrDist = dist;
						}
					} else {
						DrawDebugNode(node, 0.0f, Color.blue);
					}
				}
			} else {
				DrawDebugRect(box.rect);
				int first = box.left, second = box.right;
				float firstDist, secondDist;
				GetOrderedChildren(ref first, ref second, out firstDist, out secondDist, p);

				// Search children (closest box first to improve performance)
				if (firstDist < closestSqrDist) {
					SearchBoxClosest(first, p, ref closestSqrDist, constraint, ref nnInfo);
				}

				if (secondDist < closestSqrDist) {
					SearchBoxClosest(second, p, ref closestSqrDist, constraint, ref nnInfo);
				}
			}
		}

		/// <summary>Orders the box indices first and second by the approximate distance to the point p</summary>
		void GetOrderedChildren (ref int first, ref int second, out float firstDist, out float secondDist, Vector3 p) {
			firstDist = SquaredRectPointDistance(tree[first].rect, p);
			secondDist = SquaredRectPointDistance(tree[second].rect, p);

			if (secondDist < firstDist) {
				// Swap
				var tmp = first;
				first = second;
				second = tmp;
				var tmp2 = firstDist;
				firstDist = secondDist;
				secondDist = tmp2;
			}
		}

		/// <summary>
		/// Searches for a node which contains the specified point.
		/// If there are multiple nodes that contain the point any one of them
		/// may be returned.
		///
		/// See: TriangleMeshNode.ContainsPoint
		/// </summary>
		public TriangleMeshNode QueryInside (Vector3 p, NNConstraint constraint) {
			return count != 0 && tree[0].Contains(p) ? SearchBoxInside(0, p, constraint) : null;
		}

		/// <summary>
		/// Searches the box inside using the specified boxi
		/// </summary>
		/// <param name="boxi">The boxi</param>
		/// <param name="p">The </param>
		/// <param name="constraint">The constraint</param>
		/// <returns>The triangle mesh node</returns>
		TriangleMeshNode SearchBoxInside (int boxi, Vector3 p, NNConstraint constraint) {
			BBTreeBox box = tree[boxi];

			if (box.IsLeaf) {
				var nodes = nodeLookup;
				for (int i = 0; i < MaximumLeafSize && nodes[box.nodeOffset+i] != null; i++) {
					var node = nodes[box.nodeOffset+i];
					if (node.ContainsPoint((Int3)p)) {
						DrawDebugNode(node, 0.2f, Color.red);

						if (constraint == null || constraint.Suitable(node)) {
							return node;
						}
					} else {
						DrawDebugNode(node, 0.0f, Color.blue);
					}
				}
			} else {
				DrawDebugRect(box.rect);

				//Search children
				if (tree[box.left].Contains(p)) {
					var result = SearchBoxInside(box.left, p, constraint);
					if (result != null) return result;
				}

				if (tree[box.right].Contains(p)) {
					var result = SearchBoxInside(box.right, p, constraint);
					if (result != null) return result;
				}
			}

			return null;
		}

		/// <summary>
		/// The bb tree box
		/// </summary>
		struct BBTreeBox {
			/// <summary>
			/// The rect
			/// </summary>
			public IntRect rect;

			/// <summary>
			/// The node offset
			/// </summary>
			public int nodeOffset;
			/// <summary>
			/// The right
			/// </summary>
			public int left, right;

			/// <summary>
			/// Gets the value of the is leaf
			/// </summary>
			public bool IsLeaf {
				get {
					return nodeOffset >= 0;
				}
			}

			/// <summary>
			/// Initializes a new instance of the <see cref="BBTreeBox"/> class
			/// </summary>
			/// <param name="rect">The rect</param>
			public BBTreeBox (IntRect rect) {
				nodeOffset = -1;
				this.rect = rect;
				left = right = -1;
			}

			/// <summary>
			/// Initializes a new instance of the <see cref="BBTreeBox"/> class
			/// </summary>
			/// <param name="nodeOffset">The node offset</param>
			/// <param name="rect">The rect</param>
			public BBTreeBox (int nodeOffset, IntRect rect) {
				this.nodeOffset = nodeOffset;
				this.rect = rect;
				left = right = -1;
			}

			/// <summary>
			/// Describes whether this instance contains
			/// </summary>
			/// <param name="point">The point</param>
			/// <returns>The bool</returns>
			public bool Contains (Vector3 point) {
				var pi = (Int3)point;

				return rect.Contains(pi.x, pi.z);
			}
		}

		/// <summary>
		/// Ons the draw gizmos
		/// </summary>
		public void OnDrawGizmos () {
			Gizmos.color = new Color(1, 1, 1, 0.5F);
			if (count == 0) return;
			OnDrawGizmos(0, 0);
		}

		/// <summary>
		/// Ons the draw gizmos using the specified boxi
		/// </summary>
		/// <param name="boxi">The boxi</param>
		/// <param name="depth">The depth</param>
		void OnDrawGizmos (int boxi, int depth) {
			BBTreeBox box = tree[boxi];

			var min = (Vector3) new Int3(box.rect.xmin, 0, box.rect.ymin);
			var max = (Vector3) new Int3(box.rect.xmax, 0, box.rect.ymax);

			Vector3 center = (min+max)*0.5F;
			Vector3 size = (max-center)*2;

			size = new Vector3(size.x, 1, size.z);
			center.y += depth * 2;

			Gizmos.color = AstarMath.IntToColor(depth, 1f);
			Gizmos.DrawCube(center, size);

			if (!box.IsLeaf) {
				OnDrawGizmos(box.left, depth + 1);
				OnDrawGizmos(box.right, depth + 1);
			}
		}

		/// <summary>
		/// Describes whether node intersects circle
		/// </summary>
		/// <param name="node">The node</param>
		/// <param name="p">The </param>
		/// <param name="radius">The radius</param>
		/// <returns>The bool</returns>
		static bool NodeIntersectsCircle (TriangleMeshNode node, Vector3 p, float radius) {
			if (float.IsPositiveInfinity(radius)) return true;

			/// <summary>\bug Is not correct on the Y axis</summary>
			return (p - node.ClosestPointOnNode(p)).sqrMagnitude < radius*radius;
		}

		/// <summary>
		/// Returns true if p is within radius from r.
		/// Correctly handles cases where radius is positive infinity.
		/// </summary>
		static bool RectIntersectsCircle (IntRect r, Vector3 p, float radius) {
			if (float.IsPositiveInfinity(radius)) return true;

			Vector3 po = p;
			p.x = Math.Max(p.x, r.xmin*Int3.PrecisionFactor);
			p.x = Math.Min(p.x, r.xmax*Int3.PrecisionFactor);
			p.z = Math.Max(p.z, r.ymin*Int3.PrecisionFactor);
			p.z = Math.Min(p.z, r.ymax*Int3.PrecisionFactor);

			// XZ squared magnitude comparison
			return (p.x-po.x)*(p.x-po.x) + (p.z-po.z)*(p.z-po.z) < radius*radius;
		}

		/// <summary>Returns distance from p to the rectangle r</summary>
		static float SquaredRectPointDistance (IntRect r, Vector3 p) {
			Vector3 po = p;

			p.x = Math.Max(p.x, r.xmin*Int3.PrecisionFactor);
			p.x = Math.Min(p.x, r.xmax*Int3.PrecisionFactor);
			p.z = Math.Max(p.z, r.ymin*Int3.PrecisionFactor);
			p.z = Math.Min(p.z, r.ymax*Int3.PrecisionFactor);

			// XZ squared magnitude comparison
			return (p.x-po.x)*(p.x-po.x) + (p.z-po.z)*(p.z-po.z);
		}
	}
}
