#if UNITY_EDITOR
using System;

using MitchCroft.Helpers;

using UnityEngine;
using UnityEditor;

namespace MitchCroft.SerializedData.EditorInspector {
    /// <summary>
    /// Handle the displaying of <see cref="Data"/> values within the Inspector
    /// </summary>
    public class DefaultDataDrawer : IDataDrawer {
        /*----------Variables----------*/
        //SHARED

        /// <summary>
        /// Cache the label to use if there is no SerializedProperty instance supplied to the drawer
        /// </summary>
        private static readonly GUIContent NON_SERIALISED_LABEL = new GUIContent("Invalid Property", "The specified SerializedProperty could not be found");

        /*----------Properties----------*/
        //PUBLIC

        /// <summary>
        /// The type of data that is currently being displayed
        /// </summary>
        public Type DataType { get; set; }

        /// <summary>
        /// The modifier that is currently assigned for display
        /// </summary>
        public IGenericDataModifier Modifier { get; set; }

        /*----------Functions----------*/
        //PUBLIC
        
        /// <summary>
        /// Get the height within the inspector that is required to display this value
        /// </summary>
        /// <param name="property">The Serialized Value property that is to be displayed</param>
        /// <param name="label">The label that has been assigned to the property to be displayed</param>
        /// <returns>Returns the height required to display in pixels</returns>
        public virtual float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return (property != null ?
                EditorGUI.GetPropertyHeight(property) :
                EditorHelper.StandardLineVerticalTotal
            );
        }

        /// <summary>
        /// Display the specified property within the inspector for modification as needed
        /// </summary>
        /// <param name="position">The position within the inspector window to display the information</param>
        /// <param name="property">The Serialized Value property that is to be displayed</param>
        /// <param name="label">The label that has been assigned to the property to be displayed</param>
        public virtual void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            // Using BeginProperty / EndProperty on the parent property means that
            // prefab override logic works on the entire property.
            EditorGUI.BeginProperty(position, label, property);

            // Check that there is a property to be displayed
            if (property != null) EditorGUI.PropertyField(position, property, label, true);

            // Otherwise, display the error message
            else EditorGUI.LabelField(
                new Rect(position.x, position.y, position.width, EditorHelper.StandardLineHeight),
                label,
                NON_SERIALISED_LABEL
            );

            // End of the property being displayed
            EditorGUI.EndProperty();
        }
    }
}
#endif