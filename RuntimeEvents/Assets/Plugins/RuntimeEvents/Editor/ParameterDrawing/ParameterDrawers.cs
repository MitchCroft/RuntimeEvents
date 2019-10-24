using System;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

namespace RuntimeEvents.ParameterProcessors {
    /// <summary>
    /// Store references to the available parameter drawers that will be used for displaying parameter information within the inspector
    /// </summary>
    public static class ParameterDrawers {
        /*----------Types----------*/
        //PRIVATE

        /// <summary>
        /// Provide a custom comparer object to be used to differentiate attributes based on their values not their instance
        /// </summary>
        private sealed class CustomParameterDrawerComparer : EqualityComparer<CustomParameterDrawerAttribute> {
            /*----------Functions----------*/
            //PUBLIC
            
            /// <summary>
            /// Compare two instances of the Custom Parameter Drawer Attribute to check if they are equivalent
            /// </summary>
            /// <param name="x">The first instance to be checked</param>
            /// <param name="y">The second instance to be checked</param>
            /// <returns>Returns true if both of the instances are equivalent</returns>
            public override bool Equals(CustomParameterDrawerAttribute x, CustomParameterDrawerAttribute y) {
                //Check the possible conditions
                if (x == null && y == null) return true;
                if (x == null || y == null) return true;

                //Check if the hash codes are equivalent
                return (x.GetHashCode() == y.GetHashCode());
            }

            /// <summary>
            /// Wrap the retrieval of the hash code for an attribute
            /// </summary>
            /// <param name="obj">The object that is having its hash code retrieved</param>
            /// <returns>Returns an integer representation of the attributes values</returns>
            public override int GetHashCode(CustomParameterDrawerAttribute obj) { return obj.GetHashCode(); }
        }

        /// <summary>
        /// Define error codes that can be used to display feedback information within the Inspector
        /// </summary>
        private enum EErrorType {
            /// <summary>
            /// An instance of the pre-defined drawer instance failed to be created
            /// </summary>
            DrawerInstanceFailed,

            /// <summary>
            /// There is no drawer defined that can handle the supplied type
            /// </summary>
            DrawerMissing,
        }

        /// <summary>
        /// Provide a basic error description that can be used to display error messages within the inspector when problems occur
        /// </summary>
        private sealed class ErrorParameterDrawer : AErrorReportingDrawer {
            /*----------Variables----------*/
            //PRIVATE

            /// <summary>
            /// Store the error type that this drawer is responsible for displaying
            /// </summary>
            private readonly EErrorType errorType;

            /// <summary>
            /// Store the additional text that is supplied for this drawer 
            /// </summary>
            private readonly string additionalText;

            /*----------Functions----------*/
            //PUBLIC

            /// <summary>
            /// Initialise this drawer with the default values for the required entry
            /// </summary>
            /// <param name="type">The type of error that this drawer is responsible for displaying</param>
            /// <param name="additional">An additional message that can be displayed to support the display error</param>
            public ErrorParameterDrawer(EErrorType type, string additional = "") { errorType = type; additionalText = additional; }
            
            /// <summary>
            /// Returns the default size that is required for displaying an error message
            /// </summary>
            /// <returns>Returns a constant value as this drawer is only used to display an error message within the inspector</returns>
            public override float GetDrawerHeight(PersistentParameterCache[] parameterCaches, GUIContent label) { return GetErrorHeight(); }

            /// <summary>
            /// Display the error message within the assigned position within the inspector
            /// </summary>
            /// <param name="position">The position in which the message is to be displayed</param>
            /// <param name="label">The label of the parameter that is going to fail to draw</param>
            /// <returns>Returns a constant false as there are no interactable elements to this drawer</returns>
            public override bool DisplayParameterValue(Rect position, PersistentParameterCache[] parameterCaches, GUIContent label) {
                //Store the message that is to be displayed
                string toDisplay = string.Empty;

                //Switch on the error type that is being displayed
                switch (errorType) {
                    //Known errors
                    case EErrorType.DrawerInstanceFailed:
                        toDisplay = "Failed to create drawer, " + additionalText;
                        break;
                    case EErrorType.DrawerMissing:
                        //Set the base message
                        toDisplay = string.Format("No Drawer could be found for the type '{0}'", Processing.FullName);

                        //If there is an attribute, tack that message on as well
                        if (Attribute != null)
                            toDisplay = string.Format("{0}, or the attached Attribute type '{1}", toDisplay, Attribute.GetType().FullName);
                        break;

                    //Generic option handling
                    default:
                        toDisplay = (string.IsNullOrEmpty(additionalText) ?
                            "An unknown error has occurred" :
                            additionalText
                        );
                        break;
                }

                //Display the error elements
                return DrawErrorMessage(position, label, toDisplay);
            }
        }

        /*----------Variables----------*/
        //PRIVATE

        /// <summary>
        /// Store a lookup map for the different parameter Processor defined objects that exist within the project
        /// </summary>
        /// <remarks>
        /// Mapping for this dictionary is [AttributeMarker, DrawerClass]
        /// </remarks>
        private static Dictionary<CustomParameterDrawerAttribute, Type> parameterDrawers;

        /// <summary>
        /// Store a cache collection of the instantiated drawer objects for each of their respective types
        /// </summary>
        private static Dictionary<CustomParameterDrawerAttribute, AParameterDrawer> cachedDrawers;

        /// <summary>
        /// Store a cached lookup of which drawers respond to each type that is tested with this object
        /// </summary>
        /// <remarks>
        /// Mapping for this dictionary is [TestType, Drawer]
        /// </remarks>
        private static Dictionary<Type, AParameterDrawer> typeMapping;

        /*----------Functions----------*/
        //PRIVATE

        /// <summary>
        /// Initialise the object with the available object definitions
        /// </summary>
        static ParameterDrawers() {
            //Create the lookup dictionaries
            CustomParameterDrawerComparer comp = new CustomParameterDrawerComparer();
            parameterDrawers = new Dictionary<CustomParameterDrawerAttribute, Type>(comp);
            cachedDrawers = new Dictionary<CustomParameterDrawerAttribute, AParameterDrawer>(comp);
            typeMapping = new Dictionary<Type, AParameterDrawer>();

            //Find all of the usable drawers within the project
            foreach (Type type in AssemblyTypeScanner.GetTypesWithinAssembly<AParameterDrawer>()) {
                //Look for the custom drawer processor attribute of the object
                CustomParameterDrawerAttribute descriptor = type.GetFirstCustomAttributeOf<CustomParameterDrawerAttribute>();

                //If there is no attribute can't use it
                if (descriptor == null) continue;

                //Check if the type has already been used
                if (parameterDrawers.ContainsKey(descriptor))
                    Debug.LogWarningFormat("Class '{0}' is being used to Draw for Types '{1}' (Include Children: {2}) and will override the processor '{3}'", type.FullName, descriptor.DrawerType.FullName, descriptor.HandleChildren, parameterDrawers[descriptor].FullName);

                //Save the drawer for later use
                parameterDrawers[descriptor] = type;
            }
        }

        /// <summary>
        /// Retrieve a cached processor that can be used for the drawing of parameters
        /// </summary>
        /// <param name="descriptor">The descriptor defining the drawer to be retrieved</param>
        /// <returns>Retrieves an instance of the drawer that can be used or null if unable</returns>
        private static AParameterDrawer GetDrawer(CustomParameterDrawerAttribute descriptor) {
            //Make sure there is a cached instance for this descriptor
            if (!cachedDrawers.ContainsKey(descriptor)) {
                //Try to create an instance with the identified object
                try { cachedDrawers[descriptor] = (AParameterDrawer)Activator.CreateInstance(parameterDrawers[descriptor]); }
                catch (Exception exec) {
                    Debug.LogErrorFormat("Failed to create an instance of the Parameter Drawer object '{0}'. Error: {1}", parameterDrawers[descriptor], exec.Message);
                    cachedDrawers[descriptor] = new ErrorParameterDrawer(EErrorType.DrawerInstanceFailed, string.Format("Drawer Type '{0}' failed to instantiate. Error: {1}", parameterDrawers[descriptor], exec.Message));
                }
            }

            //Returned the cached instance
            return cachedDrawers[descriptor];
        }

        /// <summary>
        /// Search the supplied objects type chain for a drawer that can be used
        /// </summary>
        /// <param name="type">The type object that will be unwinded to look for a processor</param>
        /// <returns>Returns a Parameter Drawer Attribute object that corresponds to a usable drawer</returns>
        private static CustomParameterDrawerAttribute CheckTypeChain(Type type) {
            //Create the exact descriptor required for this type
            CustomParameterDrawerAttribute buffer = new CustomParameterDrawerAttribute(type, false);

            //Check if the exact type exists that can be used
            if (parameterDrawers.ContainsKey(buffer))
                return buffer;

            //Otherwise, traverse the hierarchy chain
            while (type != null) {
                //Create the type that is being searched for
                buffer = new CustomParameterDrawerAttribute(type, true);

                //If there is an instance for the descriptor, retrieve it
                if (parameterDrawers.ContainsKey(buffer))
                    return buffer;

                //Advance a step up the hierarchy
                type = type.BaseType;
            }

            //If gotten this far, nothing to find
            return null;
        }

        //PUBLIC

        /// <summary>
        /// Check to see if there is a drawer object available for the specified combination of draw values
        /// </summary>
        /// <param name="type">The type of value that is to be displayed</param>
        /// <param name="attribute">The attribute attached to the parameter to effect drawing</param>
        /// <returns>Returns true if there is a contained drawer for the supplied combination</returns>
        public static bool HasDrawer(Type type, ParameterAttribute attribute) {
            //Check that at least one drawer could be found
            if (attribute != null && CheckTypeChain(attribute.GetType()) != null) return true;
            else return CheckTypeChain(type) != null;
        }

        /// <summary>
        /// Retrieve a drawer that can be used to render the supplied type
        /// </summary>
        /// <param name="type">The type of the object that is to be displayed</param>
        /// <param name="attribute">The optional attribute that is attached to the type for consideration</param>
        /// <returns>Returns a Parameter Drawer that will be used to display the value</returns>
        public static AParameterDrawer GetDrawer(Type type, ParameterAttribute attribute) {
            //Store a reference to the drawer that will be used to display the current value
            AParameterDrawer drawer = null;

            //Get the type of the attribute 
            Type attributeType = (attribute != null ? attribute.GetType() : null);

            //Ensure that if there is an attribute, its drawer state is cached
            if (attributeType != null && !typeMapping.ContainsKey(attributeType)) {
                //Retrieve the descriptor for the attribute type
                CustomParameterDrawerAttribute buffer = CheckTypeChain(attributeType);

                //Cache the drawer that has been found for the attribute type
                typeMapping[attributeType] = (buffer != null ?
                    GetDrawer(buffer) :
                    null
                );
            }

            //If there is an attribute drawer now, use that over the type
            if (attribute != null && typeMapping[attributeType] != null)
                drawer = typeMapping[attributeType];

            //Otherwise, a drawer for the type is needed
            else if (!typeMapping.ContainsKey(type)) {
                //Retrieve the descriptor for the attribute type
                CustomParameterDrawerAttribute buffer = CheckTypeChain(type);

                //Store the processor in type mapping based on its existence
                drawer = typeMapping[type] = (buffer != null ?
                    GetDrawer(buffer) :
                    new ErrorParameterDrawer(EErrorType.DrawerMissing)
                );
            }

            //Otherwise, just use the type drawer
            else drawer = typeMapping[type];

            //Assign the base values to the drawer
            drawer.Processing = type;
            drawer.Attribute = attribute;

            //Return the stored drawer
            return drawer;
        }
    }
}