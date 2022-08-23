using UnityEngine;

namespace Pathfinding {
	/// <summary>
	/// The unity reference helper class
	/// </summary>
	/// <seealso cref="MonoBehaviour"/>
	[ExecuteInEditMode]
	/// <summary>
	/// Helper class to keep track of references to GameObjects.
	/// Does nothing more than to hold a GUID value.
	/// </summary>
	[HelpURL("http://arongranberg.com/astar/docs/class_pathfinding_1_1_unity_reference_helper.php")]
	public class UnityReferenceHelper : MonoBehaviour {
		/// <summary>
		/// The guid
		/// </summary>
		[HideInInspector]
		[SerializeField]
		private string guid;

		/// <summary>
		/// Gets the guid
		/// </summary>
		/// <returns>The guid</returns>
		public string GetGUID () {
			return guid;
		}

		/// <summary>
		/// Awakes this instance
		/// </summary>
		public void Awake () {
			Reset();
		}

		/// <summary>
		/// Resets this instance
		/// </summary>
		public void Reset () {
			if (string.IsNullOrEmpty(guid)) {
				guid = Pathfinding.Util.Guid.NewGuid().ToString();
				Debug.Log("Created new GUID - " + guid, this);
			} else if (gameObject.scene.name != null) {
				// Create a new GUID if there are duplicates in the scene.
				// Don't do this if this is a prefab (scene.name == null)
				foreach (UnityReferenceHelper urh in FindObjectsOfType(typeof(UnityReferenceHelper)) as UnityReferenceHelper[]) {
					if (urh != this && guid == urh.guid) {
						guid = Pathfinding.Util.Guid.NewGuid().ToString();
						Debug.Log("Created new GUID - " + guid, this);
						return;
					}
				}
			}
		}
	}
}
