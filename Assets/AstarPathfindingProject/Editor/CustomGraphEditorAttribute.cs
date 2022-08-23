namespace Pathfinding {
	/// <summary>Added to editors of custom graph types</summary>
	[System.AttributeUsage(System.AttributeTargets.All, Inherited = false, AllowMultiple = true)]
	public class CustomGraphEditorAttribute : System.Attribute {
		/// <summary>Graph type which this is an editor for</summary>
		public System.Type graphType;

		/// <summary>Name displayed in the inpector</summary>
		public string displayName;

		/// <summary>Type of the editor for the graph</summary>
		public System.Type editorType;

		/// <summary>
		/// Initializes a new instance of the <see cref="CustomGraphEditorAttribute"/> class
		/// </summary>
		/// <param name="t">The </param>
		/// <param name="displayName">The display name</param>
		public CustomGraphEditorAttribute (System.Type t, string displayName) {
			graphType = t;
			this.displayName = displayName;
		}
	}
}
