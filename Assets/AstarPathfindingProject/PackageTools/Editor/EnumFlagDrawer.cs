using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Pathfinding {
	/// <summary>
	/// The enum flag drawer class
	/// </summary>
	/// <seealso cref="PropertyDrawer"/>
	[CustomPropertyDrawer(typeof(EnumFlagAttribute))]
	public class EnumFlagDrawer : PropertyDrawer {
		/// <summary>
		/// Ons the gui using the specified position
		/// </summary>
		/// <param name="position">The position</param>
		/// <param name="property">The property</param>
		/// <param name="label">The label</param>
		public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) {
			Enum targetEnum = GetBaseProperty<Enum>(property);

			EditorGUI.BeginProperty(position, label, property);
			EditorGUI.BeginChangeCheck();
#if UNITY_2017_3_OR_NEWER
			Enum enumNew = EditorGUI.EnumFlagsField(position, label, targetEnum);
#else
			Enum enumNew = EditorGUI.EnumMaskField(position, label, targetEnum);
#endif
			if (EditorGUI.EndChangeCheck() || !property.hasMultipleDifferentValues) {
				property.intValue = (int)Convert.ChangeType(enumNew, targetEnum.GetType());
			}
			EditorGUI.EndProperty();
		}

		/// <summary>
		/// Gets the base property using the specified prop
		/// </summary>
		/// <typeparam name="T">The </typeparam>
		/// <param name="prop">The prop</param>
		/// <returns>The</returns>
		static T GetBaseProperty<T>(SerializedProperty prop) {
			// Separate the steps it takes to get to this property
			string[] separatedPaths = prop.propertyPath.Split('.');

			// Go down to the root of this serialized property
			System.Object reflectionTarget = prop.serializedObject.targetObject as object;
			// Walk down the path to get the target object
			foreach (var path in separatedPaths) {
				FieldInfo fieldInfo = reflectionTarget.GetType().GetField(path, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				reflectionTarget = fieldInfo.GetValue(reflectionTarget);
			}
			return (T)reflectionTarget;
		}
	}
}
