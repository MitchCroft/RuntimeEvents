using System;
using System.Reflection;
using System.Collections;

using UnityEditor;

namespace RuntimeEvents {
    /// <summary>
    /// Provide additional functionality for the <see cref="UnityEditor.SerializedProperty"/> object
    /// </summary>
    public static class SerializedPropertyExtensions {
        /*----------Variables----------*/
        //CONST

        /// <summary>
        /// Store the flag types that will be used to search the the serialized property path structure
        /// </summary>
        private const BindingFlags SEARCH_FLAGS = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy | BindingFlags.Instance;

        /// <summary>
        /// Store the characters that will be split on when extracting the value index when path tracing
        /// </summary>
        private static readonly char[] INDEX_SPLIT_CHARS = { '[', ']' };

        /*----------Functions----------*/
        //PUBLIC

        /// <summary>
        /// Retrieve the underlying object value of the supplied SerializedProperty object
        /// </summary>
        /// <param name="property">The property that is having its value retrieved</param>
        /// <returns>Returns the object value that exists at the SerializedProperty location</returns>
        /// <remarks>
        /// Implementation based off of the following with additional tinkering:
        /// https://forum.unity.com/threads/get-a-general-object-value-from-serializedproperty.327098/
        /// https://answers.unity.com/questions/1347203/a-smarter-way-to-get-the-type-of-serializedpropert.html
        /// https://stackoverflow.com/questions/7072088/why-does-type-getelementtype-return-null
        /// https://answers.unity.com/questions/929293/get-field-type-of-serializedproperty.html
        /// </remarks>
        public static object GetPropertyValue(this SerializedProperty property) {
            //Retrieve the base object that the property is based off of
            object obj = property.serializedObject.targetObject;

            //Retrieve the sections of the path that must be traveled by reflection
            string[] pathSections = property.propertyPath.Split('.');

            //Travel down the property path sections
            FieldInfo field;
            for (int p = 0; p < pathSections.Length; p++) {
                //If this section is not an array of data, process it simple
                if (pathSections[p] != "Array") {
                    //Retrieve the type of the current object reference
                    Type type = obj.GetType();

                    //Get the field for the next section
                    field = type.GetField(pathSections[p], SEARCH_FLAGS);

                    //Get the object value at this section
                    obj = field.GetValue(obj);
                }

                //Otherwise, retrieve the array element within the collection
                else {
                    //Retrieve the index element of the data to extract (String format should be 'data[##]')
                    string indexString = pathSections[++p].Split(INDEX_SPLIT_CHARS)[1];

                    //Attempt to parse the index
                    int index;
                    if (!int.TryParse(indexString, out index))
                        throw new OperationCanceledException(string.Format("While attempting to parse SerializedProperty path '{0}' the array index value was unable to be parsed from the string '{1}'. Expected data format was 'data[##]'", property.propertyPath, pathSections[p]));

                    //Ensure that this element is a list
                    if (!(obj is IList)) throw new NullReferenceException(string.Format("While attempting to parse SerializedProperty path '{0}' the collection object '{1}' ({2}) could not be retrieved", property.propertyPath, obj, pathSections[p - 2]));

                    //Get the object value at this section
                    obj = (obj as IList)[index];
                }
            }

            //Return the object at the end of the search
            return obj;
        }

        /// <summary>
        /// Retrieve the underlying object value of the SerializedProperty for all selected objects
        /// </summary>
        /// <param name="property">The property that is having its values retrieved</param>
        /// <returns>Returns an array of object values that exist for the SerializedProperty on all selected objects</returns>
        public static object[] GetPropertyValues(this SerializedProperty property) {
            //Retrieve the base object that the property is based off of
            object[] objs = property.serializedObject.targetObjects;

            //Check there are objects to modify
            if (objs.Length > 0) {
                //Retrieve the sections of the path that must be traveled by reflection
                string[] pathSections = property.propertyPath.Split('.');

                //Travel down the property path sections
                FieldInfo field;
                for (int p = 0; p < pathSections.Length; p++) {
                    //If this is not an array of data, process it simple
                    if (pathSections[p] != "Array") {
                        //Retrieve the type of the current object
                        Type type = objs[0].GetType();

                        //Get the field for the next section
                        field = type.GetField(pathSections[p], SEARCH_FLAGS);

                        //Get the object values at the this section
                        object[] copy = new object[objs.Length];
                        for (int i = 0; i < objs.Length; i++) {
                            if (objs[i] != null)
                                copy[i] = field.GetValue(objs[i]);
                        }
                        objs = copy;
                    }

                    //Otherwise, retrieve the array element within the collection
                    else {
                        //Retrieve the index element of the data to extract (String format should be 'data[##]')
                        string indexString = pathSections[++p].Split(INDEX_SPLIT_CHARS)[1];

                        //Attempt to parse the index
                        int index;
                        if (!int.TryParse(indexString, out index))
                            throw new OperationCanceledException(string.Format("While attempting to parse SerializedProperty path '{0}' the array index value was unable to be parsed from the string '{1}'. Expected data format was 'data[##]'", property.propertyPath, pathSections[p]));

                        //Ensure that this element is a list
                        if (!(objs[0] is IList)) throw new NullReferenceException(string.Format("While attempting to parse SerializedProperty path '{0}' the collection object '{1}' ({2}) could not be retrieved", property.propertyPath, objs[0], pathSections[p - 2]));

                        //Get the object values at this section
                        for (int i = 0; i < objs.Length; i++) {
                            if (objs[i] != null)
                                objs[i] = (objs[i] as IList)[index];
                        }
                    }
                }
            }

            //Return the final object values
            return objs;
        }

        /// <summary>
        /// Retrieve the underlying object value of the supplied SerializedProperty object cast to the expected type
        /// </summary>
        /// <typeparam name="T">The type that the evaluated value should be returned as</typeparam>
        /// <param name="property">The property that is having its value retrieved</param>
        /// <param name="value">The variable that will be filled with the retrieved value if it is of T</param>
        /// <returns>Returns true if the retrieved type was of the specified type, otherwise false</returns>
        public static bool GetPropertyValue<T>(this SerializedProperty property, out T value) {
            //Set the default value
            value = default(T);

            //Get the regular value from the property
            object val = property.GetPropertyValue();

            //If the value isn't of T, failed
            if (!(val is T)) return false;

            //Cast the value back
            value = (T)val;
            return true;
        }

        /// <summary>
        /// Retrieve the underlying object values of the SerializedProperty for all target objects, cast to the expected type
        /// </summary>
        /// <typeparam name="T">The type that the evaluated value should be returned as</typeparam>
        /// <param name="property">The property that is having its values retrieved</param>
        /// <param name="value">The array that will be filled with the retrieved value data if it is of T</param>
        /// <returns>Returns true if the retrieved type was of the specified type, otherwise false</returns>
        public static bool GetPropertyValues<T>(this SerializedProperty property, out T[] value) {
            //Get the regular values from the property
            object[] vals = property.GetPropertyValues();

            //Create the return array to match size
            value = new T[vals.Length];

            //Loop through and assign the cast values
            for (int i = 0; i < vals.Length; i++) {
                //Check that the value will work
                if (vals[i] == null || vals[i] is T)
                    value[i] = (T)vals[i];

                //Otherwise, type is a mismatch
                else {
                    value = null;
                    return false;
                }
            }

            //If gotten this far, success
            return true;
        }
    }
}