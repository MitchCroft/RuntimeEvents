using System;

using UnityEngine;

namespace RuntimeEvents.ParameterProcessors {
    /// <summary>
    /// Provide the base required functions that are needed for setting up and displaying 
    /// parameter display/modification options for custom types in the inspector
    /// </summary>
    public abstract class AParameterDrawer {
        /*----------Properties----------*/
        //PUBLIC

        /// <summary>
        /// The type of the value that is being processed by this Drawer
        /// </summary>
        /// <remarks>
        /// This type will match that of the attached CustomParameterDrawerAttribute unless applying to children
        /// </remarks>
        public Type Processing { get; set; }

        /// <summary>
        /// Retrieve the custom attribute that has been attached to this drawer for the rendering operation
        /// </summary>
        public ParameterAttribute Attribute { get; set; }

        /// <summary>
        /// Retrieve the Value Processor that is attached for processing this Drawers value element
        /// </summary>
        public AParameterProcessor Processor { get; set; }

        /*----------Functions----------*/
        //PUBLIC

        /// <summary>
        /// Provide a method for retrieving height within the inspector is required by this drawer to display the parameter value
        /// </summary>
        /// <param name="parameterCaches">The parameter caches for the currently selected objects that store the current values of this parameter</param>
        /// <param name="label">The label that is attached to this parameter to be displayed</param>
        /// <returns>Returns the height within the inspector that this drawer will take up</returns>
        public abstract float GetDrawerHeight(PersistentParameterCache[] parameterCaches, GUIContent label);

        /// <summary>
        /// Provide a method that can be populated to display the required UI elements within the inspector
        /// </summary>
        /// <param name="position">The position within the inspector that the UI elements should be displayed</param>
        /// <param name="parameterCaches">The parameter caches for the currently selected objects that store the current values of this parameter</param>
        /// <param name="label">The label that is attached to this parameter to be displayed</param>
        /// <returns>Returns true if an event occurred that requires the updating of cached values</returns>
        public abstract bool DisplayParameterValue(Rect position, PersistentParameterCache[] parameterCaches, GUIContent label);
    }
}