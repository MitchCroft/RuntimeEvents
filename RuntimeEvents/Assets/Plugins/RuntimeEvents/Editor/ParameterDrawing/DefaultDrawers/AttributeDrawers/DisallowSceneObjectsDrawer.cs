using UnityEditor;
using UnityEngine;

namespace RuntimeEvents.ParameterProcessors {
    /// <summary>
    /// Provide the functionality required to display Unity Object options within the Runtime Event inspector without the option to use scene objects
    /// </summary>
    [CustomParameterDrawer(typeof(DisallowSceneObjectsAttribute))]
    public sealed class DisallowSceneObjectsDrawer : AErrorReportingDrawer {
        /*----------Functions----------*/
        //PUBLIC

        /// <summary>
        /// Unity Object drawing is a simple process, only requires a single line
        /// </summary>
        /// <param name="parameterCaches">The parameter caches for the currently selected objects that store the current values of this parameter</param>
        /// <param name="label">The label that is attached to this parameter to be displayed</param>
        /// <returns>Returns a single line height for this value type</returns>
        public override float GetDrawerHeight(PersistentParameterCache[] parameterCaches, GUIContent label) {
            //This needs to be processing a UnityEngine.Object type
            if (typeof(UnityEngine.Object).IsAssignableFrom(Processing))
                return EditorGUIUtility.singleLineHeight;

            //Otherwise, error message logging
            return GetErrorHeight();
        }

        /// <summary>
        /// Display the current Unity Object value setting to the inspector
        /// </summary>
        /// <param name="position">The position within the inspector that the UI elements should be displayed</param>
        /// <param name="parameterCaches">The parameter caches for the currently selected objects that store the current values of this parameter</param>
        /// <param name="label">The label that is attached to this parameter to be displayed</param>
        /// <returns>Returns true if an event occurred that requires the updating of cached values</returns>
        public override bool DisplayParameterValue(Rect position, PersistentParameterCache[] parameterCaches, GUIContent label) {
            //If we are not processing a UnityEngine.Object type, error report it
            if (!typeof(UnityEngine.Object).IsAssignableFrom(Processing))
                return DrawErrorMessage(position, label, "DisallowSceneObjects Attribute is only usable on UnityEngine.Object types");

            //Cast the value to a Unity Engine object
            UnityEngine.Object obj = (UnityEngine.Object)parameterCaches[0].Value;

            //Check if the contained values are different
            bool isDifferent = false;
            if (parameterCaches.Length > 1) {
                for (int i = 1; i < parameterCaches.Length; i++) {
                    //Retrieve this entries value
                    object newVal = parameterCaches[1].Value;

                    //If the values are different, flag it
                    if ((UnityEngine.Object)newVal != obj) {
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
                UnityEngine.Object newObj = EditorGUI.ObjectField(position, label, obj, Processing, false);

                //If the value changed, apply it 
                if (EditorGUI.EndChangeCheck()) {
                    modified = true;
                    for (int i = 0; i < parameterCaches.Length; i++) 
                        parameterCaches[i].SetValue(newObj, Processing);
                }
            }

            //Return the modified flag
            return modified;
        }
    }
}