using System;
using System.Text;
using System.Reflection;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

using MethodHash = System.Int32;
using RaiseableType = System.Type;

namespace RuntimeEvents {
    /// <summary>
    /// Manage the identifying of persistent callbacks that can be selected for Runtime Events
    /// </summary>
    public static class PersistentOptionsUtility {
        /*----------Variables----------*/
        //CONST

        /// <summary>
        /// Define the text that prefixes a setter properties method name
        /// </summary>
        public const string SETTER_PROPERTY_PREFIX = "set_";

        //PRIVATE

        /// <summary>
        /// Store a collection of the identified methods available for the specified objects
        /// </summary>
        private static Dictionary<RaiseableType, Dictionary<MethodHash, PersistentMethod>> objectMethods;

        /*----------Functions----------*/
        //PRIVATE

        /// <summary>
        /// Initialise this object with the default values
        /// </summary>
        static PersistentOptionsUtility() { objectMethods = new Dictionary<RaiseableType, Dictionary<MethodHash, PersistentMethod>>(); }

        /// <summary>
        /// Process the supplied type to establish the usable methods that are contained for it
        /// </summary>
        /// <param name="key">The type that is to be processed to identify usable methods</param>
        private static void ProcessObjectType(Type key) {
            //Find all the methods that belong to the type with the specified flags
            List<MethodInfo> baseMethods = new List<MethodInfo>(key.GetMethods(PersistentCallbackUtility.SEARCH_FLAGS));

            //Remove any method that is not valid
            baseMethods.RemoveAll(method => {
                //If this contains generic parameters, not gonna use it
                if (method.ContainsGenericParameters)
                    return true;

                //If this is a property (With a special name) only allow setters
                if (method.IsSpecialName && !method.Name.StartsWith(SETTER_PROPERTY_PREFIX))
                    return true;

                //Check if there is an exclusion attribute
                if (method.GetFirstCustomAttributeOf<OmitRuntimeEventAttribute>() != null)
                    return true;

                //Retrieve the parameters of the object
                ParameterInfo[] parameters = method.GetParameters();

                //Check that each of the parameter types has a supported type
                foreach (ParameterInfo parm in parameters) {
                    if (!ParameterProcessors.ParameterProcessors.HasProcessor(parm.ParameterType))
                        return true;
                }

                //If gotten this far, not going to be used
                return false;
            });

            //Process each of the stored entries for inclusion in the lookup
            Dictionary<MethodHash, PersistentMethod> typeLookup = new Dictionary<MethodHash, PersistentMethod>(baseMethods.Count);

            //Process all of the stored methods
            foreach (MethodInfo method in baseMethods) {
                //Get all of the parameters belonging to this method that needs to processed
                ParameterInfo[] parameters = method.GetParameters();

                //Create an array for all of the parameters within this method
                PersistentParameter[] paramsInfo = new PersistentParameter[parameters.Length];

                //Store the type signature for this method
                Type[] signature = new Type[parameters.Length];

                //Store the collection of objects that will be used to create this Methods Hash
                List<object> hashable = new List<object>(parameters.Length + 1);
                hashable.Add(method.Name);

                //Process each of the parameter signature values
                for (int i = 0; i < parameters.Length; i++) {
                    //Store the type for the entry
                    signature[i] = parameters[i].ParameterType;

                    //Add the type to the hashable list
                    hashable.Add(parameters[i].ParameterType);

                    //Look for a description attribute that is attached to this parameter
                    DescriptionAttribute paramDesc = parameters[i].GetFirstCustomAttributeOf<DescriptionAttribute>();

                    //Create the information object for this parameter
                    paramsInfo[i] = new PersistentParameter(
                        new GUIContent(
                            ObjectNames.NicifyVariableName(parameters[i].Name),
                            paramDesc != null ? paramDesc.Description : string.Empty
                        ),
                        parameters[i].GetFirstCustomAttributeOf<ParameterAttribute>(),
                        parameters[i].DefaultValue
                    );
                }

                //Look for a description attribute that is attached to this method
                DescriptionAttribute methodDesc = method.GetFirstCustomAttributeOf<DescriptionAttribute>();

                //Create the information object for this method
                PersistentMethod persistentMethod = new PersistentMethod(
                    new GUIContent(
                        GenerateLabelFromSignature(
                            (method.IsSpecialName ? 
                                method.Name.Substring(SETTER_PROPERTY_PREFIX.Length) :
                                method.Name
                            ),
                            signature
                        ),
                        methodDesc != null ? methodDesc.Description : string.Empty
                    ),
                    method.Name,
                    signature,
                    paramsInfo,
                    method.IsSpecialName
                );

                //Get the hash key for this method
                MethodHash hashKey = HashUtitlity.GetCombinedHash(hashable.ToArray());

                //Warn on a key conflict (Ideally, shouldn't happen ever)
                if (typeLookup.ContainsKey(hashKey))
                    Debug.LogWarningFormat("Generated hash key for the method '{0}' (For Type: {1}) is identical to the method '{2}'. This method will overrite the previous", persistentMethod.DisplayLabel.text, key.FullName, typeLookup[hashKey].DisplayLabel.text);

                //Add the method to the lookup
                typeLookup[hashKey] = persistentMethod;
            }

            //Stash the values in the general lookup
            objectMethods[key] = typeLookup;
        }

        //PUBLIC

        /// <summary>
        /// Retrieve all usable methods that belong to the supplied object type
        /// </summary>
        /// <param name="obj">The object that is to have its methods returned</param>
        /// <returns>Returns an array of the objects that describe the usable methods of the object</returns>
        public static PersistentMethod[] RetrieveObjectMethods(UnityEngine.Object obj) {
            //Get the type of the object to process
            RaiseableType key = obj.GetType();

            //Check if the key has already been cached
            if (!objectMethods.ContainsKey(key))
                ProcessObjectType(key);

            //Make a copy buffer for the methods that are retrieved
            PersistentMethod[] buffer = new PersistentMethod[objectMethods[key].Count];
            objectMethods[key].Values.CopyTo(buffer, 0);

            //Return the identified methods
            return buffer;
        }

        /// <summary>
        /// Retrieve the PersistentMethod description for the callback described by the callback
        /// </summary>
        /// <param name="callback">The callback object containing the information for the method to be raised</param>
        /// <returns>Returns the Persistent Method definition for the callback or null if not found</returns>
        public static PersistentMethod GetPersistentMethodFromCallback(PersistentCallback callback) {
            //Check that the callback is valid
            if (!callback.IsValid) return null;

            //Get the type of the target object
            Type key = callback.Target.GetType();

            //If the type hasn't been processed, get to it
            if (!objectMethods.ContainsKey(key))
                ProcessObjectType(key);

            //Create a hashable list for lookup
            List<object> hashable = new List<object>(callback.ParameterInfo.Length + 1);
            hashable.Add(callback.MethodName);

            //Add the parameter signature to the list
            foreach (var param in callback.ParameterInfo)
                hashable.Add(param.ParameterType);

            //Generate the hash key for this entry
            MethodHash hashKey = HashUtitlity.GetCombinedHash(hashable.ToArray());

            //Return the corresponding element if it exists
            return (objectMethods[key].ContainsKey(hashKey) ?
                objectMethods[key][hashKey] :
                null
            );
        }

        /// <summary>
        /// Retrieve the display label that can be used to describe the method stored in a callback
        /// </summary>
        /// <param name="callback">The callback object containing the information for the method to be raised</param>
        /// <returns>Returns a string describing the current method stored in the callback</returns>
        public static string GetPersistentMethodLabelFromCallback(PersistentCallback callback) {
            //Check that the callback is valid
            if (!callback.IsValid) return string.Empty;

            //Retrieve the method parameter signature
            int prog = 0;
            Type[] signature = new Type[callback.ParameterInfo.Length];
            foreach (var param in callback.ParameterInfo) {
                //Get the type from the object
                signature[prog] = param.ParameterType;

                //Check that the parameter type exists
                if (signature[prog] == null) {
                    Debug.LogErrorFormat("Invalid Parameter Type at index {0} in the Persistent Callback description for Method '{1}'", prog, callback.MethodName);
                    return "Invalid Persistent Callback";
                }

                //Increment progress through array
                ++prog;
            }

            //Return the generated label for the callback
            return GenerateLabelFromSignature(callback.MethodName, signature);
        }

        /// <summary>
        /// Create a display label for a method based on the supplied name and its signature
        /// </summary>
        /// <param name="methodName">The name of the method that is having a label generated</param>
        /// <param name="signature">The parameter signature for the method</param>
        /// <returns>Returns a formatted display label for the method</returns>
        public static string GenerateLabelFromSignature(string methodName, Type[] signature) {
            //Construct the display name of this method
            StringBuilder sb = new StringBuilder(methodName.StartsWith(SETTER_PROPERTY_PREFIX) ?
                methodName.Substring(SETTER_PROPERTY_PREFIX.Length) :
                methodName
            );

            //Check if there are parameters to tack on
            if (signature.Length > 0) {
                sb.Append(" ");
                sb.Append(GenerateSignatureLabelString(signature));
            }

            //Return the generated name
            return sb.ToString();
        }

        /// <summary>
        /// Generate a formatted string for signature type elements in a label
        /// </summary>
        /// <param name="signature">The type elements that make up the signature</param>
        /// <returns>Returns a formatted string for the received elements</returns>
        public static string GenerateSignatureLabelString(Type[] signature) {
            //Construct the string by iterating over the stored values
            StringBuilder sb = new StringBuilder("(");
            for (int i = 0; i < signature.Length; i++) {
                //Add the type name
                sb.Append(signature[i].Name);

                //Check for grammar
                if (i < signature.Length - 1)
                    sb.Append(", ");
            }

            //Finish the string
            sb.Append(")");
            return sb.ToString();
        }
    }

    /// <summary>
    /// Store information about a persistent method that can be selected for use 
    /// </summary>
    public sealed class PersistentMethod {
        /*----------Properties----------*/
        //PUBLIC
     
        /// <summary>
        /// Store the display label that is used when this method has been selected
        /// </summary>
        public GUIContent DisplayLabel { get; private set; }

        /// <summary>
        /// Store the name of the method that is to be invoked for this method
        /// </summary>
        public string MethodName { get; private set; }

        /// <summary>
        /// Store the types of the parameters that are used for this method
        /// </summary>
        public Type[] ParameterSignature { get; private set; }

        /// <summary>
        /// Store information about the parameters belonging to this method option
        /// </summary>
        public PersistentParameter[] Parameters { get; private set; }

        /// <summary>
        /// Flag if this method is a property that will be raised
        /// </summary>
        public bool IsProperty { get; private set; }

        /*----------Functions----------*/
        //PUBLIC
        
        /// <summary>
        /// Initialise this object with the information about the method that is represented by this object
        /// </summary>
        /// <param name="label">The label that will be displayed when this option is selected</param>
        /// <param name="method">The name of the method that should be raised when this option is selected</param>
        /// <param name="signature">The parameter type signature that is used to differentiate overloaded methods</param>
        /// <param name="parameters">A descriptive array breakdown of the parameters included within this method</param>
        /// <param name="isProperty">Flags if this method is a property being raised</param>
        public PersistentMethod(GUIContent label, string method, Type[] signature, PersistentParameter[] parameters, bool isProperty) {
            DisplayLabel = label;
            MethodName = method;
            ParameterSignature = signature;
            Parameters = parameters;
            IsProperty = isProperty;
        }
    }

    /// <summary>
    /// Store information about a parameter that exists as part of a persistent method
    /// </summary>
    public sealed class PersistentParameter {
        /*----------Properties----------*/
        //PUBLIC
        
        /// <summary>
        /// Store the display label that is used when this parameter is being displayed
        /// </summary>
        public GUIContent DisplayLabel { get; private set; }

        /// <summary>
        /// Store a reference to the custom Parameter Attribute that is attached to this parameter
        /// </summary>
        public ParameterAttribute Attribute { get; private set; }

        /// <summary>
        /// The default value of the parameter object (Or null if none)
        /// </summary>
        public object DefaultValue { get; private set; }

        /*----------Functions----------*/
        //PUBLIC
        
        /// <summary>
        /// Initialise this object with the information about the parameter that is represented by this object
        /// </summary>
        /// <param name="label">The label that will be displayed when this option is selected</param>
        /// <param name="attribute">The custom ParameterAttribute that was attached to this object for rendering</param>
        /// <param name="defaultValue">The default value that is to be used for this parameter value</param>
        public PersistentParameter(GUIContent label, ParameterAttribute attribute, object defaultValue) {
            DisplayLabel = label;
            Attribute = attribute;
            DefaultValue = defaultValue;
        }
    }
}