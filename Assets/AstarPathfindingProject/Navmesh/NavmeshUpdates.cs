using UnityEngine;
using System.Collections.Generic;
using Pathfinding.Util;
using Pathfinding.Serialization;
#if UNITY_5_5_OR_NEWER
using UnityEngine.Profiling;
#endif

namespace Pathfinding {
	/// <summary>
	/// Helper for navmesh cut objects.
	/// Responsible for keeping track of which navmesh cuts have moved and coordinating graph updates to account for those changes.
	///
	/// See: navmeshcutting (view in online documentation for working links)
	/// See: <see cref="AstarPath.navmeshUpdates"/>
	/// See: <see cref="Pathfinding.NavmeshBase.enableNavmeshCutting"/>
	/// </summary>
	[System.Serializable]
	public class NavmeshUpdates {
		/// <summary>
		/// How often to check if an update needs to be done (real seconds between checks).
		/// For worlds with a very large number of NavmeshCut objects, it might be bad for performance to do this check every frame.
		/// If you think this is a performance penalty, increase this number to check less often.
		///
		/// For almost all games, this can be kept at 0.
		///
		/// If negative, no updates will be done. They must be manually triggered using <see cref="ForceUpdate"/>.
		///
		/// <code>
		/// // Check every frame (the default)
		/// AstarPath.active.navmeshUpdates.updateInterval = 0;
		///
		/// // Check every 0.1 seconds
		/// AstarPath.active.navmeshUpdates.updateInterval = 0.1f;
		///
		/// // Never check for changes
		/// AstarPath.active.navmeshUpdates.updateInterval = -1;
		/// // You will have to schedule updates manually using
		/// AstarPath.active.navmeshUpdates.ForceUpdate();
		/// </code>
		///
		/// You can also find this in the AstarPath inspector under Settings.
		/// [Open online documentation to see images]
		/// </summary>
		public float updateInterval;

		/// <summary>
		/// The navmesh update settings class
		/// </summary>
		internal class NavmeshUpdateSettings {
			/// <summary>
			/// Initializes a new instance of the <see cref="NavmeshUpdateSettings"/> class
			/// </summary>
			/// <param name="graph">The graph</param>
			public NavmeshUpdateSettings(NavmeshBase graph) {}
			/// <summary>
			/// Ons the recalculated tiles using the specified tiles
			/// </summary>
			/// <param name="tiles">The tiles</param>
			public void OnRecalculatedTiles (NavmeshTile[] tiles) {}
		}
		/// <summary>
		/// Updates this instance
		/// </summary>
		internal void Update () {}
		/// <summary>
		/// Ons the enable
		/// </summary>
		internal void OnEnable () {}
		/// <summary>
		/// Ons the disable
		/// </summary>
		internal void OnDisable () {}
	}
}
