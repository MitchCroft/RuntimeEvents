using UnityEditor;
using UnityEngine;

namespace RuntimeEvents.ParameterProcessors {
    /// <summary>
    /// Provide the functionality required to display maximum bounded numerical options within the Runtime Event inspector
    /// </summary>
    [CustomParameterDrawer(typeof(NumericMaxAttribute))]
    public sealed class NumericMaxDrawer : AErrorReportingDrawer {
        /*----------Functions----------*/
        //PUBLIC

        /// <summary>
        /// Field drawing is a simple process, only requires a single line
        /// </summary>
        /// <param name="parameterCaches">The parameter caches for the currently selected objects that store the current values of this parameter</param>
        /// <param name="label">The label that is attached to this parameter to be displayed</param>
        /// <returns>Returns a single line height for this value type</returns>
        public override float GetDrawerHeight(PersistentParameterCache[] parameterCaches, GUIContent label) {
            //Check that the type is right
            if (Processing == typeof(int) || Processing == typeof(float))
                return EditorGUIUtility.singleLineHeight;

            //Otherwise, invalid type
            return GetErrorHeight();
        }

        /// <summary>
        /// Display the current numeric value setting to the inspector
        /// </summary>
        /// <param name="position">The position within the inspector that the UI elements should be displayed</param>
        /// <param name="parameterCaches">The parameter caches for the currently selected objects that store the current values of this parameter</param>
        /// <param name="label">The label that is attached to this parameter to be displayed</param>
        /// <returns>Returns true if an event occurred that requires the updating of cached values</returns>
        public override bool DisplayParameterValue(Rect position, PersistentParameterCache[] parameterCaches, GUIContent label) {
            //Check that the type is valid
            if (Processing != typeof(int) && Processing != typeof(float))
                return DrawErrorMessage(position, label, "NumericMin Attribute is only usable on int/float types");

            //Get the value of the primary cache object
            object currentObj = parameterCaches[0].Value;

            //Check if the element has changed
            bool modified = false;

            //Get the attribute value 
            NumericMaxAttribute att = Attribute as NumericMaxAttribute;

            //Process the value as a float
            if (currentObj is float) {
                //Cast the value to a float
                float val = (float)currentObj;

                //Check if the contained values are different
                bool isDifferent = false;
                if (parameterCaches.Length > 1) {
                    for (int i = 1; i < parameterCaches.Length; i++) {
                        //Retrieve this entries value
                        object newVal = parameterCaches[i].Value;

                        //If the values are different, flag it
                        if ((float)newVal != val) {
                            isDifferent = true;
                            break;
                        }
                    }
                }

                //Setup the UI if this is a mixed value
                using (GUIMixer.PushSegment(isDifferent)) {
                    //Begin looking for changes
                    EditorGUI.BeginChangeCheck();

                    //If this type has a flags attribute, show a masking field
                    float newVal = Mathf.Max(EditorGUI.FloatField(position, label, val), att.Max);

                    //If the value changed, apply it 
                    if (EditorGUI.EndChangeCheck()) {
                        modified = true;
                        for (int i = 0; i < parameterCaches.Length; i++) 
                            parameterCaches[i].SetValue(newVal, Processing);
                    }
                }
            }

            //Process the value as an integer
            else {
                //Cast the value to a integer
                int val = (int)currentObj;

                //Check if the contained values are different
                bool isDifferent = false;
                if (parameterCaches.Length > 1) {
                    for (int i = 1; i < parameterCaches.Length; i++) {
                        //Retrieve this entries value
                        object newVal = parameterCaches[i].Value;

                        //If the values are different, flag it
                        if ((int)newVal != val) {
                            isDifferent = true;
                            break;
                        }
                    }
                }

                //Setup the UI if this is a mixed value
                using (GUIMixer.PushSegment(isDifferent)) {
                    //Begin looking for changes
                    EditorGUI.BeginChangeCheck();

                    //If this type has a flags attribute, show a masking field
                    int newVal = Mathf.Max(EditorGUI.IntField(position, label, val), (int)att.Max);

                    //If the value changed, apply it 
                    if (EditorGUI.EndChangeCheck()) {
                        modified = true;
                        for (int i = 0; i < parameterCaches.Length; i++) 
                            parameterCaches[i].SetValue(newVal, Processing);
                    }
                }
            }

            //Return the modified flag
            return modified;
        }
    }
}