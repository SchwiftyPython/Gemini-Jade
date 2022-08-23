using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Pathfinding {
	/// <summary>Helper for creating editors</summary>
	[CustomEditor(typeof(VersionedMonoBehaviour), true)]
	[CanEditMultipleObjects]
	public class EditorBase : Editor {
		/// <summary>
		/// The cached tooltips
		/// </summary>
		static System.Collections.Generic.Dictionary<string, string> cachedTooltips;
		/// <summary>
		/// The cached ur ls
		/// </summary>
		static System.Collections.Generic.Dictionary<string, string> cachedURLs;
		/// <summary>
		/// The serialized property
		/// </summary>
		Dictionary<string, SerializedProperty> props = new Dictionary<string, SerializedProperty>();
		/// <summary>
		/// The dictionary
		/// </summary>
		Dictionary<string, string> localTooltips = new Dictionary<string, string>();

		/// <summary>
		/// The gui content
		/// </summary>
		static GUIContent content = new GUIContent();
		/// <summary>
		/// The gui content
		/// </summary>
		static GUIContent showInDocContent = new GUIContent("Show in online documentation", "");
		/// <summary>
		/// The gui layout option
		/// </summary>
		static GUILayoutOption[] noOptions = new GUILayoutOption[0];
		/// <summary>
		/// The get documentation url
		/// </summary>
		public static System.Func<string> getDocumentationURL;

		/// <summary>
		/// Loads the meta
		/// </summary>
		static void LoadMeta () {
			if (cachedTooltips == null) {
				var filePath = EditorResourceHelper.editorAssets + "/tooltips.tsv";

				try {
					cachedURLs = System.IO.File.ReadAllLines(filePath).Select(l => l.Split('\t')).Where(l => l.Length == 2).ToDictionary(l => l[0], l => l[1]);
					cachedTooltips = new System.Collections.Generic.Dictionary<string, string>();
				} catch {
					cachedURLs = new System.Collections.Generic.Dictionary<string, string>();
					cachedTooltips = new System.Collections.Generic.Dictionary<string, string>();
				}
			}
		}

		/// <summary>
		/// Finds the url using the specified type
		/// </summary>
		/// <param name="type">The type</param>
		/// <param name="path">The path</param>
		/// <returns>The string</returns>
		static string FindURL (System.Type type, string path) {
			// Find the correct type if the path was not an immediate member of #type
			while (true) {
				var index = path.IndexOf('.');
				if (index == -1) break;
				var fieldName = path.Substring(0, index);
				var remaining = path.Substring(index + 1);
				var field = type.GetField(fieldName, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
				if (field != null) {
					type = field.FieldType;
					path = remaining;
				} else {
					// Could not find the correct field
					return null;
				}
			}

			// Find a documentation entry for the field, fall back to parent classes if necessary
			while (type != null) {
				var url = FindURL(type.FullName + "." + path);
				if (url != null) return url;
				type = type.BaseType;
			}
			return null;
		}

		/// <summary>
		/// Finds the url using the specified path
		/// </summary>
		/// <param name="path">The path</param>
		/// <returns>The url</returns>
		static string FindURL (string path) {
			LoadMeta();
			string url;
			cachedURLs.TryGetValue(path, out url);
			return url;
		}

		/// <summary>
		/// Finds the tooltip using the specified path
		/// </summary>
		/// <param name="path">The path</param>
		/// <returns>The tooltip</returns>
		static string FindTooltip (string path) {
			LoadMeta();

			string tooltip;
			cachedTooltips.TryGetValue(path, out tooltip);
			return tooltip;
		}

		/// <summary>
		/// Finds the local tooltip using the specified path
		/// </summary>
		/// <param name="path">The path</param>
		/// <returns>The result</returns>
		string FindLocalTooltip (string path) {
			string result;

			if (!localTooltips.TryGetValue(path, out result)) {
				var fullPath = target.GetType().Name + "." + path;
				result = localTooltips[path] = FindTooltip(fullPath);
			}
			return result;
		}

		/// <summary>
		/// Ons the enable
		/// </summary>
		protected virtual void OnEnable () {
			foreach (var target in targets) if (target != null) (target as IVersionedMonoBehaviourInternal).UpgradeFromUnityThread();
		}

		/// <summary>
		/// Ons the inspector gui
		/// </summary>
		public sealed override void OnInspectorGUI () {
			EditorGUI.indentLevel = 0;
			serializedObject.Update();
			try {
				Inspector();
			} catch (System.Exception e) {
				Debug.LogException(e, target);
			}
			serializedObject.ApplyModifiedProperties();
			if (targets.Length == 1 && (target as MonoBehaviour).enabled) {
				var attr = target.GetType().GetCustomAttributes(typeof(UniqueComponentAttribute), true);
				for (int i = 0; i < attr.Length; i++) {
					string tag = (attr[i] as UniqueComponentAttribute).tag;
					foreach (var other in (target as MonoBehaviour).GetComponents<MonoBehaviour>()) {
						if (!other.enabled || other == target) continue;
						if (other.GetType().GetCustomAttributes(typeof(UniqueComponentAttribute), true).Select(c => (c as UniqueComponentAttribute).tag == tag).Any()) {
							EditorGUILayout.HelpBox("This component and " + other.GetType().Name + " cannot be used at the same time", MessageType.Warning);
						}
					}
				}
			}
		}

		/// <summary>
		/// Inspectors this instance
		/// </summary>
		protected virtual void Inspector () {
			// Basically the same as DrawDefaultInspector, but with tooltips
			bool enterChildren = true;

			for (var prop = serializedObject.GetIterator(); prop.NextVisible(enterChildren); enterChildren = false) {
				PropertyField(prop.propertyPath);
			}
		}

		/// <summary>
		/// Finds the property using the specified name
		/// </summary>
		/// <param name="name">The name</param>
		/// <exception cref="System.ArgumentException"></exception>
		/// <returns>The res</returns>
		protected SerializedProperty FindProperty (string name) {
			if (!props.TryGetValue(name, out SerializedProperty res)) res = props[name] = serializedObject.FindProperty(name);
			if (res == null) throw new System.ArgumentException(name);
			return res;
		}

		/// <summary>
		/// Sections the label
		/// </summary>
		/// <param name="label">The label</param>
		protected void Section (string label) {
			EditorGUILayout.Separator();
			EditorGUILayout.LabelField(label, EditorStyles.boldLabel);
		}

		/// <summary>
		/// Floats the field using the specified property path
		/// </summary>
		/// <param name="propertyPath">The property path</param>
		/// <param name="label">The label</param>
		/// <param name="tooltip">The tooltip</param>
		/// <param name="min">The min</param>
		/// <param name="max">The max</param>
		protected void FloatField (string propertyPath, string label = null, string tooltip = null, float min = float.NegativeInfinity, float max = float.PositiveInfinity) {
			PropertyField(propertyPath, label, tooltip);
			Clamp(propertyPath, min, max);
		}

		/// <summary>
		/// Floats the field using the specified prop
		/// </summary>
		/// <param name="prop">The prop</param>
		/// <param name="label">The label</param>
		/// <param name="tooltip">The tooltip</param>
		/// <param name="min">The min</param>
		/// <param name="max">The max</param>
		protected void FloatField (SerializedProperty prop, string label = null, string tooltip = null, float min = float.NegativeInfinity, float max = float.PositiveInfinity) {
			PropertyField(prop, label, tooltip);
			Clamp(prop, min, max);
		}

		/// <summary>
		/// Describes whether this instance property field
		/// </summary>
		/// <param name="propertyPath">The property path</param>
		/// <param name="label">The label</param>
		/// <param name="tooltip">The tooltip</param>
		/// <returns>The bool</returns>
		protected bool PropertyField (string propertyPath, string label = null, string tooltip = null) {
			return PropertyField(FindProperty(propertyPath), label, tooltip, propertyPath);
		}

		/// <summary>
		/// Describes whether this instance property field
		/// </summary>
		/// <param name="prop">The prop</param>
		/// <param name="label">The label</param>
		/// <param name="tooltip">The tooltip</param>
		/// <returns>The bool</returns>
		protected bool PropertyField (SerializedProperty prop, string label = null, string tooltip = null) {
			return PropertyField(prop, label, tooltip, prop.propertyPath);
		}

		/// <summary>
		/// Describes whether this instance property field
		/// </summary>
		/// <param name="prop">The prop</param>
		/// <param name="label">The label</param>
		/// <param name="tooltip">The tooltip</param>
		/// <param name="propertyPath">The property path</param>
		/// <returns>The bool</returns>
		bool PropertyField (SerializedProperty prop, string label, string tooltip, string propertyPath) {
			content.text = label ?? prop.displayName;
			content.tooltip = tooltip ?? FindTooltip(propertyPath);
			var contextClick = IsContextClick();
			EditorGUILayout.PropertyField(prop, content, true, noOptions);
			// Disable context clicking on arrays (as Unity has its own very useful context menu for the array elements)
			if (contextClick && !prop.isArray && Event.current.type == EventType.Used) CaptureContextClick(propertyPath);
			return prop.propertyType == SerializedPropertyType.Boolean ? !prop.hasMultipleDifferentValues && prop.boolValue : true;
		}

		/// <summary>
		/// Describes whether this instance is context click
		/// </summary>
		/// <returns>The bool</returns>
		bool IsContextClick () {
			// Capturing context clicks turned out to be a bad idea.
			// It prevents things like reverting to prefab values and other nice things.
			return false;
			// return Event.current.type == EventType.ContextClick;
		}

		/// <summary>
		/// Captures the context click using the specified property path
		/// </summary>
		/// <param name="propertyPath">The property path</param>
		void CaptureContextClick (string propertyPath) {
			var url = FindURL(target.GetType(), propertyPath);

			if (url != null && getDocumentationURL != null) {
				Event.current.Use();
				var menu = new GenericMenu();
				menu.AddItem(showInDocContent, false, () => Application.OpenURL(getDocumentationURL() + url));
				menu.ShowAsContext();
			}
		}

		/// <summary>
		/// Popups the property path
		/// </summary>
		/// <param name="propertyPath">The property path</param>
		/// <param name="options">The options</param>
		/// <param name="label">The label</param>
		protected void Popup (string propertyPath, GUIContent[] options, string label = null) {
			var prop = FindProperty(propertyPath);

			content.text = label ?? prop.displayName;
			content.tooltip = FindTooltip(propertyPath);
			var contextClick = IsContextClick();
			EditorGUI.BeginChangeCheck();
			EditorGUI.showMixedValue = prop.hasMultipleDifferentValues;
			int newVal = EditorGUILayout.Popup(content, prop.propertyType == SerializedPropertyType.Enum ? prop.enumValueIndex : prop.intValue, options);
			if (EditorGUI.EndChangeCheck()) {
				if (prop.propertyType == SerializedPropertyType.Enum) prop.enumValueIndex = newVal;
				else prop.intValue = newVal;
			}
			EditorGUI.showMixedValue = false;
			if (contextClick && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition)) CaptureContextClick(propertyPath);
		}

		/// <summary>
		/// Masks the property path
		/// </summary>
		/// <param name="propertyPath">The property path</param>
		/// <param name="options">The options</param>
		/// <param name="label">The label</param>
		protected void Mask (string propertyPath, string[] options, string label = null) {
			var prop = FindProperty(propertyPath);

			content.text = label ?? prop.displayName;
			content.tooltip = FindTooltip(propertyPath);
			var contextClick = IsContextClick();
			EditorGUI.BeginChangeCheck();
			EditorGUI.showMixedValue = prop.hasMultipleDifferentValues;
			int newVal = EditorGUILayout.MaskField(content, prop.intValue, options);
			if (EditorGUI.EndChangeCheck()) {
				prop.intValue = newVal;
			}
			EditorGUI.showMixedValue = false;
			if (contextClick && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition)) CaptureContextClick(propertyPath);
		}

		/// <summary>
		/// Ints the slider using the specified property path
		/// </summary>
		/// <param name="propertyPath">The property path</param>
		/// <param name="left">The left</param>
		/// <param name="right">The right</param>
		protected void IntSlider (string propertyPath, int left, int right) {
			var contextClick = IsContextClick();
			var prop = FindProperty(propertyPath);

			content.text = prop.displayName;
			content.tooltip = FindTooltip(propertyPath);
			EditorGUILayout.IntSlider(prop, left, right, content, noOptions);
			if (contextClick && Event.current.type == EventType.Used) CaptureContextClick(propertyPath);
		}

		/// <summary>
		/// Sliders the property path
		/// </summary>
		/// <param name="propertyPath">The property path</param>
		/// <param name="left">The left</param>
		/// <param name="right">The right</param>
		protected void Slider (string propertyPath, float left, float right) {
			var contextClick = IsContextClick();
			var prop = FindProperty(propertyPath);

			content.text = prop.displayName;
			content.tooltip = FindTooltip(propertyPath);
			EditorGUILayout.Slider(prop, left, right, content, noOptions);
			if (contextClick && Event.current.type == EventType.Used) CaptureContextClick(propertyPath);
		}

		/// <summary>
		/// Clamps the prop
		/// </summary>
		/// <param name="prop">The prop</param>
		/// <param name="min">The min</param>
		/// <param name="max">The max</param>
		protected void Clamp (SerializedProperty prop, float min, float max = float.PositiveInfinity) {
			if (!prop.hasMultipleDifferentValues) prop.floatValue = Mathf.Clamp(prop.floatValue, min, max);
		}

		/// <summary>
		/// Clamps the name
		/// </summary>
		/// <param name="name">The name</param>
		/// <param name="min">The min</param>
		/// <param name="max">The max</param>
		protected void Clamp (string name, float min, float max = float.PositiveInfinity) {
			Clamp(FindProperty(name), min, max);
		}

		/// <summary>
		/// Clamps the int using the specified name
		/// </summary>
		/// <param name="name">The name</param>
		/// <param name="min">The min</param>
		/// <param name="max">The max</param>
		protected void ClampInt (string name, int min, int max = int.MaxValue) {
			var prop = FindProperty(name);

			if (!prop.hasMultipleDifferentValues) prop.intValue = Mathf.Clamp(prop.intValue, min, max);
		}
	}
}
