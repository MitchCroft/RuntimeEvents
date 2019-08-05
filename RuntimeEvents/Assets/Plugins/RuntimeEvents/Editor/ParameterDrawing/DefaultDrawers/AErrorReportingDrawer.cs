using UnityEditor;
using UnityEngine;

namespace RuntimeEvents.ParameterProcessors {
    /// <summary>
    /// Define a number of shared functions that can be used to easily report errors within the inspector
    /// </summary>
    public abstract class AErrorReportingDrawer : AParameterDrawer {
        /*----------Functions----------*/
        //PROTECTED

        /// <summary>
        /// Retrieve the height required for a drawer that is being used to display an error message
        /// </summary>
        /// <returns>Returns a constant height required by this drawer in pixels</returns>
        protected float GetErrorHeight() { return EditorGUIUtility.singleLineHeight * 2f; }

        /// <summary>
        /// Display an error message for a specific parameter option within the inspector
        /// </summary>
        /// <param name="position">The position in which the message is to be displayed</param>
        /// <param name="label">The label of the parameter that is going to fail to draw</param>
        /// <param name="errorMessage">An additional detail error message that is displayed below the parameter label</param>
        /// <returns>Returns a constant false as there are no interactable elements to this function</returns>
        protected bool DrawErrorMessage(Rect position, GUIContent label, string errorMessage) {
            //Display the label of this element within the inspector
            EditorGUI.LabelField(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), label, EditorStyles.boldLabel);

            //Display the error message
            EditorGUI.LabelField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight), errorMessage);

            //Nothing updates on this drawer
            return false;
        }
    }
}