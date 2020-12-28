#if UNITY_EDITOR
using System;

namespace MitchCroft.SerializedData.EditorInspector {
    /// <summary>
    /// Mark a <see cref="SerialStorage"/> object for the type that is to be stored within it
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class CustomSerialStorageAttribute : TypeMarkerAttribute, IEquatable<CustomSerialStorageAttribute> {
        /*----------Variables----------*/
        //PUBLIC

        /// <summary>
        /// Initialise with nominated value
        /// </summary>
        /// <param name="storageType">The type that will be stored in the associated <see cref="SerialStorage"/> object</param>
        /// <param name="useForChildren">Flags if this storage should be used for child classes</param>
        public CustomSerialStorageAttribute(Type storageType, bool useForChildren = false) : base(storageType, useForChildren) {}

        /// <summary>
        /// Check if this object is equal to another object based on internal values
        /// </summary>
        /// <param name="obj">The other object to be compared against</param>
        /// <returns>Returns true if both objects represent the same values</returns>
        public override bool Equals(object obj) { return Equals(obj as CustomSerialStorageAttribute); }

        /// <summary>
        /// Check if this object is equal to another object based on internal values
        /// </summary>
        /// <param name="obj">The other object to be compared against</param>
        /// <returns>Returns true if both objects represent the same values</returns>
        public bool Equals(CustomSerialStorageAttribute obj) {
            // Check null values
            if (this == null && obj == null) return true;
            if (this == null) return false;
            if (obj == null) return false;

            // Check that both of the contained values are equal
            return (this.AssociatedType == obj.AssociatedType &&
                    this.UseForChildren == obj.UseForChildren);
        }

        /// <summary>
        /// Let the base implementation handle the generating of the hash code 
        /// </summary>
        /// <returns>Returns a numerical representation of the contained values</returns>
        public override int GetHashCode() { return base.GetHashCode(); }
    }
}
#endif