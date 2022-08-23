//#define ProfileAstar

using UnityEngine;
using System.Text;

namespace Pathfinding {
	/// <summary>
	/// The astar debugger class
	/// </summary>
	/// <seealso cref="VersionedMonoBehaviour"/>
	[AddComponentMenu("Pathfinding/Pathfinding Debugger")]
	[ExecuteInEditMode]
	/// <summary>
	/// Debugger for the A* Pathfinding Project.
	/// This class can be used to profile different parts of the pathfinding system
	/// and the whole game as well to some extent.
	///
	/// Clarification of the labels shown when enabled.
	/// All memory related things profiles <b>the whole game</b> not just the A* Pathfinding System.\n
	/// - Currently allocated: memory the GC (garbage collector) says the application has allocated right now.
	/// - Peak allocated: maximum measured value of the above.
	/// - Last collect peak: the last peak of 'currently allocated'.
	/// - Allocation rate: how much the 'currently allocated' value increases per second. This value is not as reliable as you can think
	/// it is often very random probably depending on how the GC thinks this application is using memory.
	/// - Collection frequency: how often the GC is called. Again, the GC might decide it is better with many small collections
	/// or with a few large collections. So you cannot really trust this variable much.
	/// - Last collect fps: FPS during the last garbage collection, the GC will lower the fps a lot.
	///
	/// - FPS: current FPS (not updated every frame for readability)
	/// - Lowest FPS (last x): As the label says, the lowest fps of the last x frames.
	///
	/// - Size: Size of the path pool.
	/// - Total created: Number of paths of that type which has been created. Pooled paths are not counted twice.
	/// If this value just keeps on growing and growing without an apparent stop, you are are either not pooling any paths
	/// or you have missed to pool some path somewhere in your code.
	///
	/// See: pooling
	///
	/// TODO: Add field showing how many graph updates are being done right now
	/// </summary>
	[HelpURL("http://arongranberg.com/astar/docs/class_pathfinding_1_1_astar_debugger.php")]
	public class AstarDebugger : VersionedMonoBehaviour {
		/// <summary>
		/// The offset
		/// </summary>
		public int yOffset = 5;

		/// <summary>
		/// The show
		/// </summary>
		public bool show = true;
		/// <summary>
		/// The show in editor
		/// </summary>
		public bool showInEditor = false;

		/// <summary>
		/// The show fps
		/// </summary>
		public bool showFPS = false;
		/// <summary>
		/// The show path profile
		/// </summary>
		public bool showPathProfile = false;
		/// <summary>
		/// The show mem profile
		/// </summary>
		public bool showMemProfile = false;
		/// <summary>
		/// The show graph
		/// </summary>
		public bool showGraph = false;

		/// <summary>
		/// The graph buffer size
		/// </summary>
		public int graphBufferSize = 200;

		/// <summary>
		/// Font to use.
		/// A monospaced font is the best
		/// </summary>
		public Font font = null;
		/// <summary>
		/// The font size
		/// </summary>
		public int fontSize = 12;

		/// <summary>
		/// The string builder
		/// </summary>
		StringBuilder text = new StringBuilder();
		/// <summary>
		/// The cached text
		/// </summary>
		string cachedText;
		/// <summary>
		/// The last update
		/// </summary>
		float lastUpdate = -999;

		/// <summary>
		/// The graph
		/// </summary>
		private GraphPoint[] graph;

		/// <summary>
		/// The graph point
		/// </summary>
		struct GraphPoint {
			/// <summary>
			/// The memory
			/// </summary>
			public float fps, memory;
			/// <summary>
			/// The collect event
			/// </summary>
			public bool collectEvent;
		}

		/// <summary>
		/// The delayed delta time
		/// </summary>
		private float delayedDeltaTime = 1;
		/// <summary>
		/// The last collect
		/// </summary>
		private float lastCollect = 0;
		/// <summary>
		/// The last collect num
		/// </summary>
		private float lastCollectNum = 0;
		/// <summary>
		/// The delta
		/// </summary>
		private float delta = 0;
		/// <summary>
		/// The last delta time
		/// </summary>
		private float lastDeltaTime = 0;
		/// <summary>
		/// The alloc rate
		/// </summary>
		private int allocRate = 0;
		/// <summary>
		/// The last alloc memory
		/// </summary>
		private int lastAllocMemory = 0;
		/// <summary>
		/// The last alloc set
		/// </summary>
		private float lastAllocSet = -9999;
		/// <summary>
		/// The alloc mem
		/// </summary>
		private int allocMem = 0;
		/// <summary>
		/// The collect alloc
		/// </summary>
		private int collectAlloc = 0;
		/// <summary>
		/// The peak alloc
		/// </summary>
		private int peakAlloc = 0;

		/// <summary>
		/// The fps drop counter size
		/// </summary>
		private int fpsDropCounterSize = 200;
		/// <summary>
		/// The fps drops
		/// </summary>
		private float[] fpsDrops;

		/// <summary>
		/// The box rect
		/// </summary>
		private Rect boxRect;

		/// <summary>
		/// The style
		/// </summary>
		private GUIStyle style;

		/// <summary>
		/// The cam
		/// </summary>
		private Camera cam;

		/// <summary>
		/// The graph width
		/// </summary>
		float graphWidth = 100;
		/// <summary>
		/// The graph height
		/// </summary>
		float graphHeight = 100;
		/// <summary>
		/// The graph offset
		/// </summary>
		float graphOffset = 50;

		/// <summary>
		/// Starts this instance
		/// </summary>
		public void Start () {
			useGUILayout = false;

			fpsDrops = new float[fpsDropCounterSize];

			cam = GetComponent<Camera>();
			if (cam == null) {
				cam = Camera.main;
			}

			graph = new GraphPoint[graphBufferSize];

			if (Time.unscaledDeltaTime > 0) {
				for (int i = 0; i < fpsDrops.Length; i++) {
					fpsDrops[i] = 1F / Time.unscaledDeltaTime;
				}
			}
		}

		/// <summary>
		/// The max vec pool
		/// </summary>
		int maxVecPool = 0;
		/// <summary>
		/// The max node pool
		/// </summary>
		int maxNodePool = 0;

		/// <summary>
		/// The ab path
		/// </summary>
		PathTypeDebug[] debugTypes = new PathTypeDebug[] {
			new PathTypeDebug("ABPath", () => PathPool.GetSize(typeof(ABPath)), () => PathPool.GetTotalCreated(typeof(ABPath)))
		};

		/// <summary>
		/// The path type debug
		/// </summary>
		struct PathTypeDebug {
			/// <summary>
			/// The name
			/// </summary>
			string name;
			/// <summary>
			/// The get size
			/// </summary>
			System.Func<int> getSize;
			/// <summary>
			/// The get total created
			/// </summary>
			System.Func<int> getTotalCreated;
			/// <summary>
			/// Initializes a new instance of the <see cref="PathTypeDebug"/> class
			/// </summary>
			/// <param name="name">The name</param>
			/// <param name="getSize">The get size</param>
			/// <param name="getTotalCreated">The get total created</param>
			public PathTypeDebug (string name, System.Func<int> getSize, System.Func<int> getTotalCreated) {
				this.name = name;
				this.getSize = getSize;
				this.getTotalCreated = getTotalCreated;
			}

			/// <summary>
			/// Prints the text
			/// </summary>
			/// <param name="text">The text</param>
			public void Print (StringBuilder text) {
				int totCreated = getTotalCreated();

				if (totCreated > 0) {
					text.Append("\n").Append(("  " + name).PadRight(25)).Append(getSize()).Append("/").Append(totCreated);
				}
			}
		}

		/// <summary>
		/// Lates the update
		/// </summary>
		public void LateUpdate () {
			if (!show || (!Application.isPlaying && !showInEditor)) return;

			if (Time.unscaledDeltaTime <= 0.0001f)
				return;

			int collCount = System.GC.CollectionCount(0);

			if (lastCollectNum != collCount) {
				lastCollectNum = collCount;
				delta = Time.realtimeSinceStartup-lastCollect;
				lastCollect = Time.realtimeSinceStartup;
				lastDeltaTime = Time.unscaledDeltaTime;
				collectAlloc = allocMem;
			}

			allocMem = (int)System.GC.GetTotalMemory(false);

			bool collectEvent = allocMem < peakAlloc;
			peakAlloc = !collectEvent ? allocMem : peakAlloc;

			if (Time.realtimeSinceStartup - lastAllocSet > 0.3F || !Application.isPlaying) {
				int diff = allocMem - lastAllocMemory;
				lastAllocMemory = allocMem;
				lastAllocSet = Time.realtimeSinceStartup;
				delayedDeltaTime = Time.unscaledDeltaTime;

				if (diff >= 0) {
					allocRate = diff;
				}
			}

			if (Application.isPlaying) {
				fpsDrops[Time.frameCount % fpsDrops.Length] = Time.unscaledDeltaTime > 0.00001f ? 1F / Time.unscaledDeltaTime : 0;
				int graphIndex = Time.frameCount % graph.Length;
				graph[graphIndex].fps = Time.unscaledDeltaTime < 0.00001f ? 1F / Time.unscaledDeltaTime : 0;
				graph[graphIndex].collectEvent = collectEvent;
				graph[graphIndex].memory = allocMem;
			}

			if (Application.isPlaying && cam != null && showGraph) {
				graphWidth = cam.pixelWidth*0.8f;


				float minMem = float.PositiveInfinity, maxMem = 0, minFPS = float.PositiveInfinity, maxFPS = 0;
				for (int i = 0; i < graph.Length; i++) {
					minMem = Mathf.Min(graph[i].memory, minMem);
					maxMem = Mathf.Max(graph[i].memory, maxMem);
					minFPS = Mathf.Min(graph[i].fps, minFPS);
					maxFPS = Mathf.Max(graph[i].fps, maxFPS);
				}

				int currentGraphIndex = Time.frameCount % graph.Length;

				Matrix4x4 m = Matrix4x4.TRS(new Vector3((cam.pixelWidth - graphWidth)/2f, graphOffset, 1), Quaternion.identity, new Vector3(graphWidth, graphHeight, 1));

				for (int i = 0; i < graph.Length-1; i++) {
					if (i == currentGraphIndex) continue;

					DrawGraphLine(i, m, i/(float)graph.Length, (i+1)/(float)graph.Length, Mathf.InverseLerp(minMem, maxMem, graph[i].memory), Mathf.InverseLerp(minMem, maxMem, graph[i+1].memory), Color.blue);
					DrawGraphLine(i, m, i/(float)graph.Length, (i+1)/(float)graph.Length, Mathf.InverseLerp(minFPS, maxFPS, graph[i].fps), Mathf.InverseLerp(minFPS, maxFPS, graph[i+1].fps), Color.green);
				}
			}
		}

		/// <summary>
		/// Draws the graph line using the specified index
		/// </summary>
		/// <param name="index">The index</param>
		/// <param name="m">The </param>
		/// <param name="x1">The </param>
		/// <param name="x2">The </param>
		/// <param name="y1">The </param>
		/// <param name="y2">The </param>
		/// <param name="color">The color</param>
		void DrawGraphLine (int index, Matrix4x4 m, float x1, float x2, float y1, float y2, Color color) {
			Debug.DrawLine(cam.ScreenToWorldPoint(m.MultiplyPoint3x4(new Vector3(x1, y1))), cam.ScreenToWorldPoint(m.MultiplyPoint3x4(new Vector3(x2, y2))), color);
		}

		/// <summary>
		/// Ons the gui
		/// </summary>
		public void OnGUI () {
			if (!show || (!Application.isPlaying && !showInEditor)) return;

			if (style == null) {
				style = new GUIStyle();
				style.normal.textColor = Color.white;
				style.padding = new RectOffset(5, 5, 5, 5);
			}

			if (Time.realtimeSinceStartup - lastUpdate > 0.5f || cachedText == null || !Application.isPlaying) {
				lastUpdate = Time.realtimeSinceStartup;

				boxRect = new Rect(5, yOffset, 310, 40);

				text.Length = 0;
				text.AppendLine("A* Pathfinding Project Debugger");
				text.Append("A* Version: ").Append(AstarPath.Version.ToString());

				if (showMemProfile) {
					boxRect.height += 200;

					text.AppendLine();
					text.AppendLine();
					text.Append("Currently allocated".PadRight(25));
					text.Append((allocMem/1000000F).ToString("0.0 MB"));
					text.AppendLine();

					text.Append("Peak allocated".PadRight(25));
					text.Append((peakAlloc/1000000F).ToString("0.0 MB")).AppendLine();

					text.Append("Last collect peak".PadRight(25));
					text.Append((collectAlloc/1000000F).ToString("0.0 MB")).AppendLine();


					text.Append("Allocation rate".PadRight(25));
					text.Append((allocRate/1000000F).ToString("0.0 MB")).AppendLine();

					text.Append("Collection frequency".PadRight(25));
					text.Append(delta.ToString("0.00"));
					text.Append("s\n");

					text.Append("Last collect fps".PadRight(25));
					text.Append((1F/lastDeltaTime).ToString("0.0 fps"));
					text.Append(" (");
					text.Append(lastDeltaTime.ToString("0.000 s"));
					text.Append(")");
				}

				if (showFPS) {
					text.AppendLine();
					text.AppendLine();
					var delayedFPS = delayedDeltaTime > 0.00001f ? 1F/delayedDeltaTime : 0;
					text.Append("FPS".PadRight(25)).Append(delayedFPS.ToString("0.0 fps"));


					float minFps = Mathf.Infinity;

					for (int i = 0; i < fpsDrops.Length; i++) if (fpsDrops[i] < minFps) minFps = fpsDrops[i];

					text.AppendLine();
					text.Append(("Lowest fps (last " + fpsDrops.Length + ")").PadRight(25)).Append(minFps.ToString("0.0"));
				}

				if (showPathProfile) {
					AstarPath astar = AstarPath.active;

					text.AppendLine();

					if (astar == null) {
						text.Append("\nNo AstarPath Object In The Scene");
					} else {
#if ProfileAstar
						double searchSpeed = (double)AstarPath.TotalSearchedNodes*10000 / (double)AstarPath.TotalSearchTime;
						text.Append("\nSearch Speed	(nodes/ms)	").Append(searchSpeed.ToString("0")).Append(" ("+AstarPath.TotalSearchedNodes+" / ").Append(((double)AstarPath.TotalSearchTime/10000F).ToString("0")+")");
#endif

						if (Pathfinding.Util.ListPool<Vector3>.GetSize() > maxVecPool) maxVecPool = Pathfinding.Util.ListPool<Vector3>.GetSize();
						if (Pathfinding.Util.ListPool<Pathfinding.GraphNode>.GetSize() > maxNodePool) maxNodePool = Pathfinding.Util.ListPool<Pathfinding.GraphNode>.GetSize();

						text.Append("\nPool Sizes (size/total created)");

						for (int i = 0; i < debugTypes.Length; i++) {
							debugTypes[i].Print(text);
						}
					}
				}

				cachedText = text.ToString();
			}


			if (font != null) {
				style.font = font;
				style.fontSize = fontSize;
			}

			boxRect.height = style.CalcHeight(new GUIContent(cachedText), boxRect.width);

			GUI.Box(boxRect, "");
			GUI.Label(boxRect, cachedText, style);

			if (showGraph) {
				float minMem = float.PositiveInfinity, maxMem = 0, minFPS = float.PositiveInfinity, maxFPS = 0;
				for (int i = 0; i < graph.Length; i++) {
					minMem = Mathf.Min(graph[i].memory, minMem);
					maxMem = Mathf.Max(graph[i].memory, maxMem);
					minFPS = Mathf.Min(graph[i].fps, minFPS);
					maxFPS = Mathf.Max(graph[i].fps, maxFPS);
				}

				float line;
				GUI.color = Color.blue;
				// Round to nearest x.x MB
				line = Mathf.RoundToInt(maxMem/(100.0f*1000));
				GUI.Label(new Rect(5, Screen.height - AstarMath.MapTo(minMem, maxMem, 0 + graphOffset, graphHeight + graphOffset, line*1000*100) - 10, 100, 20), (line/10.0f).ToString("0.0 MB"));

				line = Mathf.Round(minMem/(100.0f*1000));
				GUI.Label(new Rect(5, Screen.height - AstarMath.MapTo(minMem, maxMem, 0 + graphOffset, graphHeight + graphOffset, line*1000*100) - 10, 100, 20), (line/10.0f).ToString("0.0 MB"));

				GUI.color = Color.green;
				// Round to nearest x.x MB
				line = Mathf.Round(maxFPS);
				GUI.Label(new Rect(55, Screen.height - AstarMath.MapTo(minFPS, maxFPS, 0 + graphOffset, graphHeight + graphOffset, line) - 10, 100, 20), line.ToString("0 FPS"));

				line = Mathf.Round(minFPS);
				GUI.Label(new Rect(55, Screen.height - AstarMath.MapTo(minFPS, maxFPS, 0 + graphOffset, graphHeight + graphOffset, line) - 10, 100, 20), line.ToString("0 FPS"));
			}
		}
	}
}
