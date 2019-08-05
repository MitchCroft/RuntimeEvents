using System;

namespace RuntimeEvents {
    /// <summary>
    /// Define a processor class that can be used to process the rendering of Parameter Attribute. This attribute should be attached to 
    /// a /*INSERT HERE*/ class object
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class CustomParameterDrawerAttribute : Attribute {
        /*----------Properties----------*/
        //PUBLIC

        /// <summary>
        /// The value type that the attached class will be used to process 
        /// </summary>
        public Type DrawerType { get; private set; }

        /// <summary>
        /// Flags if children of the supplied type should also be handled by this drawer
        /// </summary>
        public bool HandleChildren { get; private set; }

        /*----------Functions----------*/
        //PUBLIC

        /// <summary>
        /// Initialise this object with it's base values
        /// </summary>
        /// <param name="drawerType">The type object defining the value type that the attached drawer is responsible for</param>
        /// <param name="handleChildren">Flags if child classes of the drawer type should be handled by this drawer</param>
        public CustomParameterDrawerAttribute(Type drawerType, bool handleChildren = false) { DrawerType = drawerType; HandleChildren = handleChildren; }

        /// <summary>
        /// Override the default behaviour to base the hash code of the attribute off of the stored values
        /// </summary>
        /// <returns>Returns a combined hash to reflect the assigned values</returns>
        public override int GetHashCode() {
            unchecked {
                int hash = 17;
                hash = hash * 31 + DrawerType.GetHashCode();
                hash = hash * 31 + HandleChildren.GetHashCode();
                return hash;
            }
        }
    }
}