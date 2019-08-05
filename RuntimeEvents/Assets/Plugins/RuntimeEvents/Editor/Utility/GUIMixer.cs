using System;
using System.Collections.Generic;

using UnityEditor;

namespace RuntimeEvents {
    /// <summary>
    /// Allow for nested modification of the Editors Mixed Value state
    /// </summary>
    public static class GUIMixer {
        /*----------Types----------*/
        //PUBLIC
        
        /// <summary>
        /// Basic object to allow for using statements to mix GUI elements through the use of "using" blocks
        /// </summary>
        public sealed class GUIMix : IDisposable {
            /*----------Properties----------*/
            //PUBLIC
            
            /// <summary>
            /// Flag if the UI is currently mixed within the current state
            /// </summary>
            public bool GUIMixed { get; private set; }

            /*----------Functions----------*/
            //PUBLIC
            
            /// <summary>
            /// Initialise the mix element with basic information about its state
            /// </summary>
            /// <param name="mix">Flags if the UI is mixed at this specific point</param>
            public GUIMix(bool mix) { GUIMixed = mix; }

            /// <summary>
            /// Implicitly raise <see cref="GUIMixer.PopSegment"/> to control mix state sections
            /// </summary>
            public void Dispose() { GUIMixer.PopSegment(); }
        }

        /*----------Variables----------*/
        //PRIVATE

        /// <summary>
        /// Store a stack of mixed UI elements to allow for the mass modification of mixed elements
        /// </summary>
        private static Stack<bool> mixStack = new Stack<bool>();

        /*----------Functions----------*/
        //PUBLIC

        /// <summary>
        /// Add a mix segment to the current stack, returning the mix information object
        /// </summary>
        /// <param name="mixed">Flags if the UI should be treated as mixed value</param>
        /// <returns>A object descriptor that implicitly raises the pop segment when it is disposed</returns>
        public static GUIMix PushSegment(bool mixed) {
            //Push the current state onto the stack
            mixStack.Push(EditorGUI.showMixedValue);

            //Set the new mixed state
            EditorGUI.showMixedValue |= mixed;

            //Return a mix object
            return new GUIMix(EditorGUI.showMixedValue);
        }

        /// <summary>
        /// Add a mix segment based on the equivalence of the supplied objects
        /// </summary>
        /// <param name="equatable">A collection of objects that must all be equal to not create a mixed section</param>
        /// <returns>A object descriptor that implicitly raises the pop segment when it is disposed</returns>
        public static GUIMix PushSegment(params object[] equatable) {
            //Check that all values are equal
            bool equal = true;
            if (equatable.Length > 1) {
                for (int i = 1; i < equatable.Length; i++) {
                    if (object.Equals(equatable[0], equatable[i])) {
                        equal = false;
                        break;
                    }
                }
            }

            //Push a new segment on based on the values
            return PushSegment(!equal);
        }

        /// <summary>
        /// Remove the last segment on the mix stack
        /// </summary>
        public static void PopSegment() {
            //Check there are elements on the stack
            if (mixStack.Count == 0) throw new InvalidOperationException("GUIMixer had the PopSegment functionality raised, there are no locks on the stack");

            //Get the next value
            EditorGUI.showMixedValue = mixStack.Pop();
        }
    }
}