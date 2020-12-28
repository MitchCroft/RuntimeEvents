#if UNITY_EDITOR
using System;
using System.Linq;

using UnityEngine;
using UnityEditor;

namespace MitchCroft.SerializedData.EditorInspector.DrawerObjects {
    /// <summary>
    /// Handle the displaying of enumeration value options within the inspector as needed
    /// </summary>
    [CustomDataDrawer(typeof(Enum), true)]
    public sealed class EnumValueDrawer : IDataDrawer {
        /*----------Properties----------*/
        //PUBLIC

        /// <summary>
        /// The type of data that is currently being displayed
        /// </summary>
        public Type DataType { get; set; }

        /// <summary>
        /// The modifier that is currently assigned for display
        /// </summary>
        public IGenericDataModifier Modifier { get; set; }

        /*----------Functions----------*/
        //PRIVATE

        /// <summary>
        /// Nicify the collection of supplied names for display purposes
        /// </summary>
        /// <param name="names">The array of names that are to be cleaned up for display</param>
        /// <returns>Returns a reference to the same array</returns>
        private static string[] NicifyStringCollection(string[] names) {
            for (int i = 0; i < names.Length; ++i)
                names[i] = ObjectNames.NicifyVariableName(names[i]);
            return names;
        }

        /// <summary>
        /// Create an array of the enum labels to be displayed
        /// </summary>
        /// <param name="names">The string labels that are to be converted into GUIContent objects for display</param>
        /// <returns></returns>
        private static GUIContent[] CreateEnumLabels(string[] names) {
            GUIContent[] labels = new GUIContent[names.Length];
            for (int i = 0; i < names.Length; ++i)
                labels[i] = new GUIContent(names[i]);
            return labels;
        }

        //PUBLIC

        /// <summary>
        /// Get the height within the inspector that is required to display this value
        /// </summary>
        /// <param name="property">The Serialized Value property that is to be displayed</param>
        /// <param name="label">The label that has been assigned to the property to be displayed</param>
        /// <returns>Returns the height required to display in pixels</returns>
        public float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return EditorGUIUtility.singleLineHeight;
        }

        /// <summary>
        /// Display the specified property within the inspector for modification as needed
        /// </summary>
        /// <param name="position">The position within the inspector window to display the information</param>
        /// <param name="property">The Serialized Value property that is to be displayed</param>
        /// <param name="label">The label that has been assigned to the property to be displayed</param>
        /// <remarks>
        /// Implementation base on https://github.com/jamesjlinden/unity-decompiled/blob/96fb16e2eb6fff1acf3d4e25fa713defb3d17999/UnityEditor/UnityEditor/EditorGUI.cs#L4370
        /// </remarks>
        public void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            // The type must be an enumeration to display
            if (!DataType.IsEnum)
                throw new Exception(string.Format("Can't display property for '{0}' type. Type is not an enumeration", DataType));

            // Using BeginProperty / EndProperty on the parent property means that
            // prefab override logic works on the entire property.
            EditorGUI.BeginProperty(position, label, property);

            // If this type is a flags type, then show a mask field
            if (DataType.GetCustomAttributes(typeof(System.FlagsAttribute), false).Length > 0) {
                property.longValue = EditorGUI.MaskField(
                    position,
                    label,
                    (int)property.longValue,
                    NicifyStringCollection(Enum.GetNames(DataType))
                );
            }

            // Otherwise, use the the standard enum popup
            else {
                // Show the popup to give the user selection choice
                Enum[] array = Enum.GetValues(DataType).Cast<Enum>().ToArray<Enum>();
                string[] names = Enum.GetNames(DataType);
                int selectedIndex = Array.IndexOf<Enum>(array, (Enum)Enum.ToObject(DataType, property.intValue));
                int index = EditorGUI.Popup(
                    position, 
                    label,
                    selectedIndex, 
                    CreateEnumLabels(
                        NicifyStringCollection(names)
                    )
                );

                // If the value has been modified, convert the new value
                if (selectedIndex != index) {
                    property.intValue = (index < 0 || index >= names.Length ?
                        index :
                        Convert.ToInt32(array[index])
                    );
                }
            }

            // End of the property being displayed
            EditorGUI.EndProperty();
        }
    }
}
#endif