using UnityEditor;
using UnityEngine;

namespace RuntimeEvents.ParameterProcessors {
    /// <summary>
    /// Provide the functionality required to display color options within the Runtime Event inspector
    /// </summary>
    [CustomParameterDrawer(typeof(Color))]
    public sealed class ColorDrawer : AParameterDrawer {
        /*----------Functions----------*/
        //PUBLIC

        /// <summary>
        /// Color drawing is a simple process, only requires a single line
        /// </summary>
        /// <param name="parameterCaches">The parameter caches for the currently selected objects that store the current values of this parameter</param>
        /// <param name="label">The label that is attached to this parameter to be displayed</param>
        /// <returns>Returns a single line height for this value type</returns>
        public override float GetDrawerHeight(PersistentParameterCache[] parameterCaches, GUIContent label) {
            return EditorGUIUtility.singleLineHeight;
        }

        /// <summary>
        /// Display the current color value setting to the inspector
        /// </summary>
        /// <param name="position">The position within the inspector that the UI elements should be displayed</param>
        /// <param name="parameterCaches">The parameter caches for the currently selected objects that store the current values of this parameter</param>
        /// <param name="label">The label that is attached to this parameter to be displayed</param>
        /// <returns>Returns true if an event occurred that requires the updating of cached values</returns>
        public override bool DisplayParameterValue(Rect position, PersistentParameterCache[] parameterCaches, GUIContent label) {
            //Get the value of the primary cache object
            object currentObj;
            if (!Processor.GetValue(parameterCaches[0], out currentObj)) {
                EditorGUI.LabelField(position, label, new GUIContent("Failed to retrieve current value from processor"));
                return false;
            }

            //Cast the value to a color
            Color color = (Color)currentObj;

            //Check if the contained values are different
            bool isDifferent = false;
            if (parameterCaches.Length > 1) {
                for (int i = 1; i < parameterCaches.Length; i++) {
                    //Retrieve this entries value
                    object newVal;
                    if (!Processor.GetValue(parameterCaches[i], out newVal)) {
                        EditorGUI.LabelField(position, label, new GUIContent("Failed to retrieve current value from processor"));
                        return false;
                    }

                    //If the values are different, flag it
                    if ((Color)newVal != color) {
                        isDifferent = true;
                        break;
                    }
                }
            }

            //Check if the element has changed
            bool modified = false;

            //Setup the UI if this is a mixed value
            using (GUIMixer.PushSegment(isDifferent)) {
                //Begin looking for changes
                EditorGUI.BeginChangeCheck();

                //Show the color field
                Color newColor = EditorGUI.ColorField(position, label, color);

                //If the value changed, apply it 
                if (EditorGUI.EndChangeCheck()) {
                    modified = true;
                    for (int i = 0; i < parameterCaches.Length; i++) {
                        if (!Processor.AssignValue(parameterCaches[i], newColor))
                            Debug.LogErrorFormat("Failed to assign the new state value '{0}' to the parameter cache at index {1}", newColor, i);
                    }
                }
            }

            //Return the modified flag
            return modified;
        }
    }
}