#if UNITY_EDITOR
using System;

using UnityEditor;
using UnityEngine;

namespace MitchCroft.SerializedData.EditorInspector.Management {
    /// <summary>
    /// Display a warning message that the specified data type can't be serialised
    /// </summary>
    internal sealed class NonSerialDataDrawer : IDataDrawer {
        /*----------Variables----------*/
        //PRIVATE

        /// <summary>
        /// Simple message the informs the user that this data value can't be serialised for display
        /// </summary>
        private GUIContent nonSerialLabel;

        /*----------Properties----------*/
        //PUBLIC

        /// <summary>
        /// The Data Type that won't be serialised
        /// </summary>
        public Type DataType { get; set; }

        /// <summary>
        /// Ignored Modifier, irrelevant to type serialisation
        /// </summary>
        public IGenericDataModifier Modifier { get; set; }

        /*----------Functions----------*/
        //PUBLIC

        /// <summary>
        /// Initialise the display elements that are needed to show
        /// </summary>
        public NonSerialDataDrawer() {
            nonSerialLabel = new GUIContent("Can't be serialized", "Unable to serialize the data in this object for display/storage/modification");
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
        /// Display the warning message within the inspector at the required position
        /// </summary>
        /// <param name="position">The position within the inspector window to display the information</param>
        /// <param name="property">[Ignored] The Serialized Value property that is to be displayed</param>
        /// <param name="label">The label that has been assigned to the property to be displayed</param>
        public void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            EditorGUI.LabelField(
                position,
                label,
                nonSerialLabel
            );
        }
    }
}
#endif