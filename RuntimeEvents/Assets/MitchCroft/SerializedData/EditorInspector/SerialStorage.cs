#if UNITY_EDITOR
using System;

using UnityEngine;

namespace MitchCroft.SerializedData.EditorInspector {
    /// <summary>
    /// Base point for the storage and retrieval of generic data that can be displayed within the inspector
    /// </summary>
    public abstract class SerialStorage : ScriptableObject {
        /*----------Functions----------*/
        //PUBLIC

        /// <summary>
        /// Assign the specified generic data to the current storage object
        /// </summary>
        /// <param name="value">The object value that is to be assigned to the storage container</param>
        /// <returns>Returns true if the value could be assigned to the storage container</returns>
        public abstract bool SetValue(object value);

        /// <summary>
        /// Retrieve the value that is stored within this data storage object for use
        /// </summary>
        /// <returns>Returns the value that is stored within this object for use</returns>
        public abstract object GetValue();

        /// <summary>
        /// Get the name of the value property within this storage object for display
        /// </summary>
        /// <returns>Returns the name of the string that is to be displayed</returns>
        public abstract string GetValuePropertyName();
    }

    /// <summary>
    /// Store the serialised representation of a single type value for display within the inspector
    /// </summary>
    /// <typeparam name="T">The type of value that is to be stored within this asset</typeparam>
    public abstract class SerialStorage<T> : SerialStorage {
        /*----------Variables----------*/
        //SHARED

        /// <summary>
        /// Store the type that will be stored within this asset for checks
        /// </summary>
        private static readonly Type STORAGE_TYPE = typeof(T);

        //VISIBLE

        /// <summary>
        /// The data value that will be serialised for display within the inspector
        /// </summary>
        [SerializeField] protected T serialValue = default(T);

        /*----------Properties----------*/
        //PUBLIC
        
        /// <summary>
        /// Typed access to the value stored within this container 
        /// </summary>
        public T TypedValue {
            get { return serialValue; }
            set { serialValue = value; }
        }

        /*----------Functions----------*/
        //PUBLIC

        /// <summary>
        /// Assign the specified generic value to this storage object for display
        /// </summary>
        /// <param name="value">The value that is to be assigned to the object</param>
        /// <returns>Returns true if the value was assigned successfully</returns>
        public override bool SetValue(object value) {
            // If the value is null, this type must support null
            if (object.Equals(value, null)) {
                if (STORAGE_TYPE.IsValueType)
                    return false;
            }

            // Otherwise, if the object type isn't assignable
            else if (!STORAGE_TYPE.IsAssignableFrom(value.GetType()))
                return false;

            // Save the value that is to be used
            serialValue = (T)value;
            return true;
        }

        /// <summary>
        /// Retrieve the generic value that is stored within this storage object
        /// </summary>
        /// <returns>Returns the contained value as a basic object type</returns>
        public override object GetValue() {
            return (T)serialValue;
        }

        /// <summary>
        /// Retrieve the name of the serialised field that will be generated for the storage object(s)
        /// </summary>
        /// <returns>Returns the name of the typed field</returns>
        public override string GetValuePropertyName() { return "serialValue"; }
    }
}
#endif