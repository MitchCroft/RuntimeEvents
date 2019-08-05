using System;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

namespace RuntimeEvents.ParameterProcessors {
    /// <summary>
    /// Provide the functionality required to display generic enum options within the Runtime Event inspector
    /// </summary>
    [CustomParameterDrawer(typeof(Enum))]
    public sealed class GenericEnumDrawer : AParameterDrawer {
        /*----------Variables----------*/
        //PRIVATE

        /// <summary>
        /// Store an array of labels that can be used to represent the different available options
        /// </summary>
        private static readonly GUIContent[] GENERIC_OPTION_LABELS;

        /// <summary>
        /// Store a map that can be used to convert an enum value to an index in the options list
        /// </summary>
        private static readonly Dictionary<Enum, int> ENUM_TO_INDEX;

        /// <summary>
        /// Store a map that can be used to convert an index value to an enum value
        /// </summary>
        private static readonly Dictionary<int, Enum> INDEX_TO_ENUM;

        /*----------Functions----------*/
        //PRIVATE

        /// <summary>
        /// Setup the initial mapping elements for the identified generic enums
        /// </summary>
        static GenericEnumDrawer() {
            //Find all enum types with the generic option attribute
            List<Type> genericEnums = new List<Type>();
            foreach (Type type in AssemblyTypeScanner.GetTypesWithinAssembly<Enum>()) {
                //Look for the generic option attribute
                if (type.IsEnum && type.GetFirstCustomAttributeOf<GenericEnumOptionAttribute>() != null)
                    genericEnums.Add(type);
            }

            //Sort the stored types based on their names
            genericEnums.Sort((left, right) => left.Name.CompareTo(right.Name));

            //Create the mapping dictionaries
            ENUM_TO_INDEX = new Dictionary<Enum, int>();
            INDEX_TO_ENUM = new Dictionary<int, Enum>();

            //Store a list of the labels that are pulled from the identified elements
            int prog = 0;
            List<GUIContent> labels = new List<GUIContent>();
            foreach (Type generic in genericEnums) {
                //Retrieve the names from the list
                string[] enumNames = Enum.GetNames(generic);

                //Boost the list capacity for the new entries
                labels.Capacity += enumNames.Length;

                //Retrieve the values for this enumeration
                Array values = Enum.GetValues(generic);

                //Add the options to the list
                for (int i = 0; i < enumNames.Length; i++) {
                    //Add this label entry to the list
                    labels.Add(new GUIContent(string.Format("{0}/{1}", generic.Name, enumNames[i])));

                    //Add the mapping options to the 
                    Enum val = (Enum)values.GetValue(i);

                    //Add the mapping options
                    ENUM_TO_INDEX[val] = prog;
                    INDEX_TO_ENUM[prog] = val;

                    //Increment the progress
                    ++prog;
                }
            }

            //Store the final label options list
            GENERIC_OPTION_LABELS = labels.ToArray();
        }

        //PUBLIC

        /// <summary>
        /// Generic display option needs to be displayed with an information box describing how to get enums to show up
        /// </summary>
        /// <param name="parameterCaches">The parameter caches for the currently selected objects that store the current values of this parameter</param>
        /// <param name="label">The label that is attached to this parameter to be displayed</param>
        /// <returns>Returns a multi line height for this value type</returns>
        public override float GetDrawerHeight(PersistentParameterCache[] parameterCaches, GUIContent label) {
            return EditorGUIUtility.singleLineHeight * 3.5f;
        }

        /// <summary>
        /// Display the current enum value setting to the inspector
        /// </summary>
        /// <param name="position">The position within the inspector that the UI elements should be displayed</param>
        /// <param name="parameterCaches">The parameter caches for the currently selected objects that store the current values of this parameter</param>
        /// <param name="label">The label that is attached to this parameter to be displayed</param>
        /// <returns>Returns true if an event occurred that requires the updating of cached values</returns>
        public override bool DisplayParameterValue(Rect position, PersistentParameterCache[] parameterCaches, GUIContent label) {
            //Get the value of the primary cache object
            object currentObj;
            Processor.GetValue(parameterCaches[0], out currentObj);

            //Cast the value to an enum
            Enum enumVal = (Enum)currentObj;

            //Check if the contained values are different
            bool isDifferent = false;
            if (parameterCaches.Length > 1) {
                for (int i = 1; i < parameterCaches.Length; i++) {
                    //Retrieve this entries value
                    object newObj;
                    Processor.GetValue(parameterCaches[i], out newObj);

                    //Get the value as an enum
                    Enum newVal = (Enum)newObj;

                    //If the values are different, flag it
                    if ((enumVal == null && newVal != null) ||
                        (enumVal != null && newVal == null) ||
                        (enumVal.GetType() != newVal.GetType()) ||
                        (Convert.ToInt32(enumVal) != Convert.ToInt32(newVal))) {
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

                //Convert the current option to an index
                int curIndex = (enumVal != null && ENUM_TO_INDEX.ContainsKey(enumVal) ? ENUM_TO_INDEX[enumVal] : -1);

                //Show the text options for the available text
                int newIndex = EditorGUI.Popup(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), label, curIndex, GENERIC_OPTION_LABELS);

                //If the value changed, apply it 
                if (EditorGUI.EndChangeCheck()) {
                    //Convert the new index to a usable value
                    Enum convertVal = (INDEX_TO_ENUM.ContainsKey(newIndex) ? INDEX_TO_ENUM[newIndex] : null);

                    //Apply the new value to the options
                    modified = true;
                    for (int i = 0; i < parameterCaches.Length; i++) {
                        if (!Processor.AssignValue(parameterCaches[i], convertVal))
                            Debug.LogErrorFormat("Failed to assign the new state value '{0}' to the parameter cache at index {1}", convertVal, i);
                    }
                }
            }

            //Display the help box describing how to add options
            EditorGUI.HelpBox(
                new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * 1.25f, position.width, EditorGUIUtility.singleLineHeight * 2f),
                "To add enum options to this list, attach a [GenericEnumOption] attribute to the enum type definition",
                MessageType.Info
            );

            //Return the modified flag
            return modified;
        }
    }
}