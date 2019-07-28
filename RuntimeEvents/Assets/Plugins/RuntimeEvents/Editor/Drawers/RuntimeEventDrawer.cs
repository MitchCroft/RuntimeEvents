using System;

using UnityEngine;
using UnityEditor;

namespace RuntimeEvents {
    /// <summary>
    /// 
    /// </summary>
    [CustomPropertyDrawer(typeof(RuntimeEventBase), true)]
    public sealed class RuntimeEventDrawer : PropertyDrawer {
        /*----------Variables----------*/
        //INVISIBLE



        //VISIBLE



        /*----------Properties----------*/
        //PRIVATE



        //PUBLIC



        /*----------Functions----------*/
        //PRIVATE

        /// <summary>
        /// Retrieve the height of the area within the inspector that this property will take up
        /// </summary>
        /// <param name="property">The property that is to be displayed within the inspector</param>
        /// <param name="label">The label that has been assigned to the property</param>
        /// <returns>Returns the required height of the inspector window for this property in pixels</returns>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            //Get the base object
            RuntimeEventBase obj;
            property.GetPropertyValue(out obj);

            return EditorGUIUtility.singleLineHeight * Mathf.Max(1, obj.DynamicTypes.Count);
        }

        /// <summary>
        /// Display the elements of the property within the designated area on the inspector area
        /// </summary>
        /// <param name="position">The position within the inspector that the property should be drawn to</param>
        /// <param name="property">The property that is to be displayed within the inspector</param>
        /// <param name="label">The label that has been assigned to the property</param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            //Get the base object
            RuntimeEventBase obj;
            property.GetPropertyValue(out obj);

            //Check there are dynamic types
            if (obj.DynamicTypes.Count == 0) {
                EditorGUI.LabelField(position, label, new GUIContent("No Dynamic Parameters"));
            } else {
                int prog = 0;
                foreach (Type type in obj.DynamicTypes) {
                    EditorGUI.LabelField(new Rect(position.x, position.y + prog * EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight), label, new GUIContent(type.FullName));
                    ++prog;
                }
            }
        }
    }
}