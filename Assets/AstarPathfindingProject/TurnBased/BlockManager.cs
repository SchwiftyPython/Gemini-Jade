using UnityEngine;
using System.Collections.Generic;

namespace Pathfinding {
	using Pathfinding.Util;

	/// <summary>
	/// Manager for blocker scripts such as SingleNodeBlocker.
	///
	/// This is part of the turn based utilities. It can be used for
	/// any game, but it is primarily intended for turn based games.
	///
	/// See: TurnBasedAI
	/// See: turnbased (view in online documentation for working links)
	/// </summary>
	[HelpURL("http://arongranberg.com/astar/docs/class_pathfinding_1_1_block_manager.php")]
	public class BlockManager : VersionedMonoBehaviour {
		/// <summary>Contains info on which SingleNodeBlocker objects have blocked a particular node</summary>
		Dictionary<GraphNode, List<SingleNodeBlocker> > blocked = new Dictionary<GraphNode, List<SingleNodeBlocker> >();

		/// <summary>
		/// The block mode enum
		/// </summary>
		public enum BlockMode {
			/// <summary>All blockers except those in the TraversalProvider.selector list will block</summary>
			AllExceptSelector,
			/// <summary>Only elements in the TraversalProvider.selector list will block</summary>
			OnlySelector
		}

		/// <summary>Blocks nodes according to a BlockManager</summary>
		public class TraversalProvider : ITraversalProvider {
			/// <summary>Holds information about which nodes are occupied</summary>
			readonly BlockManager blockManager;

			/// <summary>Affects which nodes are considered blocked</summary>
			public BlockMode mode { get; private set; }

			/// <summary>
			/// Blockers for this path.
			/// The effect depends on <see cref="mode"/>.
			///
			/// Note that having a large selector has a performance cost.
			///
			/// See: mode
			/// </summary>
			readonly List<SingleNodeBlocker> selector;

			/// <summary>
			/// Initializes a new instance of the <see cref="TraversalProvider"/> class
			/// </summary>
			/// <param name="blockManager">The block manager</param>
			/// <param name="mode">The mode</param>
			/// <param name="selector">The selector</param>
			/// <exception cref="System.ArgumentNullException">blockManager</exception>
			/// <exception cref="System.ArgumentNullException">selector</exception>
			public TraversalProvider (BlockManager blockManager, BlockMode mode, List<SingleNodeBlocker> selector) {
				if (blockManager == null) throw new System.ArgumentNullException("blockManager");
				if (selector == null) throw new System.ArgumentNullException("selector");

				this.blockManager = blockManager;
				this.mode = mode;
				this.selector = selector;
			}

			#region ITraversalProvider implementation

			/// <summary>
			/// Describes whether this instance can traverse
			/// </summary>
			/// <param name="path">The path</param>
			/// <param name="node">The node</param>
			/// <returns>The bool</returns>
			public bool CanTraverse (Path path, GraphNode node) {
				// This first IF is the default implementation that is used when no traversal provider is used
				if (!node.Walkable || (path.enabledTags >> (int)node.Tag & 0x1) == 0) {
					return false;
				} else if (mode == BlockMode.OnlySelector) {
					return !blockManager.NodeContainsAnyOf(node, selector);
				} else {
					// assume mode == BlockMode.AllExceptSelector
					return !blockManager.NodeContainsAnyExcept(node, selector);
				}
			}

			/// <summary>
			/// Gets the traversal cost using the specified path
			/// </summary>
			/// <param name="path">The path</param>
			/// <param name="node">The node</param>
			/// <returns>The uint</returns>
			public uint GetTraversalCost (Path path, GraphNode node) {
				// Same as default implementation
				return path.GetTagPenalty((int)node.Tag) + node.Penalty;
			}

			#endregion
		}

		/// <summary>
		/// Starts this instance
		/// </summary>
		/// <exception cref="System.Exception">No AstarPath object in the scene</exception>
		void Start () {
			if (!AstarPath.active)
				throw new System.Exception("No AstarPath object in the scene");
		}

		/// <summary>True if the node contains any blocker which is included in the selector list</summary>
		public bool NodeContainsAnyOf (GraphNode node, List<SingleNodeBlocker> selector) {
			List<SingleNodeBlocker> blockersInNode;

			if (!blocked.TryGetValue(node, out blockersInNode)) {
				return false;
			}

			for (int i = 0; i < blockersInNode.Count; i++) {
				var inNode = blockersInNode[i];
				for (int j = 0; j < selector.Count; j++) {
					// Need to use ReferenceEquals because this code may be called from a separate thread
					// and the equality comparison that Unity provides is not thread safe
					if (System.Object.ReferenceEquals(inNode, selector[j])) {
						return true;
					}
				}
			}
			return false;
		}

		/// <summary>True if the node contains any blocker which is not included in the selector list</summary>
		public bool NodeContainsAnyExcept (GraphNode node, List<SingleNodeBlocker> selector) {
			List<SingleNodeBlocker> blockersInNode;

			if (!blocked.TryGetValue(node, out blockersInNode)) {
				return false;
			}

			for (int i = 0; i < blockersInNode.Count; i++) {
				var inNode = blockersInNode[i];
				bool found = false;
				for (int j = 0; j < selector.Count; j++) {
					// Need to use ReferenceEquals because this code may be called from a separate thread
					// and the equality comparison that Unity provides is not thread safe
					if (System.Object.ReferenceEquals(inNode, selector[j])) {
						found = true;
						break;
					}
				}
				if (!found) return true;
			}
			return false;
		}

		/// <summary>
		/// Register blocker as being present at the specified node.
		/// Calling this method multiple times will add multiple instances of the blocker to the node.
		///
		/// Note: The node will not be blocked immediately. Instead the pathfinding
		/// threads will be paused and then the update will be applied. It is however
		/// guaranteed to be applied before the next path request is started.
		/// </summary>
		public void InternalBlock (GraphNode node, SingleNodeBlocker blocker) {
			AstarPath.active.AddWorkItem(new AstarWorkItem(() => {
				List<SingleNodeBlocker> blockersInNode;
				if (!blocked.TryGetValue(node, out blockersInNode)) {
					blockersInNode = blocked[node] = ListPool<SingleNodeBlocker>.Claim();
				}

				blockersInNode.Add(blocker);
			}));
		}

		/// <summary>
		/// Remove blocker from the specified node.
		/// Will only remove a single instance, calling this method multiple
		/// times will remove multiple instances of the blocker from the node.
		///
		/// Note: The node will not be unblocked immediately. Instead the pathfinding
		/// threads will be paused and then the update will be applied. It is however
		/// guaranteed to be applied before the next path request is started.
		/// </summary>
		public void InternalUnblock (GraphNode node, SingleNodeBlocker blocker) {
			AstarPath.active.AddWorkItem(new AstarWorkItem(() => {
				List<SingleNodeBlocker> blockersInNode;
				if (blocked.TryGetValue(node, out blockersInNode)) {
					blockersInNode.Remove(blocker);

					if (blockersInNode.Count == 0) {
						blocked.Remove(node);
						ListPool<SingleNodeBlocker>.Release(ref blockersInNode);
					}
				}
			}));
		}
	}
}
