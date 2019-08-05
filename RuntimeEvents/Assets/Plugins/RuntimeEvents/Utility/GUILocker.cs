using System;
using System.Collections.Generic;

using UnityEngine;

namespace RuntimeEvents {
    /// <summary>
    /// Allow for nested locking of UI based on evaluated conditions
    /// </summary>
    public static class GUILocker {
        /*----------Types----------*/
        //PUBLIC

        /// <summary>
        /// Basic object to allow for using statements to block out GUI lockout sections through the use of "using" blocks
        /// </summary>
        public sealed class GUILock : IDisposable {
            /*----------Properties----------*/
            //PUBLIC
            
            /// <summary>
            /// Flag if the UI is active within the current section
            /// </summary>
            public bool GUIActive { get; private set; }

            /*----------Functions----------*/
            //PUBLIC

            /// <summary>
            /// Initialise the lock with basic information about its state
            /// </summary>
            /// <param name="state">Flags if the UI is active at this specific point</param>
            public GUILock(bool state) { GUIActive = state; }

            /// <summary>
            /// Implicitly raise <see cref="GUILocker.PopSegment"/> to control lock state sections
            /// </summary>
            public void Dispose() { GUILocker.PopSegment(); }
        }

        /*----------Variables----------*/
        //PRIVATE

        /// <summary>
        /// Store a stack of GUI enabled states to allow for the mass modification of GUI enabled elements
        /// </summary>
        private static Stack<bool> lockStack = new Stack<bool>();

        /*----------Functions----------*/
        //PUBLIC

        /// <summary>
        /// Add a lock segment to the current stack, returning a locking information object
        /// </summary>
        /// <param name="active">Flags if the UI in the next section should be locked off</param>
        /// <returns>A object descriptor that implicitly raises the pop segment when it is disposed</returns>
        public static GUILock PushSegment(bool active) {
            //Push the current state onto the stack
            lockStack.Push(GUI.enabled);

            //Set the UI state
            GUI.enabled &= active;

            //Return a lock object
            return new GUILock(GUI.enabled);
        }

        /// <summary>
        /// Remove the last segment on the lock stack
        /// </summary>
        public static void PopSegment() {
            //Check there are elements on the stack
            if (lockStack.Count == 0) throw new InvalidOperationException("GUILocker had the PopSegment functionality raised, there are no locks on the stack");

            //Get the next value
            GUI.enabled = lockStack.Pop();
        }
    }
}