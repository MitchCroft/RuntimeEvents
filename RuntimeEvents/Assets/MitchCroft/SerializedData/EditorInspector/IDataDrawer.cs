#if UNITY_EDITOR
using System;

using UnityEngine;
using UnityEditor;

namespace MitchCroft.SerializedData.EditorInspector {
    /// <summary>
    /// Handle the displaying of <see cref="Data"/> values within the inspector
    /// </summary>
    public interface IDataDrawer {
        /*----------Properties----------*/
        //PUBLIC

        /// <summary>
        /// The type of data that is currently being displayed
        /// </summary>
        Type DataType { get; set; }

        /// <summary>
        /// The modifier that is currently assigned for display
        /// </summary>
        IGenericDataModifier Modifier { get; set; }

        /*----------Functions----------*/
        //PUBLIC

        /// <summary>
        /// Get the height within the inspector that is required to display this value
        /// </summary>
        /// <param name="property">The Serialized Value property that is to be displayed</param>
        /// <param name="label">The label that has been assigned to the property to be displayed</param>
        /// <returns>Returns the height required to display in pixels</returns>
        float GetPropertyHeight(SerializedProperty property, GUIContent label);

        /// <summary>
        /// Display the specified property within the inspector for modification as needed
        /// </summary>
        /// <param name="position">The position within the inspector window to display the information</param>
        /// <param name="property">The Serialized Value property that is to be displayed</param>
        /// <param name="label">The label that has been assigned to the property to be displayed</param>
        void OnGUI(Rect position, SerializedProperty property, GUIContent label);
    }
}
#endif