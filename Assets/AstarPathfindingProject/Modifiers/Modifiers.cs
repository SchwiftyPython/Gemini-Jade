using UnityEngine;

namespace Pathfinding {
	/// <summary>
	/// Base for all path modifiers.
	/// See: MonoModifier
	/// Modifier
	/// </summary>
	public interface IPathModifier {
		/// <summary>
		/// Gets the value of the order
		/// </summary>
		int Order { get; }

		/// <summary>
		/// Applies the path
		/// </summary>
		/// <param name="path">The path</param>
		void Apply(Path path);
		/// <summary>
		/// Pres the process using the specified path
		/// </summary>
		/// <param name="path">The path</param>
		void PreProcess(Path path);
	}

	/// <summary>
	/// Base class for path modifiers which are not attached to GameObjects.
	/// See: MonoModifier
	/// </summary>
	[System.Serializable]
	public abstract class PathModifier : IPathModifier {
		/// <summary>
		/// The seeker
		/// </summary>
		[System.NonSerialized]
		public Seeker seeker;

		/// <summary>
		/// Modifiers will be executed from lower order to higher order.
		/// This value is assumed to stay constant.
		/// </summary>
		public abstract int Order { get; }

		/// <summary>
		/// Awakes the seeker
		/// </summary>
		/// <param name="seeker">The seeker</param>
		public void Awake (Seeker seeker) {
			this.seeker = seeker;
			if (seeker != null) {
				seeker.RegisterModifier(this);
			}
		}

		/// <summary>
		/// Ons the destroy using the specified seeker
		/// </summary>
		/// <param name="seeker">The seeker</param>
		public void OnDestroy (Seeker seeker) {
			if (seeker != null) {
				seeker.DeregisterModifier(this);
			}
		}

		/// <summary>
		/// Pres the process using the specified path
		/// </summary>
		/// <param name="path">The path</param>
		public virtual void PreProcess (Path path) {
			// Required by IPathModifier
		}

		/// <summary>Main Post-Processing function</summary>
		public abstract void Apply(Path path);
	}

	/// <summary>
	/// Base class for path modifiers which can be attached to GameObjects.
	/// See: Menubar -> Component -> Pathfinding -> Modifiers
	/// </summary>
	[System.Serializable]
	public abstract class MonoModifier : VersionedMonoBehaviour, IPathModifier {
		/// <summary>
		/// The seeker
		/// </summary>
		[System.NonSerialized]
		public Seeker seeker;

		/// <summary>Alerts the Seeker that this modifier exists</summary>
		protected virtual void OnEnable () {
			seeker = GetComponent<Seeker>();

			if (seeker != null) {
				seeker.RegisterModifier(this);
			}
		}

		/// <summary>
		/// Ons the disable
		/// </summary>
		protected virtual void OnDisable () {
			if (seeker != null) {
				seeker.DeregisterModifier(this);
			}
		}

		/// <summary>
		/// Modifiers will be executed from lower order to higher order.
		/// This value is assumed to stay constant.
		/// </summary>
		public abstract int Order { get; }

		/// <summary>
		/// Pres the process using the specified path
		/// </summary>
		/// <param name="path">The path</param>
		public virtual void PreProcess (Path path) {
			// Required by IPathModifier
		}

		/// <summary>Called for each path that the Seeker calculates after the calculation has finished</summary>
		public abstract void Apply(Path path);
	}
}
