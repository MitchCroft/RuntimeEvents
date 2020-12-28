#if UNITY_EDITOR
using System;

namespace MitchCroft.SerializedData.EditorInspector {
    /// <summary>
    /// Identify an attached type for use in conjunction with an aspect of <see cref="DataPropertyDrawer"/> display process
    /// </summary>
    public abstract class TypeMarkerAttribute : Attribute {
        /*----------Properties----------*/
        //PUBLIC

        /// <summary>
        /// The type object that that is associated with the attached Type
        /// </summary>
        public Type AssociatedType { get; set; }

        /// <summary>
        /// Flags if child classes of the associated type should also be managed by this type
        /// </summary>
        public bool UseForChildren { get; set; }

        /*----------Functions----------*/
        //PUBLIC

        /// <summary>
        /// Allow for default initialisation of this type object
        /// </summary>
        public TypeMarkerAttribute() {}

        /// <summary>
        /// Initialise this object with base values to be tracked for processing
        /// </summary>
        /// <param name="associatedType">The type object that that is associated with the attached Type</param>
        /// <param name="useForChildren">Flags if child classes of the associated type should also be managed by this type</param>
        public TypeMarkerAttribute(Type associatedType, bool useForChildren = false) {
            AssociatedType = associatedType;
            UseForChildren = useForChildren;
        }

        /// <summary>
        /// Return a string representation of this attribute
        /// </summary>
        /// <returns>Returns a string of the properties</returns>
        public override string ToString() {
            return string.Format("{0} (Handle Children = {1})", AssociatedType != null ? AssociatedType.Name : "Null", UseForChildren);
        }

        /// <summary>
        /// Override the default hash value process to base it on contained values
        /// </summary>
        /// <returns>Returns the combined hash code of the properties for identification</returns>
        public override int GetHashCode() {
            unchecked {
                int hash = 17;
                hash = hash * 31 + (AssociatedType != null ? AssociatedType.GetHashCode() : 0);
                hash = hash * 31 + UseForChildren.GetHashCode();
                return hash;
            }
        }
    }
}
#endif