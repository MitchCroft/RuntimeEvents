using UnityEditor;
using UnityEngine;

namespace RuntimeEvents.ParameterProcessors {
    /// <summary>
    /// Provide the functionality required to display range bounded numerical options within the Runtime Event inspector
    /// </summary>
    [CustomParameterDrawer(typeof(NumericRangeAttribute))]
    public sealed class NumericRangeDrawer : AErrorReportingDrawer {
        /*----------Functions----------*/
        //PUBLIC

        /// <summary>
        /// Slider drawing is a simple process, only requires a single line
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
                return DrawErrorMessage(position, label, "NumericRange Attribute is only usable on int/float types");

            //Get the value of the primary cache object
            object currentObj;
            if (!Processor.GetValue(parameterCaches[0], out currentObj)) {
                EditorGUI.LabelField(position, label, new GUIContent("Failed to retrieve current value from processor"));
                return false;
            }

            //Check if the element has changed
            bool modified = false;

            //Get the attribute value 
            NumericRangeAttribute att = Attribute as NumericRangeAttribute;

            //Process the value as a float
            if (currentObj is float) {
                //Cast the value to a float
                float val = (float)currentObj;

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
                    float newVal = EditorGUI.Slider(position, label, val, att.Min, att.Max);

                    //If the value changed, apply it 
                    if (EditorGUI.EndChangeCheck()) {
                        modified = true;
                        for (int i = 0; i < parameterCaches.Length; i++) {
                            if (!Processor.AssignValue(parameterCaches[i], newVal))
                                Debug.LogErrorFormat("Failed to assign the new state value '{0}' to the parameter cache at index {1}", newVal, i);
                        }
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
                        object newVal;
                        if (!Processor.GetValue(parameterCaches[i], out newVal)) {
                            EditorGUI.LabelField(position, label, new GUIContent("Failed to retrieve current value from processor"));
                            return false;
                        }

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
                    int newVal = EditorGUI.IntSlider(position, label, val, (int)att.Min, (int)att.Max);

                    //If the value changed, apply it 
                    if (EditorGUI.EndChangeCheck()) {
                        modified = true;
                        for (int i = 0; i < parameterCaches.Length; i++) {
                            if (!Processor.AssignValue(parameterCaches[i], newVal))
                                Debug.LogErrorFormat("Failed to assign the new state value '{0}' to the parameter cache at index {1}", newVal, i);
                        }
                    }
                }
            }

            //Return the modified flag
            return modified;
        }
    }
}