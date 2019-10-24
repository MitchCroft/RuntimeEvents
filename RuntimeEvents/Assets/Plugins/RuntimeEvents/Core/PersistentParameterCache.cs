using System;

using UnityEngine;

namespace RuntimeEvents {
    /// <summary>
    /// Store a collection of serializable information that is needed for raising persistent 
    /// function callbacks at runtime
    /// </summary>
    [Serializable] public sealed class PersistentParameterCache : ISerializationCallbackReceiver {
        /*----------Variables----------*/
        //PRIVATE

        /// <summary>
        /// The type of this parameter option 
        /// </summary>
        [SerializeField] private string typeString;

        /// <summary>
        /// The serialised data that makes up this cached object value
        /// </summary>
        [SerializeField] private string data;

        /*----------Properties----------*/
        //PUBLIC

        /// <summary>
        /// The type of the value that is assigned to this cache object
        /// </summary>
        public Type ParameterType { get; private set; }

        /// <summary>
        /// The value that is contained by this object
        /// </summary>
        public object Value { get; private set; }

        /*----------Functions----------*/
        //PUBLIC

        /// <summary>
        /// Assign a new value to this cache object for storage
        /// </summary>
        /// <param name="value">The value that is to be stored within this cache</param>
        /// <param name="type">The type of the value that is to be stored within this cache</param>
        public void SetValue(object value, Type type) {
            //Check that the type can be stored within this object
            if (!GenericSerialisation.CanProcess(type))
                throw new ArgumentException(string.Format("PersistentParameterCache can not store unsupported type '{0}'", type));

            //Stash the values
            ParameterType = type;
            Value = value;
        }

        /// <summary>
        /// Convert the current object data into a serialisable format
        /// </summary>
        public void OnBeforeSerialize() {
            typeString = GenericSerialisation.MinifyTypeAssemblyName(ParameterType);
            data = GenericSerialisation.Serialise(Value, ParameterType);
        }

        /// <summary>
        /// Convert the stored serialised data back into a usable object form
        /// </summary>
        public void OnAfterDeserialize() {
            ParameterType = Type.GetType(typeString);
            Value = GenericSerialisation.Parse(data, ParameterType);
        }
    }
}