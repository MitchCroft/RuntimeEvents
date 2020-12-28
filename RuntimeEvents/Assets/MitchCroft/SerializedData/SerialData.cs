using System;

using UnityEngine;

namespace MitchCroft.SerializedData {
    /// <summary>
    /// Store generic data that can be serialised and displayed within the inspector
    /// </summary>
    /// <remarks>
    /// Data types that are stored must still follow the standard Unity serialisation rules
    /// </remarks>
    [Serializable]
    public sealed class SerialData : ISerializationCallbackReceiver {
        /*----------Variables----------*/
        //PRIVATE

        /// <summary>
        /// The string representation of the type of the value that is stored within this object
        /// </summary>
        [SerializeField] private string typeString;

        /// <summary>
        /// The string representation of the data that makes up this cached object value
        /// </summary>
        [SerializeField] private string data;

        /*----------Properties----------*/
        //PUBLIC

        /// <summary>
        /// Flags if the data within this object is valid for use
        /// </summary>
        public bool IsValid { get; private set; }

        /// <summary>
        /// The type of the data that is stored within this object
        /// </summary>
        public Type DataType { get; private set; }

        /// <summary>
        /// The value that has been assigned to this object
        /// </summary>
        public object Value { get; private set; }

        /// <summary>
        /// An optional data modifier that can be associated with this data
        /// </summary>
        public IGenericDataModifier Modifier { get; set; }

        /*----------Functions----------*/
        //PUBLIC

        /// <summary>
        /// Initialise with default values
        /// </summary>
        public SerialData() {}

        /// <summary>
        /// Initialise this generic data value with the specified value
        /// </summary>
        /// <param name="value">The *non-null* value that is to be stored within this object</param>
        public SerialData(object value) {
            // If the value is null then a type can't be determined
            if (object.Equals(value, null))
                throw new ArgumentNullException("Can't initialise SerialData with null value, can't infer type");
            SetValue(value, value.GetType());
        }

        /// <summary>
        /// Initialise this generic data value with the specified value
        /// </summary>
        /// <param name="value">The value data that is to be kept in this object</param>
        /// <param name="type">The type of the object that is to be stored</param>
        public SerialData(object value, Type type) { SetValue(value, type); }

        /// <summary>
        /// Assign a new value to this generic data collection
        /// </summary>
        /// <param name="value">The value data that is to be kept in this object</param>
        /// <param name="type">The type of the object that is to be stored</param>
        /// <returns>Returns true if the value will be serialised for storage/display</returns>
        public bool SetValue<T>(T value) { return SetValue(value, typeof(T)); }

        /// <summary>
        /// Assign a new value to this generic data collection
        /// </summary>
        /// <param name="value">The value data that is to be kept in this object</param>
        /// <param name="type">The type of the object that is to be stored</param>
        /// <returns>Returns true if the value will be serialised for storage/display</returns>
        public bool SetValue(object value, Type type) {
            // Set the valid flag based on the received values
            if (!GenericSerialisation.CanProcess(type))
                IsValid = false;
            else if (object.Equals(value, null))
                IsValid = !type.IsValueType;
            else IsValid = type.IsAssignableFrom(value.GetType());

            // Save the values for use
            DataType = type;
            Value = value;

            return IsValid;
        }

        /// <summary>
        /// Maintain the serialised data so that it is able to be reproduced
        /// </summary>
        public void OnBeforeSerialize() {
            if (IsValid && DataType != null) {
                typeString = GenericSerialisation.MinifyTypeAssemblyName(DataType);
                data = GenericSerialisation.Serialise(Value, DataType);
            } else {
                typeString = string.Empty;
                data = string.Empty;
            }
        }

        /// <summary>
        /// De-serialise the stored data so that it can be used
        /// </summary>
        public void OnAfterDeserialize() {
            DataType = Type.GetType(typeString, false);
            IsValid = GenericSerialisation.CanProcess(DataType);
            Value = (IsValid ?
                GenericSerialisation.Parse(data, DataType) :
                null
            );
        }
    }
}