namespace Pathfinding {
	/// <summary>
	/// The unique component attribute class
	/// </summary>
	/// <seealso cref="System.Attribute"/>
	[System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = true)]
	public class UniqueComponentAttribute : System.Attribute {
		/// <summary>
		/// The tag
		/// </summary>
		public string tag;
	}
}
