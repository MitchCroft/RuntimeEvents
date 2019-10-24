using System;
using System.Text;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

using RuntimeEvents.ParameterProcessors;

#if !UNITY_5_6_OR_NEWER
#error Runtime Event Drawing requires Unity 5.6 or newer to be be able to be displayed in the inspector
#endif

namespace RuntimeEvents {
    /// <summary>
    /// Handle the displaying of persistent callback information for modification within the inspector
    /// </summary>
    public sealed partial class RuntimeEventDrawer : PropertyDrawer {
        private static class PersistentCallbackDrawer {
            /*----------Types----------*/
            //PRIVATE

            /// <summary>
            /// Callback that is used to setup specific callback values on the supplied callback object
            /// </summary>
            /// <param name="target">The object that is being targetted by the method invocation</param>
            /// <param name="method">Information about the method that will be raised by the </param>
            /// <param name="isDynamic">Flags if this callback is dynamic, receiving values from called event</param>
            private delegate void AssignCallbackDel(UnityEngine.Object target, PersistentMethod method, bool isDynamic);

            /*----------Variables----------*/
            //CONST

            /// <summary>
            /// Store the label that will be used when the stored function is missing
            /// </summary>
            private const string MISSING_LABEL = "<Missing>";

            /// <summary>
            /// Store the label that will be displayed when no function has been selected
            /// </summary>
            private static readonly GUIContent NO_SELECTION_LABEL = new GUIContent("No Function", "No function will be raised for this persistent callback");

            /// <summary>
            /// Store the height that will be used as a buffer between display elements
            /// </summary>
            private static readonly float BUFFER_SPACE = EditorGUIUtility.singleLineHeight * .25f;

            /*----------Functions----------*/
            //PRIVATE

            /// <summary>
            /// Generate a rectangle for rendering points based on the supplied containing rectangle and required elements
            /// </summary>
            /// <param name="container">The parent container that is used to position the offset rectangle</param>
            /// <param name="line">The line number that the offset should be generated for</param>
            /// <param name="widthPercentage">0-1 scale defining the width of the container that should be used for the offset</param>
            /// <param name="insetPercentage">0-1 scale defining the width of the container that should be offset in from the left</param>
            /// <returns>Returns a rectangle that can be used for rendering UI elements</returns>
            private static Rect GetLineOffsetPosition(Rect container, int line, float widthPercentage, float insetPercentage = 0f) {
                return new Rect(
                    container.x + container.width * insetPercentage, 
                    container.y + (EditorGUIUtility.singleLineHeight + BUFFER_SPACE) * line, 
                    container.width * widthPercentage, 
                    EditorGUIUtility.singleLineHeight
                );
            }

            /// <summary>
            /// Assign the supplied persistent method to the supplied callback object
            /// </summary>
            /// <param name="callbacks">The persistent callback objects that will be populated with the method</param>
            /// <param name="target">The object that is being targetted by the method invocation</param>
            /// <param name="method">Information about the method that will be raised by the </param>
            /// <param name="isDynamic">Flags if this callback is dynamic, receiving values from called event</param>
            private static void AssignPersistentCallback(PersistentCallback[] callbacks, UnityEngine.Object target, PersistentMethod method, bool isDynamic) {
                //Loop through the callback objects and assign the method values
                for (int i = 0; i < callbacks.Length; i++) {
                    //Setup the default values for the parameters that can be raised
                    int ind = 0;
                    PersistentCallback.PersistentCallbackParameterInfo[] parameterInfos = new PersistentCallback.PersistentCallbackParameterInfo[method.Parameters.Length];
                    foreach (PersistentParameter param in method.Parameters) {
                        //Store the default value that will be assigned to the parameter
                        object defaultValue = null;

                        //Check if there is a default value for this entry
                        if (param.DefaultValue is DBNull) defaultValue = method.ParameterSignature[ind].GetDefaultValue();
                        else defaultValue = param.DefaultValue;

                        //Assign the values to the parameter info object
                        parameterInfos[ind] = new PersistentCallback.PersistentCallbackParameterInfo();
                        parameterInfos[ind].ParameterType = method.ParameterSignature[ind];
                        parameterInfos[ind].ParameterCache.SetValue(defaultValue, method.ParameterSignature[ind]);

                        //Increase the progress
                        ++ind;
                    }


                    //Assign the new values
                    callbacks[i].Target = target;
                    callbacks[i].MethodName = method.MethodName;
                    callbacks[i].ParameterInfo = parameterInfos;
                    callbacks[i].IsDynamic = isDynamic;
                }
            }

            /// <summary>
            /// Retrieve the base object that is being targeted given a reference
            /// </summary>
            /// <param name="target">The currently stored target object that is being targetted</param>
            /// <returns>Returns the root target object relating to the supplied target</returns>
            private static UnityEngine.Object GetBaseTarget(UnityEngine.Object target) {
                return (target is Component ? (target as Component).gameObject : target);
            }

            /// <summary>
            /// Populate a Generic Menu with the available persistent methods on the supplied target
            /// </summary>
            /// <param name="target">The target object that should be scanned for raisable methods</param>
            /// <param name="currentIsValid">Flags if the current callback is valid</param>
            /// <param name="currentIsDynamic">Flags if the current persistent callback is a dynamic one</param>
            /// <param name="selectedMethod">The previous method that has been selected for use</param>
            /// <param name="dynamicTypes">The types that are able to be dynamic in their raising of the callback</param>
            /// <param name="resetCallback">A callback that is raised when the no function option is selected for callback(s)</param>
            /// <param name="assignCallback">The callback that will be used to assign the callback values to the callback object</param>
            /// <returns>Returns a Generic Menu that can be displayed to list the available options</returns>
            private static GenericMenu CreateOptionsSelectionMenu(UnityEngine.Object target, bool currentIsValid, bool currentIsDynamic, PersistentMethod selectedMethod, Type[] dynamicTypes, GenericMenu.MenuFunction resetCallback, AssignCallbackDel assignCallback) {
                //Create a generic menu that can be used to display possible options
                GenericMenu optionsMenu = new GenericMenu();

                #if UNITY_2018_2_OR_NEWER
                //Toggle the option to allow for the method to be raised on specific components if there are multiple of the same type
                optionsMenu.allowDuplicateNames = true;
                #endif

                //Store a list of the elements that are to have their values displayed
                List<UnityEngine.Object> searchTargets = new List<UnityEngine.Object>(1);

                //Store a reference to the Game Object to show options for
                GameObject searchObject = GetBaseTarget(target) as GameObject;

                //If there is a search object, use that
                if (searchObject != null) {
                    //Add the search object itself
                    searchTargets.Add(searchObject);

                    //Grab all of the components attached to the object
                    searchTargets.AddRange(searchObject.GetComponents<Component>());
                }

                //Otherwise, just use the target (Scriptable Assets etc.)
                else searchTargets.Add(target);

                //Add a label for the target object
                optionsMenu.AddDisabledItem(new GUIContent(searchTargets[0].name + " "));
                optionsMenu.AddSeparator(string.Empty);

                //Add the clear option to the menu
                optionsMenu.AddItem(NO_SELECTION_LABEL, !currentIsValid, resetCallback);
                optionsMenu.AddSeparator(string.Empty);

                //Store the signature string for dynamic elements
                string dynamicSignature = (dynamicTypes.Length > 0 ? PersistentOptionsUtility.GenerateSignatureLabelString(dynamicTypes) : string.Empty);

                //Process all of the elements to be searched
                foreach (UnityEngine.Object search in searchTargets) {
                    //Retrieve the methods that can be used at this level
                    PersistentMethod[] methods = PersistentOptionsUtility.RetrieveObjectMethods(search);

                    //If there are no methods, skip
                    if (methods.Length == 0) continue;

                    //Sort the methods based on their display labels
                    Array.Sort(methods, (left, right) => {
                        //If the property flag differs, sort based on that
                        if (left.IsProperty != right.IsProperty)
                            return right.IsProperty.CompareTo(left.IsProperty);

                        //Otherwise, go alphabetical
                        return left.DisplayLabel.text.CompareTo(right.DisplayLabel.text);
                    });

                    //Store the starting directory label that will be used for creating the sub directories of options
                    string labelPrefix = search.GetType().Name + "/";

                    //Use a string builder to construct the display names for the different method options
                    StringBuilder sb = new StringBuilder();

                    //Store the target that will be operated on if this option is selected
                    UnityEngine.Object objTarget = search;

                    //Loop through the options to add the dynamic methods
                    if (dynamicTypes.Length > 0) {
                        //Flag if an option was found
                        bool foundOne = false;

                        //Check if any of the options can be used as dynamic types
                        for (int i = 0; i < methods.Length; i++) {
                            //Check the parameter length is the same
                            if (methods[i].ParameterSignature.Length != dynamicTypes.Length)
                                continue;

                            //Flag if this is a valid candidate
                            bool valid = true;
                            for (int j = 0; j < dynamicTypes.Length; j++) {
                                if (methods[i].ParameterSignature[j] != dynamicTypes[j]) {
                                    valid = false;
                                    break;
                                }
                            }

                            //If the method isn't valid, don't bother
                            if (!valid) continue;

                            //Check if this is the first valid option found
                            if (!foundOne) {
                                //Add the initial header element
                                optionsMenu.AddDisabledItem(new GUIContent(labelPrefix + "Dynamic Methods " + dynamicSignature));

                                //Flag one as found
                                foundOne = true;
                            }

                            //Store a collective index for the lambda
                            int ind = i;

                            //Add the option to the menu
                            optionsMenu.AddItem(
                                new GUIContent(
                                    labelPrefix + (methods[i].IsProperty ?
                                        methods[i].MethodName.Substring(PersistentOptionsUtility.SETTER_PROPERTY_PREFIX.Length) :
                                        methods[i].MethodName) +
                                    " ",
                                    methods[i].DisplayLabel.tooltip
                                ),
                                currentIsDynamic && methods[i] == selectedMethod,
                                () => assignCallback(objTarget, methods[ind], true)
                            );
                        }

                        //If an option was found add the ending buffer elements
                        if (foundOne) {
                            optionsMenu.AddDisabledItem(new GUIContent(labelPrefix + " "));
                            optionsMenu.AddDisabledItem(new GUIContent(labelPrefix + "Constant Methods"));
                        }
                    }

                    //Loop through the options to add the persistent options
                    for (int i = 0; i < methods.Length; i++) {
                        //Store a collectible index for the lambda
                        int ind = i;

                        //Clear out the previous string value
                        sb.Length = 0;

                        //Add the current prefix to the entry
                        sb.Append(labelPrefix);

                        //Add the display text to the entry
                        sb.Append(methods[i].DisplayLabel.text);

                        //Check to see if all of the contained entries have drawers
                        bool found = false;

                        //Check if a missing drawer was found
                        for (int j = 0; j < methods[i].Parameters.Length; j++) {
                            if (!ParameterDrawers.HasDrawer(methods[i].ParameterSignature[j], methods[i].Parameters[j].Attribute)) {
                                found = true;
                                break;
                            }
                        }

                        //Check for a missing drawer
                        if (found) sb.Append("\t*");

                        //Add the option to the menu
                        optionsMenu.AddItem(
                            new GUIContent(sb.ToString(), methods[i].DisplayLabel.tooltip),
                            !currentIsDynamic && methods[i] == selectedMethod,
                            () => assignCallback(objTarget, methods[ind], false)
                        );
                    }
                }

                //Return the constructed menu
                return optionsMenu;
            }

            //PUBLIC

            /// <summary>
            /// Find the height of a persistent callback object
            /// </summary>
            /// <param name="property">The property that is to have the height calculated for</param>
            /// <returns>Returns the height in pixels that should be reserved for this element</returns>
            public static float GetElementHeight(SerializedProperty property) {
                //Get the target objects of the property
                PersistentCallback[] callbacks;
                property.GetPropertyValues(out callbacks);

                //Check that there are values to check
                if (callbacks.Length == 0) return 0f;

                //Get the height that is used for the object/method selection elements
                float height = EditorGUIUtility.singleLineHeight * 2 + (BUFFER_SPACE * 2f);

                //Get the method that is in use by the primary (first) callback
                PersistentMethod primary = PersistentOptionsUtility.GetPersistentMethodFromCallback(callbacks[0]);

                //Check if this method is the same across all selections
                bool isDifferent = false;
                for (int i = 1; i < callbacks.Length; i++) {
                    if (primary != PersistentOptionsUtility.GetPersistentMethodFromCallback(callbacks[i])) {
                        isDifferent = true;
                        break;
                    }
                }

                //If the methods are the same then the parameters can be shown
                if (!isDifferent && !callbacks[0].IsDynamic && primary != null) {
                    //Calculate the height required for each drawer
                    int ind = 0;
                    foreach (var param in primary.Parameters) {
                        //Get the drawer for the type
                        AParameterDrawer drawer = ParameterDrawers.GetDrawer(primary.ParameterSignature[ind], param.Attribute);

                        //Collate the parameter cache's that are needed for this callback
                        PersistentParameterCache[] current = new PersistentParameterCache[callbacks.Length];
                        for (int i = 0; i < callbacks.Length; i++)
                            current[i] = callbacks[i].ParameterInfo[ind].ParameterCache;

                        //Retrieve the height for the drawer
                        height += drawer.GetDrawerHeight(current, param.DisplayLabel) + BUFFER_SPACE;

                        //Increment the progress
                        ++ind;
                    }
                }

                //Return the final completed height
                return height;
            }

            /// <summary>
            /// Display the elements of the property within the designated area on the inspector area
            /// </summary>
            /// <param name="position">The position within the inspector that the property should be drawn to</param>
            /// <param name="property">The property that is to be displayed within the inspector</param>
            /// <param name="dynamicTypes">Defines the types that are designated as dynamic types for operation</param>
            /// <param name="forceDirty">An action that can be raised from the generic menus callback to force a dirty of the current elements</param>
            /// <returns>Returns true if an event occurred that caused changes that need to be saved</returns>
            public static bool DrawLayoutElements(Rect position, SerializedProperty property, Type[] dynamicTypes, Action forceDirty) {
                //Get the target objects of the property
                PersistentCallback[] callbacks;
                property.GetPropertyValues(out callbacks);

                //Check that there are objects to be displayed
                if (callbacks.Length == 0) return false;

                //Flag if the height of this object needs to be recalculated
                bool elementsModified = false;

                {
                    //Check if the event state is different for the contained callbacks
                    bool isDifferent = false;
                    for (int i = 1; i < callbacks.Length; i++) {
                        if (callbacks[0].EventState != callbacks[i].EventState) {
                            isDifferent = true;
                            break;
                        }
                    }

                    //Display an option to change the event state of the options
                    using (GUIMixer.PushSegment(isDifferent)) {
                        //Begin checking for UI changes
                        EditorGUI.BeginChangeCheck();

                        //Present the option for changing the event state
                        ERuntimeEventState newState = (ERuntimeEventState)EditorGUI.EnumPopup(GetLineOffsetPosition(position, 0, 1f), GUIContent.none, callbacks[0].EventState);

                        //If the state has changed, apply it to all of the callbacks
                        if (EditorGUI.EndChangeCheck()) {
                            for (int i = 0; i < callbacks.Length; i++)
                                callbacks[i].EventState = newState;
                        }
                    }
                }

                {
                    //Check if the target object is different for the contained callbacks
                    bool isDifferent = false;

                    //Check there are multiple objects to modify
                    if (callbacks.Length > 1) {
                        //Get the target of the initial object
                        UnityEngine.Object baseTarget = GetBaseTarget(callbacks[0].Target);

                        //Compare the base target against the others
                        for (int i = 1; i < callbacks.Length; i++) {
                            if (baseTarget != GetBaseTarget(callbacks[i].Target)) {
                                isDifferent = true;
                                break;
                            }
                        }
                    }

                    //Display a target field for the callback operations
                    using (GUIMixer.PushSegment(isDifferent)) {
                        //Begin checking for UI changes
                        EditorGUI.BeginChangeCheck();

                        //Present the option for changing the target object
                        UnityEngine.Object newTarget = EditorGUI.ObjectField(GetLineOffsetPosition(position, 1, .4f), GUIContent.none, callbacks[0].Target, typeof(UnityEngine.Object), true);

                        //If the target changed
                        if (EditorGUI.EndChangeCheck()) {
                            //Apply the new target to all contained elements
                            for (int i = 0; i < callbacks.Length; i++)
                                callbacks[i].Target = newTarget;

                            //That can cause massive changes
                            elementsModified = true;
                        }
                    }
                }

                //Store the persistent method that is assigned to the primary
                PersistentMethod primaryMethod = PersistentOptionsUtility.GetPersistentMethodFromCallback(callbacks[0]);

                //Store a flag that indicates if the callbacks have different methods
                bool methodsAreDifferent = false;

                //Allow for the modification of selected function if there is a target set
                using (GUILocker.PushSegment(callbacks[0].Target)) {
                    //Retrieve the Content that will be displayed for the function selection selected option
                    GUIContent selectedDisplay;

                    //If a method match could be found, use its display label
                    if (primaryMethod != null) selectedDisplay = primaryMethod.DisplayLabel;

                    //Otherwise, method may be missing
                    else {
                        //Try to retrieve the label for the callback
                        string generatedLabel = PersistentOptionsUtility.GetPersistentMethodLabelFromCallback(callbacks[0]);

                        //If the string is empty, no function has been set yet
                        selectedDisplay = (string.IsNullOrEmpty(generatedLabel) ?
                            NO_SELECTION_LABEL :
                            new GUIContent(generatedLabel + " " + MISSING_LABEL)
                        );
                    }

                    //Check there are multiple callbacks to compare against
                    if (callbacks.Length > 1) {
                        //Check if the selected callbacks for the other callbacks is different
                        for (int i = 1; i < callbacks.Length; i++) {
                            if (primaryMethod != PersistentOptionsUtility.GetPersistentMethodFromCallback(callbacks[i])) {
                                methodsAreDifferent = true;
                                break;
                            }
                        }
                    }

                    //Display the dropdown button for selecting the active callback
                    using (GUIMixer.PushSegment(methodsAreDifferent)) {
                        if (EditorGUI.DropdownButton(GetLineOffsetPosition(position, 1, .6f, .4f), selectedDisplay, FocusType.Passive)) {
                            //Retrieve the constructed menu
                            GenericMenu optionsMenu = CreateOptionsSelectionMenu(callbacks[0].Target, callbacks[0].IsValid, callbacks[0].IsDynamic, primaryMethod, dynamicTypes,
                                //Reset function
                                () => {
                                    //Loop through and reset all of the callback methods
                                    for (int i = 0; i < callbacks.Length; i++)
                                        callbacks[i].ResetMethod();

                                    //Reset the primary method
                                    primaryMethod = null;
                                    methodsAreDifferent = false;

                                    //Values modified, force the calling object to re-calculate this element
                                    forceDirty();
                                },
                                //Assign method function
                                (target, method, isDynamic) => {
                                    //Apply the new method to the option
                                    AssignPersistentCallback(callbacks, target, method, isDynamic);

                                    //Methods are now the same
                                    primaryMethod = method;
                                    methodsAreDifferent = false;

                                    //Values modified, force the calling object to re-calculate this element
                                    forceDirty();
                                });

                            //Display the various options
                            optionsMenu.DropDown(GetLineOffsetPosition(position, 1, .6f, .4f));
                        }
                    }
                }

                //If the primary method is different across the multiple objects
                if (!methodsAreDifferent && !callbacks[0].IsDynamic && primaryMethod != null) {
                    //Store the rect of the last point
                    Rect displayRect = GetLineOffsetPosition(position, 1, 1f);

                    //Loop through all of the parameters for this method
                    int ind = 0;
                    foreach (var param in primaryMethod.Parameters) {
                        //Retrieve the drawer for this parameter
                        AParameterDrawer drawer = ParameterDrawers.GetDrawer(primaryMethod.ParameterSignature[ind], param.Attribute);

                        //Collate the parameter cache's that are needed for this callback
                        PersistentParameterCache[] current = new PersistentParameterCache[callbacks.Length];
                        for (int i = 0; i < callbacks.Length; i++)
                            current[i] = callbacks[i].ParameterInfo[ind].ParameterCache;

                        //Setup the display rect for displaying values correctly given the values
                        displayRect.y += displayRect.height + BUFFER_SPACE;
                        displayRect.height = drawer.GetDrawerHeight(current, param.DisplayLabel);

                        //Draw the elements to the inspector
                        elementsModified |= drawer.DisplayParameterValue(displayRect, current, param.DisplayLabel);

                        //Increment the current progress through the parameters
                        ++ind;
                    }
                }

                //Return the re-calculate flag
                return elementsModified;
            }
        }
    }
}