#pragma warning disable 414
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding {
	/// <summary>
	/// The heuristic optimization mode enum
	/// </summary>
	public enum HeuristicOptimizationMode {
		/// <summary>
		/// The none heuristic optimization mode
		/// </summary>
		None,
		/// <summary>
		/// The random heuristic optimization mode
		/// </summary>
		Random,
		/// <summary>
		/// The random spread out heuristic optimization mode
		/// </summary>
		RandomSpreadOut,
		/// <summary>
		/// The custom heuristic optimization mode
		/// </summary>
		Custom
	}

	/// <summary>
	/// Implements heuristic optimizations.
	///
	/// See: heuristic-opt
	/// See: Game AI Pro - Pathfinding Architecture Optimizations by Steve Rabin and Nathan R. Sturtevant
	/// </summary>
	[System.Serializable]
	public class EuclideanEmbedding {
		/// <summary>
		/// If heuristic optimization should be used and how to place the pivot points.
		/// See: heuristic-opt
		/// See: Game AI Pro - Pathfinding Architecture Optimizations by Steve Rabin and Nathan R. Sturtevant
		/// </summary>
		public HeuristicOptimizationMode mode;

		/// <summary>
		/// The seed
		/// </summary>
		public int seed;

		/// <summary>All children of this transform will be used as pivot points</summary>
		public Transform pivotPointRoot;

		/// <summary>
		/// The spread out count
		/// </summary>
		public int spreadOutCount = 1;

		/// <summary>
		/// The dirty
		/// </summary>
		[System.NonSerialized]
		public bool dirty;


		/// <summary>
		/// Ensures the capacity using the specified index
		/// </summary>
		/// <param name="index">The index</param>
		void EnsureCapacity (int index) {
		}

		/// <summary>
		/// Gets the heuristic using the specified node index 1
		/// </summary>
		/// <param name="nodeIndex1">The node index</param>
		/// <param name="nodeIndex2">The node index</param>
		/// <returns>The uint</returns>
		public uint GetHeuristic (int nodeIndex1, int nodeIndex2) {
			return 0;
		}


		/// <summary>
		/// Recalculates the pivots
		/// </summary>
		public void RecalculatePivots () {
		}

		/// <summary>
		/// Recalculates the costs
		/// </summary>
		public void RecalculateCosts () {
			dirty = false;
		}


		/// <summary>
		/// Ons the draw gizmos
		/// </summary>
		public void OnDrawGizmos () {
		}
	}
}
