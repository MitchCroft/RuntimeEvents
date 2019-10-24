using UnityEditor;
using UnityEngine;

namespace RuntimeEvents.ParameterProcessors {
    /// <summary>
    /// Provide the functionality required to display string options in an area within the Runtime Event inspector
    /// </summary>
    [CustomParameterDrawer(typeof(TextFieldAreaAttribute))]
    public sealed class TextFieldAreaDrawer : AErrorReportingDrawer {
        /*----------Functions----------*/
        //PUBLIC

        /// <summary>
        /// String drawing is a simple process, only requires a single line
        /// </summary>
        /// <param name="parameterCaches">The parameter caches for the currently selected objects that store the current values of this parameter</param>
        /// <param name="label">The label that is attached to this parameter to be displayed</param>
        /// <returns>Returns a single line height for this value type</returns>
        public override float GetDrawerHeight(PersistentParameterCache[] parameterCaches, GUIContent label) {
            //Check that the type is a string
            if (Processing == typeof(string)) {
                //Retrieve the attribute to get line count
                TextFieldAreaAttribute att = Attribute as TextFieldAreaAttribute;

                //Use the height specified in the attribute
                return att.Height + EditorGUIUtility.singleLineHeight;
            }

            //Otherwise, error 
            return GetErrorHeight();
        }

        /// <summary>
        /// Display the current string value setting to the inspector
        /// </summary>
        /// <param name="position">The position within the inspector that the UI elements should be displayed</param>
        /// <param name="parameterCaches">The parameter caches for the currently selected objects that store the current values of this parameter</param>
        /// <param name="label">The label that is attached to this parameter to be displayed</param>
        /// <returns>Returns true if an event occurred that requires the updating of cached values</returns>
        public override bool DisplayParameterValue(Rect position, PersistentParameterCache[] parameterCaches, GUIContent label) {
            //Check that this type is a string
            if (Processing != typeof(string))
                return DrawErrorMessage(position, label, "TextFieldArea Attribute is only usable on string types");

            //Cast the value to a string
            string val = (string)parameterCaches[0].Value;

            //Check if the contained values are different
            bool isDifferent = false;
            if (parameterCaches.Length > 1) {
                for (int i = 1; i < parameterCaches.Length; i++) {
                    //Retrieve this entries value
                    object newVal = parameterCaches[i].Value;

                    //If the values are different, flag it
                    if ((string)newVal != val) {
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

                //If this type has a flags attribute, show a masking field
                EditorGUI.LabelField(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), label);
                string newVal = EditorGUI.TextArea(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.width, position.height - EditorGUIUtility.singleLineHeight), val);

                //If the value changed, apply it 
                if (EditorGUI.EndChangeCheck()) {
                    modified = true;
                    for (int i = 0; i < parameterCaches.Length; i++) 
                        parameterCaches[i].SetValue(newVal, Processing);
                }
            }

            //Return the modified flag
            return modified;
        }
    }
}