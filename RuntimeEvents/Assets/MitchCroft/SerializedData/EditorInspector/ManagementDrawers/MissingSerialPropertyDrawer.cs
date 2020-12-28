#if UNITY_EDITOR
using System;

using UnityEditor;
using UnityEngine;

namespace MitchCroft.SerializedData.EditorInspector.Management {
    /// <summary>
    /// Display a warning message within the inspector about a received null serialized property
    /// </summary>
    internal sealed class MissingSerialPropertyDrawer : IDataDrawer {
        /*----------Variables----------*/
        //PRIVATE

        /// <summary>
        /// Simple GUI label that will be displayed to inform that there was no property to be displayed
        /// </summary>
        private GUIContent nullLabel;

        /*----------Properties----------*/
        //PUBLIC

        /// <summary>
        /// Ignored Data Type, no serialised property to be display
        /// </summary>
        public Type DataType { get; set; }

        /// <summary>
        /// Ignored Modifier, no serialised property to be displayed
        /// </summary>
        public IGenericDataModifier Modifier { get; set; }

        /*----------Functions----------*/
        //PUBLIC

        /// <summary>
        /// Initialise the display elements that are needed to show
        /// </summary>
        public MissingSerialPropertyDrawer() {
            nullLabel = new GUIContent("No Serialized Property", "Unable to retrieve a SerializedProperty object from the specified data storage object");
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
        /// Display the non-serialised message within the inspector at the required position
        /// </summary>
        /// <param name="position">The position within the inspector window to display the information</param>
        /// <param name="property">[Ignored] The Serialized Value property that is to be displayed</param>
        /// <param name="label">The label that has been assigned to the property to be displayed</param>
        public void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            EditorGUI.LabelField(
                position,
                label,
                nullLabel
            );
        }
    }
}
#endif