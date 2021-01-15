#if UNITY_EDITOR
using System;
using System.Collections.Generic;

using MitchCroft.Helpers;
using MitchCroft.Utility;
using MitchCroft.SerializedData.EditorInspector.Management;

using UnityEngine;
using UnityEditor;

namespace MitchCroft.SerializedData.EditorInspector {
    /// <summary>
    /// Handle the displaying of generic serialised data within the Unity Inspector window
    /// </summary>
    [CustomPropertyDrawer(typeof(SerialData))]
    public sealed class SerialDataPropertyDrawer : PropertyDrawer {
        /*----------Types----------*/
        //PRIVATE

        /// <summary>
        /// Provide a custom comparer object to be used to differentiate attributes based on their values instead of instances
        /// </summary>
        private sealed class TypeMarkerAttributeComparer : EqualityComparer<TypeMarkerAttribute> {
            /*----------Functions----------*/
            //PUBLIC

            /// <summary>
            /// Determine if two <see cref="TypeMarkerAttributeComparer"/> instances are equivelant
            /// </summary>
            /// <param name="x">The first instance of the attribute to be tested</param>
            /// <param name="y">The second instance of the attribute to be tested</param>
            /// <returns>Returns true if both instances are equivelant</returns>
            public override bool Equals(TypeMarkerAttribute x, TypeMarkerAttribute y) {
                // Check possible null conditions
                if (x == null && y == null) return true;
                if (x == null || y == null) return false;

                // Compare the hash codes
                return (x.GetHashCode() == y.GetHashCode());
            }

            /// <summary>
            /// Retrieve the hash code of a specified <see cref="TypeMarkerAttributeComparer"/> object instance
            /// </summary>
            /// <param name="obj">The object whose hash code is to be retrieved</param>
            /// <returns>Returns the hash code of the object instance</returns>
            public override int GetHashCode(TypeMarkerAttribute obj) {
                return (object.Equals(obj, null) ?
                    0 :
                    obj.GetHashCode()
                );
            }
        }

        /*----------Variables----------*/
        //CONST

        /// <summary>
        /// Store the basic management types that will be used to display required management information
        /// </summary>
        private static readonly IDataDrawer mismatchDrawer,
                                            nonSerialDrawer,
                                            generateDrawer,
                                            missingSerialDrawer,
                                            defaultDrawer;

        //SHARED

        /// <summary>
        /// The different drawer object definitions that are located within the project for use
        /// </summary>
        private static Dictionary<TypeMarkerAttribute, Type> dataDrawerTypes;

        /// <summary>
        /// The different data storage object definitions that are located within the project for use
        /// </summary>
        private static Dictionary<TypeMarkerAttribute, Type> dataStorageTypes;

        //PRIVATE

        /// <summary>
        /// Cache the data drawer instances that are created to display the required information
        /// </summary>
        private Dictionary<Type, IDataDrawer> dataDrawerInstances;

        /// <summary>
        /// Cache the data storage instances that are needed to display the required information
        /// </summary>
        private Dictionary<Type, List<SerialStorage>> dataStorageInstances;

        /// <summary>
        /// Cache the drawer that is used for each type that is looked up
        /// </summary>
        private Dictionary<Type, IDataDrawer> drawerSearchResults;

        /// <summary>
        /// Cache the storage type that is used for each type that is looked up
        /// </summary>
        private Dictionary<Type, Type> storageSearchResults;

        /*----------Functions----------*/
        //PRIVATE

        /// <summary>
        /// Identify the object elements within the currently loaded assembly for display
        /// </summary>
        static SerialDataPropertyDrawer() {
            // Find all of the drawer types that can be used
            Type drawerAttributeType = typeof(CustomDataDrawerAttribute);
            TypeMarkerAttributeComparer comparer = new TypeMarkerAttributeComparer();
            dataDrawerTypes = new Dictionary<TypeMarkerAttribute, Type>(comparer);
            foreach (Type type in AssemblyTypeScanner.GetTypesWithinAssembly<IDataDrawer>()) {
                // Look for the drawer marker attribute for use
                object[] atts = type.GetCustomAttributes(drawerAttributeType, false);
                if (atts == null || atts.Length == 0)
                    continue;

                // Get the attribute that will be used to identify the type it will be used for
                CustomDataDrawerAttribute att = null;
                for (int i = 0; i < atts.Length; ++i) {
                    att = atts[i] as CustomDataDrawerAttribute;
                    if (att != null) break;
                }
                if (att == null || att.AssociatedType == null) continue;

                // Check that this type can be created for use
                if (type.IsAbstract) {
                    Debug.LogErrorFormat("Unable to use Drawer type '{0}' for the type '{1}' as the drawer is abstract", type, att.AssociatedType);
                    continue;
                } else if (type.GetConstructor(Type.EmptyTypes) == null) {
                    Debug.LogErrorFormat("Unable to use Drawer type '{0}' for the type '{1}' as the drawer has no default constructor", type, att.AssociatedType);
                    continue;
                }

                // If there is an existing drawer for this type, warn of override
                if (dataDrawerTypes.ContainsKey(att))
                    Debug.LogWarningFormat("Drawer Type '{0}' is overriding '{1}' for use to draw '{2}'", type, dataDrawerTypes[att], att);
                dataDrawerTypes[att] = type;
            }

            // Find all of the serial storage types that can be used
            Type serialAttributeType = typeof(CustomSerialStorageAttribute);
            dataStorageTypes = new Dictionary<TypeMarkerAttribute, Type>(comparer);
            foreach (Type type in AssemblyTypeScanner.GetTypesWithinAssembly<SerialStorage>()) {
                // Look for the storage attribute for use
                object[] atts = type.GetCustomAttributes(serialAttributeType, false);
                if (atts == null || atts.Length == 0)
                    continue;

                // Get the attribute that will be used to identify the type it will be used for
                CustomSerialStorageAttribute att = null;
                for (int i = 0; i < atts.Length; ++i) {
                    att = atts[i] as CustomSerialStorageAttribute;
                    if (att != null) break;
                }
                if (att == null || att.AssociatedType == null) continue;

                // If there is an existing storage type for the value type, warn of override
                if (dataStorageTypes.ContainsKey(att))
                    Debug.LogWarningFormat("Storage Type '{0}' is overriding '{1}' for storage of type '{2}'", type, dataStorageTypes[att], att.AssociatedType);
                dataStorageTypes[att] = type;
            }

            // Create the management drawer objects
            mismatchDrawer = new TypeMismatchDataDrawer();
            nonSerialDrawer = new NonSerialDataDrawer();
            generateDrawer = new GenerateStorageDataDrawer();
            missingSerialDrawer = new MissingSerialPropertyDrawer();
            defaultDrawer = new DefaultDataDrawer();
        }

        /// <summary>
        /// Setup the required elements needed to display the current data elements within the inspector
        /// </summary>
        /// <param name="serialDataProperty">The property that defines the location of the data to retrieve</param>
        /// <param name="data">Passes out the data actual objects that are being shown within the inspector</param>
        /// <param name="valueProperty">Passes out the serial value property that is being displayed within the inspector</param>
        /// <param name="storage">Passes out the storage objects that will hold the data values that are being shown</param>
        /// <param name="dataDrawer">Passes out the Drawer instance that will be used to display the required information</param>
        private void SetupDataForDisplay(SerializedProperty serialDataProperty, out SerialData[] data, out SerializedProperty valueProperty, out SerialStorage[] storage, out IDataDrawer dataDrawer) {
            // Retrieve the data objects that are to be shown
            data = GetPropertyValues(serialDataProperty);

            // If the data can be displayed, find a useful data drawer
            if (CanDataBeDisplayed(data, out valueProperty, out storage, out dataDrawer))
                dataDrawer = GetDrawerForData(data[0]);

            // If there is a data object, apply it's settings to the drawer
            if (data[0] != null) {
                dataDrawer.DataType = data[0].DataType;
                dataDrawer.Modifier = data[0].Modifier;
            }
        }

        /// <summary>
        /// Determine if the specified data objects can be displayed using a drawer
        /// </summary>
        /// <param name="data">The data objects that are to be shown within the inspector</param>
        /// <param name="valueProperty">The serialised value property that is to be displayed</param>
        /// <param name="storage">The storage elements that are being used to hold the display values</param>
        /// <param name="invalidDrawer">A Drawer that will be used in the case that will be used in the case the data can't be shown</param>
        /// <returns>Returns true if the data can be shown to inspector window</returns>
        private bool CanDataBeDisplayed(SerialData[] data, out SerializedProperty valueProperty, out SerialStorage[] storage, out IDataDrawer invalidDrawer) {
            // Set the default out values
            valueProperty = null;
            storage = null;
            invalidDrawer = null;

            // Check that all of the data objects are valid for display
            SerialData baseLine = null;
            if (data != null) {
                for (int i = 0; i < data.Length; ++i) {
                    // If this value is null then we have a problem
                    if (data == null) {
                        baseLine = null;
                        break;
                    }

                    // If there is no baseline instance, use this
                    else if (baseLine == null) baseLine = data[i];

                    // If there is a type mismatch then we have a problem
                    else if (baseLine.DataType != data[i].DataType ||
                             !object.Equals(baseLine.Modifier, data[i].Modifier)) {
                        baseLine = null;
                        break;
                    }
                }
            }

            // If there is no baseline then there is a type mismatch that can't be resolved
            if (baseLine == null) {
                invalidDrawer = mismatchDrawer;
                return false;
            }

            // If the data isn't serialisable then can't display
            if (!baseLine.IsValid) {
                invalidDrawer = nonSerialDrawer;
                return false;
            }

            // Get the collection of storage objects that will be used for display
            storage = GetStorageForData(baseLine.DataType, data.Length);
            if (storage == null) {
                invalidDrawer = generateDrawer;
                return false;
            }

            // Assign the data to the storage objects for display
            for (int i = 0; i < data.Length; ++i) {
                // Ensure the live value reflects the serial data properly
                data[i].OnAfterDeserialize();

                // Assign the specified value to the storage element
                if (!storage[i].SetValue(data[i].Value)) {
                    Debug.LogErrorFormat("Unable to assign the value '{0}' to the storage object '{1}'. Check storage asset is compatible with this type", data[i].Value, storage[i]);
                    storage = null;
                    invalidDrawer = generateDrawer;
                    return false;
                }
            }

            // Try to serialise the storage objects for display
            SerializedObject serialStorage = new SerializedObject(storage);
            valueProperty = serialStorage.FindProperty(storage[0].GetValuePropertyName());
            if (valueProperty == null) {
                storage = null;
                invalidDrawer = missingSerialDrawer;
                return false;
            }

            // If got this far, valid
            return true;
        }

        /// <summary>
        /// Retrieve the storage instances that will be used for the specified type
        /// </summary>
        /// <param name="type">The type of value that needs to be stored in the resulting storage objects</param>
        /// <param name="count">The number of storage objects that are needed for the current display</param>
        /// <returns>Returns an array of the required storage objects or null if unable</returns>
        private SerialStorage[] GetStorageForData(Type type, int count) {
            // If there is not already a search result, go looking
            if (!storageSearchResults.ContainsKey(type)) {
                // Look for a storage type that can support the specified type
                Type storageType = CheckTypeChainForEntry(new CustomSerialStorageAttribute(type), dataStorageTypes);

                // If there is a type defined, need to make sure the base elements are setup
                if (storageType != null) {
                    // Ensure that there is a list of instances setup ready for display
                    if (!dataStorageInstances.ContainsKey(storageType))
                        dataStorageInstances[storageType] = new List<SerialStorage>();
                    storageSearchResults[type] = storageType;
                }

                // Otherwise, this isn't going to be able to be displayed
                else {
                    Debug.LogErrorFormat("Unable to find a SerialStorage type to contain '{0}', can't display value", type);
                    storageSearchResults[type] = null;
                }
            }

            // If there is no type listed, then we can't serialise this data
            if (storageSearchResults[type] == null)
                return null;

            // Ensure that there are enough instances of the storage type for use
            Type instanceType = storageSearchResults[type];
            while (dataStorageInstances[instanceType].Count < count) {
                // Create a new instance of the type for inclusion in the cache
                SerialStorage buffer = ScriptableObject.CreateInstance(instanceType) as SerialStorage;
                if (buffer != null) dataStorageInstances[instanceType].Add(buffer);
                else {
                    Debug.LogErrorFormat("Data storage type '{0}' for value type '{1}' can't be created for use. Check type definition is correct", instanceType, type);
                    return null;
                }
            }

            // Get the collection of objects that will be used to display
            SerialStorage[] storage = new SerialStorage[count];
            dataStorageInstances[instanceType].CopyTo(0, storage, 0, count);
            return storage;
        }

        /// <summary>
        /// Retrieve the Data Drawer object that will be used to display the supplied data
        /// </summary>
        /// <param name="data">The SerialData object that contains the type information that needs to be displayed</param>
        /// <returns>Returns a data drawer that will be used to show the information</returns>
        private IDataDrawer GetDrawerForData(SerialData data) {
            // Determine if the there is a modifier drawer that is needed for this type
            if (data.Modifier != null) {
                // Check if there is a data drawer that can be used for the 
                IDataDrawer modDrawer = GetDrawerForType(data.Modifier.GetType());
                if (modDrawer != null)
                    return modDrawer;
            }

            // Check if there is a drawer setup for the data type itself
            IDataDrawer valDrawer = GetDrawerForType(data.DataType);
            if (valDrawer != null)
                return valDrawer;

            // If got this far, we're displaying with the default drawer
            return defaultDrawer;
        }

        /// <summary>
        /// Get the <see cref="IDataDrawer"/> instance required to display the specified type
        /// </summary>
        /// <param name="displayType">The type of data (Modifier or value actual) that needs to be displayed</param>
        /// <returns>Returns a IDataDrawer instance that can be used to display the type or null if none</returns>
        private IDataDrawer GetDrawerForType(Type displayType) {
            // Check if there is a search result cached
            if (!drawerSearchResults.ContainsKey(displayType)) {
                // Try to find a drawer for the specified type
                Type displayDrawerType = CheckTypeChainForEntry(new CustomDataDrawerAttribute(displayType), dataDrawerTypes);

                // If there is a drawer type, we need an instance 
                if (displayDrawerType != null) {
                    // Check if there is already an instance for this type
                    if (!dataDrawerInstances.ContainsKey(displayDrawerType))
                        dataDrawerInstances[displayDrawerType] = InstantiateDataDrawerType(displayDrawerType);

                    // Save the instantiated instance (if there was one) for this type
                    drawerSearchResults[displayType] = dataDrawerInstances[displayDrawerType];
                }

                // Otherwise, just use null so the search isn't done multiple times
                else drawerSearchResults[displayType] = null;
            }

            // Return the cached searched results for this type
            return drawerSearchResults[displayType];
        }

        /// <summary>
        /// Instantiate an instance of the specified <see cref="IDataDrawer"/> type 
        /// </summary>
        /// <param name="drawerType">The type of IDataDrawer to be instantiated for use</param>
        /// <returns>Returns a Data Drawer instance or null if unable to create</returns>
        private IDataDrawer InstantiateDataDrawerType(Type drawerType) {
            try {
                return (IDataDrawer)Activator.CreateInstance(drawerType);
            } catch (Exception exec) {
                Debug.LogErrorFormat("Unable to create a IDataDrawer instance of type '{0}'. ERROR: {1}", drawerType, exec);
                return null;
            }
        }

        /// <summary>
        /// Retrieve the actual data objects that are referenced
        /// </summary>
        /// <param name="serialDataProperty">The property that defines the location of the data to retrieve</param>
        /// <returns>Returns an array of the Data objects that were found or null if unable to find them</returns>
        private static SerialData[] GetPropertyValues(SerializedProperty serialDataProperty) {
            SerialData[] collection;
            EditorHelper.GetPropertyValues(serialDataProperty, out collection);
            return collection;
        }

        /// <summary>
        /// Searched the cached lookup elements for a type that can be used for the specified marker
        /// </summary>
        /// <param name="marker">The marker that is used to identify the type (if any) that should be used</param>
        /// <param name="lookup">The lookup dictionary that will be checked for a matching type</param>
        /// <returns>Returns the type that supports the marker of null if not found</returns>
        private static Type CheckTypeChainForEntry(TypeMarkerAttribute marker, Dictionary<TypeMarkerAttribute, Type> lookup) {
            // Start by searching for a specific type that can handle this type
            marker.UseForChildren = false;
            if (lookup.ContainsKey(marker))
                return lookup[marker];

            // Otherwise, traverse up the hierarchy chain for a child supporting type that will work
            marker.UseForChildren = true;
            do {
                // Check for an entry at this level
                if (lookup.ContainsKey(marker))
                    return lookup[marker];

                // Work the way up the chain
                marker.AssociatedType = marker.AssociatedType.BaseType;
            } while (marker.AssociatedType != null);

            // If got this far, no dedicated drawer object
            return null;
        }

        //PUBLIC

        /// <summary>
        /// Initialise the default values of this drawer for operation
        /// </summary>
        public SerialDataPropertyDrawer() {
            dataDrawerInstances = new Dictionary<Type, IDataDrawer>(dataDrawerTypes.Count);
            dataStorageInstances = new Dictionary<Type, List<SerialStorage>>(dataStorageTypes.Count);
            drawerSearchResults = new Dictionary<Type, IDataDrawer>();
            storageSearchResults = new Dictionary<Type, Type>();
        }

        /// <summary>
        /// Setup the required objects needed to calculate the height required for the specified data
        /// </summary>
        /// <param name="property">The <see cref="SerialData"/> property that contains the information to be displayed</param>
        /// <param name="label">The label that has been assigned to the property for display</param>
        /// <returns>Returns the height required to display the property information</returns>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            SerialData[] data;
            SerializedProperty valueProperty;
            SerialStorage[] storage;
            IDataDrawer dataDrawer;
            SetupDataForDisplay(property, out data, out valueProperty, out storage, out dataDrawer);
            return dataDrawer.GetPropertyHeight(valueProperty, label);
        }

        /// <summary>
        /// Setup the required objects needed to display the values within the current data 
        /// </summary>
        /// <param name="position">The position within the inspector where the value is to be displayed</param>
        /// <param name="property">The property that is to be displayed to within the inspector</param>
        /// <param name="label">The label that assigned to the property to be displayed</param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            // Using BeginProperty / EndProperty on the parent property means that
            // prefab override logic works on the entire property.
            EditorGUI.BeginProperty(position, label, property);

            // Get the base elements needed to display any data
            SerialData[] data;
            SerializedProperty valueProperty;
            SerialStorage[] storage;
            IDataDrawer dataDrawer;
            SetupDataForDisplay(property, out data, out valueProperty, out storage, out dataDrawer);

            // Begin checking for changes to the UI elements for data updates
            EditorGUI.BeginChangeCheck();

            // Use the drawer to show the value information
            dataDrawer.OnGUI(position, valueProperty, label);

            // If anything changed then the data needs to be updated
            if (EditorGUI.EndChangeCheck() && valueProperty != null) {
                // Apply the modified properties to the storage elements that are about to have their data modified
                valueProperty.serializedObject.ApplyModifiedPropertiesWithoutUndo();

                // Record the changes to the target objects that are about to have their data modified
                Undo.RecordObjects(property.serializedObject.targetObjects, "SerialData Modified");

                // Apply the new data to the specified objects
                object toApply = storage[0].GetValue();
                for (int i = 0; i < data.Length; ++i) {
                    // If the data can't be assigned, warn the user and hope they can figure it out
                    if (!data[i].SetValue(toApply, data[i].DataType))
                        Debug.LogErrorFormat("Failed to apply the data value '{0}' to the object of type '{1}'", toApply, data[i].DataType);
                    
                    else {
                        // Apply the data changes to the serial storage for record
                        data[i].OnBeforeSerialize();

                        // Record any prefab changes that are needed for updates
                        PrefabUtility.RecordPrefabInstancePropertyModifications(property.serializedObject.targetObjects[i]);
                    }
                }
            }

            // End of the property being displayed
            EditorGUI.EndProperty();
        }
    }
}
#endif