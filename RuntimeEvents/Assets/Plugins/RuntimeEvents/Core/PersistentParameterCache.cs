using System;

using UnityEngine;

namespace RuntimeEvents {
    /// <summary>
    /// Store a collection of serializable information that is needed for raising persistent 
    /// function callbacks at runtime
    /// </summary>
    [Serializable] public sealed class PersistentParameterCache {
        /*----------Variables----------*/
        //PRIVATE

        /// <summary>
        /// A UnityEngine.Object reference that can be assigned to this object
        /// </summary>
        [SerializeField] private UnityEngine.Object unityObject;

        /// <summary>
        /// A string value that defines a type object that can be used for evaluation
        /// </summary>
        [SerializeField] private string typeValue;

        /// <summary>
        /// An integer value that can be assigned to this object
        /// </summary>
        [SerializeField] private int intValue;

        /// <summary>
        /// A float value that can be assigned to this object
        /// </summary>
        [SerializeField] private float floatValue;

        /// <summary>
        /// A string value that can be assigned to this object
        /// </summary>
        [SerializeField] private string stringValue;

        /// <summary>
        /// A boolean value that can be assigned to this object
        /// </summary>
        [SerializeField] private bool boolValue;

        /*----------Properties----------*/
        //PUBLIC

        /// <summary>
        /// Get and set the Unity Object reference that is stored within this object
        /// </summary>
        public UnityEngine.Object UnityObject {
            get { return unityObject; }
            set { unityObject = value; }
        }

        /// <summary>
        /// Get the Type object describing the currently assigned Type Value
        /// </summary>
        /// <remarks>This will return null if there is no valid type string assigned</remarks>
        public Type TypeValue {
            set { typeValue = (value != null ? MinifyTypeAssemblyName(value.AssemblyQualifiedName) : string.Empty); }
            get { return (string.IsNullOrEmpty(typeValue) ? null : Type.GetType(typeValue, false)); }
        }

        /// <summary>
        /// Get and set the integer value that is stored within this object
        /// </summary>
        public int IntValue {
            get { return intValue; }
            set { intValue = value; }
        }

        /// <summary>
        /// Get and set the float value that is stored within this object
        /// </summary>
        public float FloatValue {
            get { return floatValue; }
            set { floatValue = value; }
        }

        /// <summary>
        /// Get and set the string value that is stored within this object
        /// </summary>
        public string StringValue {
            get { return stringValue; }
            set { stringValue = value; }
        }

        /// <summary>
        /// Get and set the boolean value that is stored within this object
        /// </summary>
        public bool BoolValue {
            get { return boolValue; }
            set { boolValue = value; }
        }

        /*----------Functions----------*/
        //PUBLIC
        
        /// <summary>
        /// Reset all stored cache values to their default 
        /// </summary>
        public void ResetValues() {
            unityObject = null;
            typeValue = string.Empty;
            intValue = 0;
            floatValue = 0f;
            stringValue = string.Empty;
            boolValue = false;
            boolValue = false;
        }

        //STATIC

        /// <summary>
        /// Trim down the supplied type assembly name for simple storage
        /// </summary>
        /// <param name="typeAssembly">A type defintion string that is to be trimmed down to its basic information</param>
        /// <returns>Returns the supplied string without additional assembly information</returns>
        /// <remarks>
        /// This function is intended to handle strings produced by the Type.AssemblyQualifiedName property
        /// 
        /// Implementation is taken from the deserialized Argument Cache object within Unity
        /// Reference document https://github.com/jamesjlinden/unity-decompiled/blob/master/UnityEngine/UnityEngine/Events/ArgumentCache.cs
        /// </remarks>
        public static string MinifyTypeAssemblyName(string typeAssembly) {
            //Check that there is a unity object type name to clean
            if (string.IsNullOrEmpty(typeAssembly)) return string.Empty;

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