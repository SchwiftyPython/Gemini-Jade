using Pathfinding.Serialization;

namespace Pathfinding {
	/// <summary>
	/// The graph editor base class
	/// </summary>
	[JsonOptIn]
	/// <summary>Defined here only so non-editor classes can use the <see cref="target"/> field</summary>
	public class GraphEditorBase {
		/// <summary>NavGraph this editor is exposing</summary>
		public NavGraph target;
	}
}
