#if UNITY_EDITOR
using System;

using UnityEditor;
using UnityEngine;

namespace MitchCroft.SerializedData.EditorInspector.Management {
    /// <summary>
    /// Display a warning message within the inspector about mixed data that can't be modified
    /// </summary>
    internal sealed class TypeMismatchDataDrawer : IDataDrawer {
        /*----------Variables----------*/
        //PRIVATE

        /// <summary>
        /// Simple GUI label that will be displayed to inform that data can't be can't be displayed
        /// </summary>
        private GUIContent mixedLabel;

        /*----------Properties----------*/
        //PUBLIC

        /// <summary>
        /// Ignored Data Type, mixed data could have multiple types
        /// </summary>
        public Type DataType { get; set; }

        /// <summary>
        /// Ignored Modifier, mixed data could have multiple modifiers
        /// </summary>
        public IGenericDataModifier Modifier { get; set; }

        /*----------Functions----------*/
        //PUBLIC

        /// <summary>
        /// Initialise the display elements that are needed to show
        /// </summary>
        public TypeMismatchDataDrawer() {
            mixedLabel = new GUIContent("Can't display mixed data", "The Data objects to be displayed have different combinations of Types and/or Modifiers");
        }

        /// <summary>
        /// Get the height within the inspector that is required to display this value
        /// </summary>
        /// <param name="property">[Ignored] The Serialized Value property that is to be displayed</param>
        /// <param name="label">[Ignored] The label that has been assigned to the property to be displayed</param>
        /// <returns>Returns the height required to display in pixels</returns>
        public float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return EditorGUIUtility.singleLineHeight;
        }

        /// <summary>
        /// Display the mismatched message within the inspector at the required position
        /// </summary>
        /// <param name="position">The position within the inspector window to display the information</param>
        /// <param name="property">[Ignored] The Serialized Value property that is to be displayed</param>
        /// <param name="label">The label that has been assigned to the property to be displayed</param>
        public void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            EditorGUI.LabelField(
                position,
                label,
                mixedLabel
            );
        }
    }
}
#endif