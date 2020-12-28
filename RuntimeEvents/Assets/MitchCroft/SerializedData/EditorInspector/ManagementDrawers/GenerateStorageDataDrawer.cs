#if UNITY_EDITOR
using System;
using System.IO;

using UnityEditor;
using UnityEngine;

namespace MitchCroft.SerializedData.EditorInspector.Management {
    /// <summary>
    /// Display a button that can be used to generate a basic value type storage object for use
    /// </summary>
    internal sealed class GenerateStorageDataDrawer : IDataDrawer {
        /*----------Variables----------*/
        //CONST

        /// <summary>
        /// Store the template text that will be output to generate a type that can be used to store the required display type
        /// </summary>
        /// <remarks>
        /// Key:
        ///     {0} = File Name
        ///     {1} = Type Full Name
        /// </remarks>
        private const string SERIAL_STORAGE_TEMPLATE = "#if UNITY_EDITOR\nnamespace MitchCroft.SerializedData.EditorInspector.StorageObjects {{\n    /// <summary>\n    /// Store values of the <see cref=\"{1}\"/> type so that they can be displayed with <see cref=\"Data\"/> objects in the inspector\n    /// </summary>\n    [CustomSerialStorage(typeof({1}))]\n    public sealed class {0} : SerialStorage<{1}> {{}}\n}}\n#endif";

        /*----------Properties----------*/
        //PUBLIC

        /// <summary>
        /// The Data Type that is being offered for type construction
        /// </summary>
        public Type DataType { get; set; }

        /// <summary>
        /// Ignored Modifier, irrelevant to type storage
        /// </summary>
        public IGenericDataModifier Modifier { get; set; }

        /*----------Functions----------*/
        //PUBLIC

        /// <summary>
        /// Get the height within the inspector that is required to display this value
        /// </summary>
        /// <param name="property">[Ignored] The Serialized Value property that is to be displayed</param>
        /// <param name="label">[Ignored] The label that has been assigned to the property to be displayed</param>
        /// <returns>Returns the height required to display in pixels</returns>
        public float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return EditorGUIUtility.singleLineHeight;
        }

        /// <summary>
        /// Display the generate prompt within the inspector at the required position
        /// </summary>
        /// <param name="position">The position within the inspector window to display the information</param>
        /// <param name="property">[Ignored] The Serialized Value property that is to be displayed</param>
        /// <param name="label">The label that has been assigned to the property to be displayed</param>
        public void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            // Prefix the label assigned to this value for display
            position = EditorGUI.PrefixLabel(position, label);

            // Offer the button prompt for creating the type
            if (GUI.Button(
                    position,
                    new GUIContent(string.Format("Generate '{0}' Container", DataType.Name), "Generate a basic data storage object definition to handle the current type")
                )) {
                // Find the location within the project to generate the file
                string location = EditorUtility.SaveFilePanelInProject(
                    string.Format("Generate '{0}' Storage Type", DataType.Name),
                    string.Format("{0}SerialStorage", DataType.Name),
                    "cs",
                    "Choose a location to output the generated type storage object"
                );

                // If there was a location, export it
                if (!string.IsNullOrEmpty(location)) {
                    // Compile the final text and export to location
                    File.WriteAllText(
                        location,
                        string.Format(
                            SERIAL_STORAGE_TEMPLATE,
                            Path.GetFileNameWithoutExtension(location),
                            DataType.FullName
                        )
                    );

                    // Refresh the assets that are in the project
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
            }
        }
    }
}
#endif