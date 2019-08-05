using System;

namespace RuntimeEvents {
    /// <summary>
    /// Have the inspector draw an area for string elements to be entered into
    /// </summary>
    public sealed class TextFieldAreaAttribute : ParameterAttribute {
        /*----------Properties----------*/
        //PUBLIC

        /// <summary>
        /// Store the height of the area that the text is to be displayed in
        /// </summary>
        public float Height { get; private set; }

        /*----------Functions----------*/
        //PUBLIC

        /// <summary>
        /// Initialise the attribute with the number of lines to use
        /// </summary>
        /// <param name="height">The height of the area that is used to display the text</param>
        public TextFieldAreaAttribute(float height = 20f) { Height = Math.Max(height, 0f); }
    }
}