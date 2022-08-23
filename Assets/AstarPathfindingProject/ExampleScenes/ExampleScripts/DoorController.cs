using UnityEngine;

namespace Pathfinding.Examples {
	/// <summary>Example script used in the example scenes</summary>
	[HelpURL("http://arongranberg.com/astar/docs/class_pathfinding_1_1_examples_1_1_door_controller.php")]
	public class DoorController : MonoBehaviour {
		/// <summary>
		/// The open
		/// </summary>
		private bool open = false;

		/// <summary>
		/// The opentag
		/// </summary>
		public int opentag = 1;
		/// <summary>
		/// The closedtag
		/// </summary>
		public int closedtag = 1;
		/// <summary>
		/// The update graphs with guo
		/// </summary>
		public bool updateGraphsWithGUO = true;
		/// <summary>
		/// The offset
		/// </summary>
		public float yOffset = 5;

		/// <summary>
		/// The bounds
		/// </summary>
		Bounds bounds;

		/// <summary>
		/// Starts this instance
		/// </summary>
		public void Start () {
			// Capture the bounds of the collider while it is closed
			bounds = GetComponent<Collider>().bounds;

			// Initially open the door
			SetState(open);
		}

		/// <summary>
		/// Ons the gui
		/// </summary>
		void OnGUI () {
			// Show a UI button for opening and closing the door
			if (GUI.Button(new Rect(5, yOffset, 100, 22), "Toggle Door")) {
				SetState(!open);
			}
		}

		/// <summary>
		/// Sets the state using the specified open
		/// </summary>
		/// <param name="open">The open</param>
		public void SetState (bool open) {
			this.open = open;

			if (updateGraphsWithGUO) {
				// Update the graph below the door
				// Set the tag of the nodes below the door
				// To something indicating that the door is open or closed
				GraphUpdateObject guo = new GraphUpdateObject(bounds);
				int tag = open ? opentag : closedtag;

				// There are only 32 tags
				if (tag > 31) { Debug.LogError("tag > 31"); return; }

				guo.modifyTag = true;
				guo.setTag = tag;
				guo.updatePhysics = false;

				AstarPath.active.UpdateGraphs(guo);
			}

			// Play door animations
			if (open) {
				GetComponent<Animation>().Play("Open");
			} else {
				GetComponent<Animation>().Play("Close");
			}
		}
	}
}
