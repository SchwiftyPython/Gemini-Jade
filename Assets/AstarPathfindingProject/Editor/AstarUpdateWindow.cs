using System;
using UnityEditor;
using UnityEngine;

namespace Pathfinding {
	/// <summary>
	/// The astar update window class
	/// </summary>
	/// <seealso cref="EditorWindow"/>
	public class AstarUpdateWindow : EditorWindow {
		/// <summary>
		/// The large style
		/// </summary>
		static GUIStyle largeStyle;
		/// <summary>
		/// The normal style
		/// </summary>
		static GUIStyle normalStyle;
		/// <summary>
		/// The version
		/// </summary>
		Version version;
		/// <summary>
		/// The summary
		/// </summary>
		string summary;
		/// <summary>
		/// The set reminder
		/// </summary>
		bool setReminder;

		/// <summary>
		/// Inits the version
		/// </summary>
		/// <param name="version">The version</param>
		/// <param name="summary">The summary</param>
		/// <returns>The window</returns>
		public static AstarUpdateWindow Init (Version version, string summary) {
			// Get existing open window or if none, make a new one:
			AstarUpdateWindow window = EditorWindow.GetWindow<AstarUpdateWindow>(true, "", true);

			window.position = new Rect(Screen.currentResolution.width/2 - 300, Mathf.Max(5, Screen.currentResolution.height/3 - 150), 600, 400);
			window.version = version;
			window.summary = summary;
#if UNITY_4_6 || UNITY_5_0
			window.title = "New Version of the A* Pathfinding Project";
#else
			window.titleContent = new GUIContent("New Version of the A* Pathfinding Project");
#endif
			return window;
		}

		/// <summary>
		/// Ons the destroy
		/// </summary>
		public void OnDestroy () {
			if (version != null && !setReminder) {
				Debug.Log("Closed window, reminding again tomorrow");
				EditorPrefs.SetString("AstarRemindUpdateDate", DateTime.UtcNow.AddDays(1).ToString(System.Globalization.CultureInfo.InvariantCulture));
				EditorPrefs.SetString("AstarRemindUpdateVersion", version.ToString());
			}
		}

		/// <summary>
		/// Ons the gui
		/// </summary>
		void OnGUI () {
			if (largeStyle == null) {
				largeStyle = new GUIStyle(EditorStyles.largeLabel);
				largeStyle.fontSize = 32;
				largeStyle.alignment = TextAnchor.UpperCenter;
				largeStyle.richText = true;

				normalStyle = new GUIStyle(EditorStyles.label);
				normalStyle.wordWrap = true;
				normalStyle.richText = true;
			}

			if (version == null) {
				return;
			}

			GUILayout.Label("New Update Available!", largeStyle);
			GUILayout.Label("There is a new version of the <b>A* Pathfinding Project</b> available for download.\n" +
				"The new version is <b>" + version + "</b> you have <b>" + AstarPath.Version + "</b>\n\n"+
				"<i>Summary:</i>\n"+summary, normalStyle
				);

			GUILayout.FlexibleSpace();

			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();

			GUILayout.BeginVertical();

			Color col = GUI.color;
			GUI.backgroundColor *= new Color(0.5f,  1f, 0.5f);
			if (GUILayout.Button("Take me to the download page!", GUILayout.Height(30), GUILayout.MaxWidth(300))) {
				Application.OpenURL(AstarUpdateChecker.GetURL("download"));
			}
			GUI.backgroundColor = col;


			if (GUILayout.Button("What's new? (full changelog)")) {
				Application.OpenURL(AstarUpdateChecker.GetURL("changelog"));
			}

			GUILayout.EndVertical();

			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();

			GUILayout.FlexibleSpace();

			GUILayout.BeginHorizontal();

			if (GUILayout.Button("Skip this version", GUILayout.MaxWidth(100))) {
				EditorPrefs.SetString("AstarSkipUpToVersion", version.ToString());
				setReminder = true;
				Close();
			}

			if (GUILayout.Button("Remind me later ( 1 week )", GUILayout.MaxWidth(200))) {
				EditorPrefs.SetString("AstarRemindUpdateDate", DateTime.UtcNow.AddDays(7).ToString(System.Globalization.CultureInfo.InvariantCulture));
				EditorPrefs.SetString("AstarRemindUpdateVersion", version.ToString());
				setReminder = true;
				Close();
			}

			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}
	}
}
