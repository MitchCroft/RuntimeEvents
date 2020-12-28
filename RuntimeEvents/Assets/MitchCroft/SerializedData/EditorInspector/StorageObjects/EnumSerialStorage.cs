#if UNITY_EDITOR
using System;

using UnityEngine;

namespace MitchCroft.SerializedData.EditorInspector.StorageObjects {
    /// <summary>
    /// Store enumeration values that can be displayed within the inspector for modification
    /// </summary>
    [CustomSerialStorage(typeof(Enum), true)]
    public sealed class EnumSerialStorage : SerialStorage {
        /*----------Variables----------*/
        //INVISIBLE

        /// <summary>
        /// Store the type of enumeration that is stored in this object for retrieval
        /// </summary>
        private Type enumType;

        //VISIBLE

        /// <summary>
        /// The numerical representation of the enum value that is to be modified
        /// </summary>
        [SerializeField] private int enumValue;

        /*----------Functions----------*/
        //PUBLIC

        /// <summary>
        /// Try to assign an enumeration value to this storage object for serialised display
        /// </summary>
        /// <param name="value">The enumeration value that is to be shown in the inspector</param>
        /// <returns>Returns true if the value is a valid enumeration value</returns>
        public override bool SetValue(object value) {
            // Reset the stored enum type
            enumType = null;

            // If the value is null then we have a problem
            if (object.Equals(value, null))
                return false;

            // Get the enum type of the value being set
            Type incoming = value.GetType();
            if (!incoming.IsEnum)
                return false;

            // Save the values that will be needed for display
            enumType = incoming;
            enumValue = Convert.ToInt32(value);
            return true;
        }

        /// <summary>
        /// Retrieve the underlying enumeration value that is located within this storage object
        /// </summary>
        /// <returns>Returns the enumeration value or null if unable</returns>
        public override object GetValue() {
            // Check that there is a valid type to be converted to
            return (enumType == null || !enumType.IsEnum ?
                null :
                Enum.ToObject(enumType, enumValue)
            );
        }

        /// <summary>
        /// Retrieve the name of the serial property that will be displayed within the inspector
        /// </summary>
        /// <returns>Returns the name of the container field</returns>
        public override string GetValuePropertyName() { return "enumValue"; }
    }
}
#endif