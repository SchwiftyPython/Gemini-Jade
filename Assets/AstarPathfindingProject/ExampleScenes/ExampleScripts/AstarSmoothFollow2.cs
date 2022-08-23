using UnityEngine;

namespace Pathfinding.Examples {
	/// <summary>
	/// Smooth Camera Following.
	/// \author http://wiki.unity3d.com/index.php/SmoothFollow2
	/// </summary>
	[HelpURL("http://arongranberg.com/astar/docs/class_pathfinding_1_1_examples_1_1_astar_smooth_follow2.php")]
	public class AstarSmoothFollow2 : MonoBehaviour {
		/// <summary>
		/// The target
		/// </summary>
		public Transform target;
		/// <summary>
		/// The distance
		/// </summary>
		public float distance = 3.0f;
		/// <summary>
		/// The height
		/// </summary>
		public float height = 3.0f;
		/// <summary>
		/// The damping
		/// </summary>
		public float damping = 5.0f;
		/// <summary>
		/// The smooth rotation
		/// </summary>
		public bool smoothRotation = true;
		/// <summary>
		/// The follow behind
		/// </summary>
		public bool followBehind = true;
		/// <summary>
		/// The rotation damping
		/// </summary>
		public float rotationDamping = 10.0f;
		/// <summary>
		/// The static offset
		/// </summary>
		public bool staticOffset = false;

		/// <summary>
		/// Lates the update
		/// </summary>
		void LateUpdate () {
			Vector3 wantedPosition;

			if (staticOffset) {
				wantedPosition = target.position + new Vector3(0, height, distance);
			} else {
				if (followBehind)
					wantedPosition = target.TransformPoint(0, height, -distance);
				else
					wantedPosition = target.TransformPoint(0, height, distance);
			}
			transform.position = Vector3.Lerp(transform.position, wantedPosition, Time.deltaTime * damping);

			if (smoothRotation) {
				Quaternion wantedRotation = Quaternion.LookRotation(target.position - transform.position, target.up);
				transform.rotation = Quaternion.Slerp(transform.rotation, wantedRotation, Time.deltaTime * rotationDamping);
			} else transform.LookAt(target, target.up);
		}
	}
}
