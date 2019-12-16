using UnityEditor;
using UnityEngine;

namespace RuntimeEvents.ParameterProcessors {
    /// <summary>
    /// Provide the functionality required to display Quaternion options within the Runtime Event inspector
    /// </summary>
    [CustomParameterDrawer(typeof(Quaternion))]
    public sealed class QuaternionDrawer : AParameterDrawer {
        /*----------Functions----------*/
        //PUBLIC

        /// <summary>
        /// Quaternion drawing is a simple process, only requires a single line
        /// </summary>
        /// <param name="parameterCaches">The parameter caches for the currently selected objects that store the current values of this parameter</param>
        /// <param name="label">The label that is attached to this parameter to be displayed</param>
        /// <returns>Returns a single line height for this value type</returns>
        public override float GetDrawerHeight(PersistentParameterCache[] parameterCaches, GUIContent label) { return EditorGUIUtility.singleLineHeight; }

        /// <summary>
        /// Display the current Quaternion value setting to the inspector
        /// </summary>
        /// <param name="position">The position within the inspector that the UI elements should be displayed</param>
        /// <param name="parameterCaches">The parameter caches for the currently selected objects that store the current values of this parameter</param>
        /// <param name="label">The label that is attached to this parameter to be displayed</param>
        /// <returns>Returns true if an event occurred that requires the updating of cached values</returns>
        public override bool DisplayParameterValue(Rect position, PersistentParameterCache[] parameterCaches, GUIContent label) {
            //Cast the value to a Quaternion
            Quaternion quaternion = (Quaternion)parameterCaches[0].Value;

            //Check if the contained values are different
            bool isDifferent = false;
            if (parameterCaches.Length > 1) {
                for (int i = 1; i < parameterCaches.Length; i++) {
                    //Retrieve this entries value
                    object newVal = parameterCaches[i].Value;

                    //If the values are different, flag it
                    if ((Quaternion)newVal != quaternion) {
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

                //Show the Quaternion as a Vector field
                Vector4 newVector = EditorGUI.Vector4Field(position, label, new Vector4(quaternion.x, quaternion.y, quaternion.z, quaternion.w));

                //If the value changed, apply it 
                if (EditorGUI.EndChangeCheck()) {
                    //Create a quaternion from the vector value
                    quaternion = new Quaternion(newVector.x, newVector.y, newVector.z, newVector.w);

                    //Set the new value
                    modified = true;
                    for (int i = 0; i < parameterCaches.Length; i++) 
                        parameterCaches[i].SetValue(quaternion, Processing);
                }
            }

            //Return the modified flag
            return modified;
        }
    }
}