using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

#if !UNITY_5_6_OR_NEWER
#error Runtime Event Drawing requires Unity 5.6 or newer to be be able to be displayed in the inspector
#endif

namespace RuntimeEvents {
    /// <summary>
    /// Handle the displaying of persistent callback information for modification within the inspector
    /// </summary>
    public sealed partial class RuntimeEventDrawer : PropertyDrawer {
        private static class PersistentCallbackDrawer {
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
            /// <param name="callback">The persistent callback object that will be populated with the method</param>
            /// <param name="target">The object that is being targetted by the method invocation</param>
            /// <param name="method">Information about the method that will be raised by the </param>
            /// <param name="isDynamic">Flags if this callback is dynamic, receiving values from called event</param>
            private static void AssignPersistentCallback(PersistentCallback callback, UnityEngine.Object target, PersistentMethod method, bool isDynamic) {
                //Setup the default values for the parameters that can be raised
                int ind = 0;
                PersistentCallback.PersistentCallbackParameterInfo[] parameterInfos = new PersistentCallback.PersistentCallbackParameterInfo[method.Parameters.Length];
                foreach (PersistentParameter param in method.Parameters) {
                    //Retrieve the processor for this parameter type
                    ParameterProcessors.AParameterProcessor processor = ParameterProcessors.ParameterProcessors.GetProcessor(method.ParameterSignature[ind]);

                    //This should always be successful, but just in case
                    if (processor == null) throw new OperationCanceledException("Unable to retrieve a Parameter Processor instance for the type '" + method.ParameterSignature[ind].FullName + "'");

                    //Store the default value that will be assigned to the parameter
                    object defaultValue = null;

                    //Check if there is a default value for this entry
                    if (param.DefaultValue is DBNull) defaultValue = processor.GetDefaultValue(method.ParameterSignature[ind]);
                    else defaultValue = param.DefaultValue;

                    //Assign the values to the parameter info object
                    parameterInfos[ind] = new PersistentCallback.PersistentCallbackParameterInfo();
                    parameterInfos[ind].ParameterType = method.ParameterSignature[ind];
                    processor.AssignValue(parameterInfos[ind].ParameterCache, defaultValue);

                    //Increase the progress
                    ++ind;
                }

                //Reset the previous method values
                callback.ResetMethod();

                //Assign the new new values
                callback.Target = target;
                callback.MethodName = method.MethodName;
                callback.ParameterInfo = parameterInfos;
                callback.IsDynamic = isDynamic;
            }

            //PUBLIC

            /// <summary>
            /// Find the height of a persistent callback object
            /// </summary>
            /// <param name="property">The property that is to have the height calculated for</param>
            /// <returns>Returns the height in pixels that should be reserved for this element</returns>
            public static float GetElementHeight(SerializedProperty property) {
                return EditorGUIUtility.singleLineHeight * 3f;
            }

            /// <summary>
            /// Display the elements of the property within the designated area on the inspector area
            /// </summary>
            /// <param name="position">The position within the inspector that the property should be drawn to</param>
            /// <param name="property">The property that is to be displayed within the inspector</param>
            /// <param name="dynamicTypes">Defines the types that are designated as dynamic types for operation</param>
            /// <returns>Returns true if an event occurred that caused changes that need to be saved</returns>
            public static bool DrawLayoutElements(Rect position, SerializedProperty property, Type[] dynamicTypes) {
                //Get the target object of the property
                PersistentCallback callback;
                property.GetPropertyValue(out callback);

                //Flag if the height of this object needs to be recalculated
                bool elementsModified = false;

                //Display the option to change the event state of the current callback
                callback.EventState = (ERuntimeEventState)EditorGUI.EnumPopup(GetLineOffsetPosition(position, 0, 1f), GUIContent.none, callback.EventState);

                //Display the target field for callback operation
                UnityEngine.Object newTarget = EditorGUI.ObjectField(GetLineOffsetPosition(position, 1, .3f), GUIContent.none, callback.Target, typeof(UnityEngine.Object), true);
                if (newTarget != callback.Target) {
                    callback.Target = newTarget;
                    elementsModified = true;   //Number of visible elements may have changed
                }

                //Allow for the modification of selected function if there is a target set
                using (GUILocker.PushSegment(newTarget)) {
                    //Retrieve the Content that will be displayed for the function selection selected option
                    GUIContent selectedDisplay;

                    //Try to retrieve the current method described by this callback
                    PersistentMethod selectedMethod = PersistentOptionsUtility.GetPersistentMethodFromCallback(callback);

                    //If a method match could be found, use its display label
                    if (selectedMethod != null) selectedDisplay = selectedMethod.DisplayLabel;

                    //Otherwise, method may be missing
                    else {
                        //Try to retrieve the label for the callback
                        string generatedLabel = PersistentOptionsUtility.GetPersistentMethodLabelFromCallback(callback);

                        //If the string is empty, no function has been set yet
                        selectedDisplay = (string.IsNullOrEmpty(generatedLabel) ?
                            NO_SELECTION_LABEL :
                            new GUIContent(generatedLabel + " " + MISSING_LABEL)
                        );
                    }

                    //Display the dropdown button for selecting the active callback
                    if (EditorGUI.DropdownButton(GetLineOffsetPosition(position, 1, .7f, .3f), selectedDisplay, FocusType.Passive)) {
                        //Create a generic menu that can be used to display possible options
                        GenericMenu optionsMenu = new GenericMenu();

                        #if UNITY_2018_2_OR_NEWER
                        //Toggle the option to allow for the method to be raised on specific components if there are multiple of the same type
                        optionsMenu.allowDuplicateNames = true;
                        #endif

                        //Store a list of the elements that are to have their values displayed
                        List<UnityEngine.Object> searchTargets = new List<UnityEngine.Object>(1);

                        //Store a reference to the Game Object to show options for
                        GameObject searchObject = (newTarget is GameObject ? newTarget as GameObject : (newTarget is Component ? (newTarget as Component).gameObject : null));
                        
                        //If there is a search object, use that
                        if (searchObject != null) {
                            //Add the search object itself
                            searchTargets.Add(searchObject);

                            //Grab all of the components attached to the object
                            searchTargets.AddRange(searchObject.GetComponents<Component>());
                        }

                        //Otherwise, just use the target (Scriptable Assets etc.)
                        else searchTargets.Add(newTarget);

                        //Add the clear option to the menu
                        optionsMenu.AddItem(NO_SELECTION_LABEL, !callback.IsValid, callback.ResetMethod);
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

                            //Store the target that will be operated on if this option is selected
                            UnityEngine.Object target = search;

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
                                        callback.IsDynamic && methods[i] == selectedMethod,
                                        () => {
                                            AssignPersistentCallback(callback, target, methods[ind], true);
                                            elementsModified = true;
                                        }
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

                                //Add the option to the menu
                                optionsMenu.AddItem(
                                    new GUIContent(labelPrefix + methods[i].DisplayLabel.text, methods[i].DisplayLabel.tooltip),
                                    !callback.IsDynamic && methods[i] == selectedMethod,
                                    () => {
                                        AssignPersistentCallback(callback, target, methods[ind], false);
                                        elementsModified = true;
                                    }
                                );
                            }
                        }

                        //Display the various options
                        optionsMenu.DropDown(GetLineOffsetPosition(position, 1, .7f, .3f));
                    }
                }

                //Return the re-calculate flag
                return elementsModified;
            }
        }
    }
}