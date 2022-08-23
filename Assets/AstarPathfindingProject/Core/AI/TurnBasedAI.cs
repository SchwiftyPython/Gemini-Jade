using UnityEngine;
using System.Collections.Generic;

namespace Pathfinding.Examples {
	/// <summary>Helper script in the example scene 'Turn Based'</summary>
	[HelpURL("http://arongranberg.com/astar/docs/class_pathfinding_1_1_examples_1_1_turn_based_a_i.php")]
	public class TurnBasedAI : VersionedMonoBehaviour {
		/// <summary>
		/// The movement points
		/// </summary>
		public int movementPoints = 2;
		/// <summary>
		/// The block manager
		/// </summary>
		public BlockManager blockManager;
		/// <summary>
		/// The blocker
		/// </summary>
		public SingleNodeBlocker blocker;
		/// <summary>
		/// The target node
		/// </summary>
		public GraphNode targetNode;
		/// <summary>
		/// The traversal provider
		/// </summary>
		public BlockManager.TraversalProvider traversalProvider;

		/// <summary>
		/// Starts this instance
		/// </summary>
		void Start () {
			blocker.BlockAtCurrentPosition();
		}

		/// <summary>
		/// Awakes this instance
		/// </summary>
		protected override void Awake () {
			base.Awake();
			// Set the traversal provider to block all nodes that are blocked by a SingleNodeBlocker
			// except the SingleNodeBlocker owned by this AI (we don't want to be blocked by ourself)
			traversalProvider = new BlockManager.TraversalProvider(blockManager, BlockManager.BlockMode.AllExceptSelector, new List<SingleNodeBlocker>() { blocker });
		}
	}
}
