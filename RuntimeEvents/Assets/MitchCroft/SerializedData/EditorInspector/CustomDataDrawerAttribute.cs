#if UNITY_EDITOR
using System;

namespace MitchCroft.SerializedData.EditorInspector {
    /// <summary>
    /// Identify a <see cref="IDataDrawer"/> object type that can be used to display <see cref="Data"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class CustomDataDrawerAttribute : TypeMarkerAttribute, IEquatable<CustomDataDrawerAttribute> {
        /*----------Functions----------*/
        //PUBLIC

        /// <summary>
        /// Initialise this object with its base values
        /// </summary>
        /// <param name="drawerType">The type of value that will be displayed by this type</param>
        /// <param name="useForChildren">Flags if child classes of the specified type will also be handled by this process</param>
        public CustomDataDrawerAttribute(Type drawerType, bool useForChildren = false) : base(drawerType, useForChildren) {}

        /// <summary>
        /// Check if this object is equal to another object based on internal values
        /// </summary>
        /// <param name="obj">The other object to be compared against</param>
        /// <returns>Returns true if both objects represent the same values</returns>
        public override bool Equals(object obj) { return Equals(obj as CustomDataDrawerAttribute); }

        /// <summary>
        /// Check if this object is equal to another object based on internal values
        /// </summary>
        /// <param name="obj">The other object to be compared against</param>
        /// <returns>Returns true if both objects represent the same values</returns>
        public bool Equals(CustomDataDrawerAttribute obj) {
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