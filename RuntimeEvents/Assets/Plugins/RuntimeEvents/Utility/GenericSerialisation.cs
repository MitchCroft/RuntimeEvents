using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace RuntimeEvents {
    /// <summary>
    /// Manage the serialisation of any number of serialisable objects that are supplied
    /// </summary>
    public static class GenericSerialisation {
        /*----------Types----------*/
        //PRIVATE
        
        /// <summary>
        /// Store the representative information that can be used to serialise and read back in object information
        /// </summary>
        [Serializable] private struct SerialCollectionContainer {
            /// <summary>
            /// Store string representation information for each entry within the serialised object
            /// </summary>
            /// <remarks>
            /// If IList objects are serialised they require an entry per object
            /// </remarks>
            public string[] vals;
        }

        /// <summary>
        /// Wrap a reference to an object for serialisation and parsing processing
        /// </summary>
        [Serializable] private struct SerialUnityObjectContainer {
            /// <summary>
            /// Store a reference to the object that is serialised
            /// </summary>
            public UnityEngine.Object obj;
        }

        /*----------Variables----------*/
        //CONST

        /// <summary>
        /// Store the known types that are required for the process
        /// </summary>
        private static readonly Type SBYTE      = typeof(sbyte),
                                     SHORT      = typeof(short),
                                     INT        = typeof(int),
                                     LONG       = typeof(long),
                                     BYTE       = typeof(byte),
                                     USHORT     = typeof(ushort),
                                     UINT       = typeof(uint),
                                     ULONG      = typeof(ulong),
                                     FLOAT      = typeof(float),
                                     DOUBLE     = typeof(double),
                                     DECIMAL    = typeof(decimal),
                                     BOOL       = typeof(bool),
                                     STRING     = typeof(string),
                                     CHAR       = typeof(char),
                                     OBJ        = typeof(object),
                                     UOBJ       = typeof(UnityEngine.Object),
                                     ENUM       = typeof(Enum),
                                     LIST       = typeof(List<>),
                                     ILIST      = typeof(IList);

        /// <summary>
        /// Store a quick lookup for the basic types that can be tested easily
        /// </summary>
        private static readonly HashSet<Type> BASIC_TYPE_LOOKUP = new HashSet<Type>(new Type[] {
            SBYTE, SHORT, INT, LONG, BYTE, USHORT, UINT, ULONG, FLOAT, DOUBLE, DECIMAL, BOOL, STRING
        });

        /*----------Functions----------*/
        //PRIVATE

        /// <summary>
        /// Check to see if the supplied type can be processed
        /// </summary>
        /// <param name="type">The type of object that is being tested</param>
        /// <returns>Returns true if the type can be processed</returns>
        private static bool CanProcessTypeValue(Type type) {
            //If the type is stored in the basic types lookup, its good
            if (BASIC_TYPE_LOOKUP.Contains(type)) return true;

            //Check the other basic types
            if (type == CHAR) return true;
            if (UOBJ.IsAssignableFrom(type)) return true;
            if (ENUM.IsAssignableFrom(type)) return true;

            //Can't process a generic object only
            if (type == OBJ) return false;

            //Otherwise check it is a value or has a default constructor
            return (type.IsValueType || type.HasDefaultConstructor());
        }

        /// <summary>
        /// Retrieve the serialised value for a single object
        /// </summary>
        /// <param name="obj">The object that is to be serialsied</param>
        /// <param name="type">The type of object that is being serialised</param>
        /// <returns>Returns the value as a serialised string</returns>
        private static string SerialiseValue(object obj, Type type) {
            //If the object is a basic type, just to string it
            if (BASIC_TYPE_LOOKUP.Contains(type))
                return (obj != null ? obj.ToString() : string.Empty);

            //If this is a char, use the integral value
            if (type == CHAR) return ((short)(char)obj).ToString();

            //If a Unity object, serialise an object container
            if (UOBJ.IsAssignableFrom(type))
                return JsonUtility.ToJson(new SerialUnityObjectContainer { obj = obj as UnityEngine.Object });

            //If the object is an enum
            if (ENUM.IsAssignableFrom(type)) {
                return JsonUtility.ToJson(new SerialCollectionContainer {
                    vals = new string[] {
                        MinifyTypeAssemblyName(type),
                        Convert.ToInt64(obj).ToString()
                    }
                });
            }

            //At this point, use JsonUtility to manage other types
            return JsonUtility.ToJson(obj);
        }

        /// <summary>
        /// Parse the supplied text to an object of the specified type
        /// </summary>
        /// <param name="serial">The serialised data that is to be parsed to re-create the object definition</param>
        /// <param name="type">The type of the object that is to be returned by this operation</param>
        /// <returns>Returns a generic object of the specified type</returns>
        private static object ParseValue(string serial, Type type) {
            //Check if the object is one of the default types
            if (type == SBYTE)      { sbyte val;    sbyte.TryParse(serial, out val);    return val; } 
            if (type == SHORT)      { short val;    short.TryParse(serial, out val);    return val; }
            if (type == INT)        { int val;      int.TryParse(serial, out val);      return val; }
            if (type == LONG)       { long val;     long.TryParse(serial, out val);     return val; }
            if (type == BYTE)       { byte val;     byte.TryParse(serial, out val);     return val; }
            if (type == USHORT)     { ushort val;   ushort.TryParse(serial, out val);   return val; }
            if (type == UINT)       { uint val;     uint.TryParse(serial, out val);     return val; }
            if (type == ULONG)      { ulong val;    ulong.TryParse(serial, out val);    return val; }
            if (type == FLOAT)      { float val;    float.TryParse(serial, out val);    return val; }
            if (type == DOUBLE)     { double val;   double.TryParse(serial, out val);   return val; }
            if (type == DECIMAL)    { decimal val;  decimal.TryParse(serial, out val);  return val; }
            if (type == BOOL)       { bool val;     bool.TryParse(serial, out val);     return val; }
            if (type == STRING)     { return serial; }
            if (type == CHAR)       { short val;    short.TryParse(serial, out val);    return (char)val; }

            //If this is a Unity Engine object, deserialise the usual way
            if (UOBJ.IsAssignableFrom(type)) {
                //Deserialise the object container
                SerialUnityObjectContainer objectContainer = JsonUtility.FromJson<SerialUnityObjectContainer>(serial);

                //Return the object reference value
                return objectContainer.obj;
            }

            //If the object is an enumeration
            if (ENUM.IsAssignableFrom(type)) {
                //Deserialise a container object
                SerialCollectionContainer container = JsonUtility.FromJson<SerialCollectionContainer>(serial);

                //There should be two values in the container
                if (container.vals.Length != 2) throw new ArgumentException("Serialised enumeration data has two values for processing. Received " + container.vals.Length);

                //Determine the type that is being used
                Type enumType = Type.GetType(container.vals[0]);

                //Get the integral value to use
                long val = Convert.ToInt64(container.vals[1]);

                //Return the final value converted to the enumeration
                return Enum.ToObject(enumType, val);
            }

            //At this point, use JsonUtility to manage other types
            return JsonUtility.FromJson(serial, type);
        }

        //PUBLIC

        /// <summary>
        /// Check to see if the supplied type can be processed
        /// </summary>
        /// <typeparam name="T">The type of object that is being tested</typeparam>
        /// <returns>Returns true if the type can be processed</returns>
        public static bool CanProcess<T>() { return CanProcess(typeof(T)); }

        /// <summary>
        /// Check to see if the supplied type can be processed
        /// </summary>
        /// <param name="type">The type of object that is being tested</param>
        /// <returns>Returns true if the type can be processed</returns>
        public static bool CanProcess(Type type) {
            //Check that the type is valid
            if (type == null) return false;

            //If the type is a List
            if (ILIST.IsAssignableFrom(type)) {
                //Check if this is a List
                if (type.IsGenericType && type.GetGenericTypeDefinition() == LIST) {
                    //This type needs to have 1 generic parameter
                    if (type.GetGenericArguments().Length != 1) return false;

                    //Test the generic value
                    return CanProcessTypeValue(type.GetGenericArguments()[0]);
                }

                //If the object is an array
                else if (type.IsArray)
                    return CanProcessTypeValue(type.GetElementType());

                //Otherwise, no
                return false;
            }

            //Otherwise, basic testing
            return CanProcessTypeValue(type);
        }

        /// <summary>
        /// Convert the supplied object value into a serialised string format that can be re-created as needed
        /// </summary>
        /// <typeparam name="T">The type of object that is being serialised</typeparam>
        /// <param name="obj">The object that is to be serialised</param>
        /// <returns>Returns a string containing the serialised format of the supplied object</returns>
        public static string Serialise<T>(T obj) { return Serialise(obj, typeof(T)); }

        /// <summary>
        /// Convert the supplied object value into a serialised string format that can be re-created as needed
        /// </summary>
        /// <param name="obj">The object that is to be serialised</param>
        /// <param name="type">The type of object that is being serialised</param>
        /// <returns>Returns a string containing the serialised format of the supplied object</returns>
        public static string Serialise(object obj, Type type) {
            //Check that this object can be processed
            if (!CanProcess(type))
                throw new ArgumentException(string.Format("Unable to serialise an object of type '{0}'", type));

            //If this is a collection of objects, process individually
            if (ILIST.IsAssignableFrom(type)) {
                //Get the object as a list
                IList collection = obj as IList;

                //Create a container for the different values
                SerialCollectionContainer container = new SerialCollectionContainer { vals = new string[collection != null ? collection.Count : 0] };

                //Ensure that there are values to process
                if (collection != null) {
                    //Get the type of the contained object
                    Type genericType = null;

                    //Check if this object is a list
                    if (type.IsGenericType && type.GetGenericTypeDefinition() == LIST) {
                        //This type needs to have one generic parameter
                        if (type.GetGenericArguments().Length != 1)
                            throw new ArgumentException(string.Format("Supplied type '{0}' can't be parsed. Type is list but can't determine generic parameter", type));

                        //Use the generic argument type as the value
                        genericType = type.GetGenericArguments()[0];
                    }

                    //If its an array
                    else if (type.IsArray)
                        genericType = type.GetElementType();

                    //Can't use this
                    else throw new ArgumentException(string.Format("Unable to serialise an object of type '{0}'", type));

                    //Serialise the individual values
                    for (int i = 0; i < collection.Count; i++)
                        container.vals[i] = SerialiseValue(collection[i], genericType);
                }

                //Return the serialised object values
                return JsonUtility.ToJson(container);
            }

            //Otherwise, general handling
            return SerialiseValue(obj, type);
        }

        /// <summary>
        /// Parse the supplied text to an object of the specified type
        /// </summary>
        /// <typeparam name="T">The type of object that should be returned by this operation</typeparam>
        /// <param name="serial">The serialised data that is to be parsed to re-create the object definition</param>
        /// <returns>Returns an object of the specified type, re-created from the serialised data</returns>
        public static T Parse<T>(string serial) { return (T)Parse(serial, typeof(T)); }

        /// <summary>
        /// Parse the supplied text to an object of the specified type
        /// </summary>
        /// <param name="serial">The serialised data that is to be parsed to re-create the object definition</param>
        /// <param name="type">The type of the object that is to be returned by this operation</param>
        /// <returns>Returns a generic object of the specified type</returns>
        public static object Parse(string serial, Type type) {
            //Check that this object can be processed
            if (!CanProcess(type))
                throw new ArgumentException(string.Format("Unable to parse an object of type '{0}'", type));

            //If the type is a List
            if (ILIST.IsAssignableFrom(type)) {
                //Store a reference to the object being returned
                IList list = null;

                //Parse the serial data as a container
                SerialCollectionContainer container = JsonUtility.FromJson<SerialCollectionContainer>(serial);

                //Check if this is a List
                if (type.IsGenericType && type.GetGenericTypeDefinition() == LIST) {
                    //Create a new list object from the type
                    list = (IList)Activator.CreateInstance(type, new object[] { container.vals.Length });

                    //This type needs to have one generic parameter
                    if (type.GetGenericArguments().Length != 1)
                        throw new ArgumentException(string.Format("Supplied type '{0}' can't be parsed. Type is list but can't determine generic parameter", type));

                    //Use the generic argument type as the value
                    Type genericType = type.GetGenericArguments()[0];

                    //Add objects to the list as they are parsed
                    for (int i = 0; i < container.vals.Length; i++)
                        list.Add(ParseValue(container.vals[i], genericType));
                }

                //If the object is an array
                else if (type.IsArray) {
                    //Grab the underlying type of the array
                    Type genericType = type.GetElementType();

                    //Create a new array that can hold the returned values
                    list = Array.CreateInstance(genericType, container.vals.Length);

                    //Parse the objects back out as required
                    for (int i = 0; i < container.vals.Length; i++)
                        list[i] = ParseValue(container.vals[i], genericType);
                }

                //Return the created instance
                return list;
            }

            //Otherwise, use basic processing
            return ParseValue(serial, type);
        }

        /// <summary>
        /// Trim down the supplied type assembly name for simple storage
        /// </summary>
        /// <param name="type">A type defintion that is to be trimmed down to its basic information</param>
        /// <returns>Returns the supplied string without additional assembly information</returns>
        /// <remarks>
        /// This function is intended to handle strings produced by the Type.AssemblyQualifiedName property
        /// 
        /// Implementation is taken from the deserialized Argument Cache object within Unity
        /// Reference document https://github.com/jamesjlinden/unity-decompiled/blob/master/UnityEngine/UnityEngine/Events/ArgumentCache.cs
        /// </remarks>
        public static string MinifyTypeAssemblyName(Type type) {
            //Check that there is a unity object type name to clean
            if (type == null) return string.Empty;

            //Get the assembly name of the object
            string typeAssembly = type.AssemblyQualifiedName;

            //Find the point to cut off the type definition
            int point = int.MaxValue;

            //Find the points that are usually included within an assembly type name
            int buffer = typeAssembly.IndexOf(", Version=");
            if (buffer != -1) point = Math.Min(point, buffer);
            buffer = typeAssembly.IndexOf(", Culture=");
            if (buffer != -1) point = Math.Min(point, buffer);
            buffer = typeAssembly.IndexOf(", PublicKeyToken=");
            if (buffer != -1) point = Math.Min(point, buffer);

            //If nothing was found, type is fine
            if (point == int.MaxValue) return typeAssembly;

            //Substring the type to give the shortened version
            return typeAssembly.Substring(0, point);
        }
    }
}