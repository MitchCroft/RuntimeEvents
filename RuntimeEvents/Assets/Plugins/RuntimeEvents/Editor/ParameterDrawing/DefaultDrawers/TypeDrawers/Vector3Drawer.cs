using UnityEditor;
using UnityEngine;

namespace RuntimeEvents.ParameterProcessors {
    /// <summary>
    /// Provide the functionality required to display Vector3 options within the Runtime Event inspector
    /// </summary>
    [CustomParameterDrawer(typeof(Vector3))]
    public sealed class Vector3Drawer : AParameterDrawer {
        /*----------Functions----------*/
        //PUBLIC

        /// <summary>
        /// Vector3 drawing is a simple process, only requires a single line
        /// </summary>
        /// <param name="parameterCaches">The parameter caches for the currently selected objects that store the current values of this parameter</param>
        /// <param name="label">The label that is attached to this parameter to be displayed</param>
        /// <returns>Returns a single line height for this value type</returns>
        public override float GetDrawerHeight(PersistentParameterCache[] parameterCaches, GUIContent label) {
            return EditorGUIUtility.singleLineHeight;
        }

        /// <summary>
        /// Display the current Vector3 value setting to the inspector
        /// </summary>
        /// <param name="position">The position within the inspector that the UI elements should be displayed</param>
        /// <param name="parameterCaches">The parameter caches for the currently selected objects that store the current values of this parameter</param>
        /// <param name="label">The label that is attached to this parameter to be displayed</param>
        /// <returns>Returns true if an event occurred that requires the updating of cached values</returns>
        public override bool DisplayParameterValue(Rect position, PersistentParameterCache[] parameterCaches, GUIContent label) {
            //Cast the value to a Vector3
            Vector3 vector = (Vector3)parameterCaches[0].Value;

            //Check if the contained values are different
            bool isDifferent = false;
            if (parameterCaches.Length > 1) {
                for (int i = 1; i < parameterCaches.Length; i++) {
                    //Retrieve this entries value
                    object newVal = parameterCaches[i].Value;

                    //If the values are different, flag it
                    if ((Vector3)newVal != vector) {
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

                //Show the vector field
                Vector3 newVector = EditorGUI.Vector3Field(position, label, vector);

                //If the value changed, apply it 
                if (EditorGUI.EndChangeCheck()) {
                    modified = true;
                    for (int i = 0; i < parameterCaches.Length; i++) 
                        parameterCaches[i].SetValue(newVector, Processing);
                }
            }

            //Return the modified flag
            return modified;
        }
    }
}