using UnityEngine;
using UnityEditor;

namespace RuntimeEvents.Testing.Example {
    /// <summary>
    /// Manage the displaying of the Game Controllers values
    /// </summary>
    [CustomEditor(typeof(GameController))]
    public sealed class GameControllerDrawer : Editor {
        /*----------Variables----------*/
        //CONST

        /// <summary>
        /// Store a constant value for 2xPI
        /// </summary>
        private const float TWOPI = Mathf.PI * 2f;

        //VISIBLE

        /// <summary>
        /// Store the prefab that will be instantiated in a circle with the specified settings
        /// </summary>
        private GameObject circularPrefab;

        /// <summary>
        /// Store the frequency with which the prefabs will be instantiated
        /// </summary>
        private float frequency = .1f;

        /// <summary>
        /// The radius from world origin that the prefabs will be instantiated at
        /// </summary>
        private float radius = 5f;

        /// <summary>
        /// The height above world origin the prefabs will be instantiated
        /// </summary>
        private float height = .5f;
        
        /*----------Functions----------*/
        //PUBLIC

        /// <summary>
        /// Manage the displaying of additional options within the inspector
        /// </summary>
        public override void OnInspectorGUI() {
            //Draw the default UI
            DrawDefaultInspector();

            //Provide additional options in editor mode
            using (GUILocker.PushSegment(!Application.isPlaying)) {
                //Display the additional options
                GUILayout.Space(20f);
                EditorGUILayout.LabelField("Instantiation Settings", EditorStyles.boldLabel);
                circularPrefab = (GameObject)EditorGUILayout.ObjectField("Circular Prefab", circularPrefab, typeof(GameObject), true);
                frequency = Mathf.Max(float.Epsilon, EditorGUILayout.FloatField("Frequency", frequency));
                radius = EditorGUILayout.FloatField("Radius", radius);
                height = EditorGUILayout.FloatField("Height", height);

                //Present buttons to allow for the instantiation of elements
                using (GUILocker.PushSegment(circularPrefab)) {
                    if (GUILayout.Button("Instantiate Objects")) {
                        //Get the incremental amount
                        float incer = TWOPI * frequency;

                        //Loop through and create the instances
                        for (float prog = 0f; prog < TWOPI; prog += incer) {
                            //Instantiate the new object
                            GameObject obj = Instantiate(circularPrefab);

                            //Position the object as required
                            obj.transform.position = new Vector3(
                                Mathf.Sin(prog) * radius,
                                height,
                                Mathf.Cos(prog) * radius
                            );

                            //Record the object has been created
                            Undo.RegisterCreatedObjectUndo(obj, obj.name);
                        }
                    }
                }
            }
        }
    }
}