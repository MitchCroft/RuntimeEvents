#if UNITY_EDITOR
using System;

using UnityEngine;
using UnityEditor;

namespace MitchCroft.SerializedData.EditorInspector.DrawerObjects {
    /// <summary>
    /// Handle the displaying of an object field for any <see cref="UnityEngine.Object"/> elements that are needed
    /// </summary>
    [CustomDataDrawer(typeof(UnityEngine.Object), true)]
    public sealed class UnityObjectDrawer : IDataDrawer {
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
        public float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return EditorGUIUtility.singleLineHeight;
        }

        /// <summary>
        /// Display the specified property within the inspector for modification as needed
        /// </summary>
        /// <param name="position">The position within the inspector window to display the information</param>
        /// <param name="property">The Serialized Value property that is to be displayed</param>
        /// <param name="label">The label that has been assigned to the property to be displayed</param>
        public void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            // Using BeginProperty / EndProperty on the parent property means that
            // prefab override logic works on the entire property.
            EditorGUI.BeginProperty(position, label, property);

            // Offer the object field for assignment of the new value
            property.objectReferenceValue = EditorGUI.ObjectField(
                position,
                label,
                property.objectReferenceValue,
                DataType,
                true
            );

            // End of the property being displayed
            EditorGUI.EndProperty();
        }
    }
}
#endif