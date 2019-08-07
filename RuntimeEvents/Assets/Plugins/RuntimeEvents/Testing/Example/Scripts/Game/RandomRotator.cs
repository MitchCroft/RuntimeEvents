using UnityEngine;

namespace RuntimeEvents.Testing.Example {
    /// <summary>
    /// Apply random rotations to the transform to give some animation to the object
    /// </summary>
    public sealed class RandomRotator : MonoBehaviour {
        /*----------Variables----------*/
        //INVISIBLE

        /// <summary>
        /// Cache a reference to the attached transform
        /// </summary>
        private Transform trans;

        /// <summary>
        /// Store the rotation target that is desired by this object
        /// </summary>
        private Quaternion targetRot;

        /// <summary>
        /// Store a timer for the update operation
        /// </summary>
        private float timer;

        //VISIBLE

        [Header("Rotation Settings")]

        [SerializeField, Tooltip("The frequency (in seconds) with which the target rotation will be updated")]
        private float updateFrequency = .5f;

        [SerializeField, Range(0f, 1f), Tooltip("The strength that will be given to the generated rotations and their impact on the target")]
        private float generatedStrength = .2f;

        [SerializeField, Tooltip("The update strength that is applied when this object is rotating to target the rotation amount")]
        private float moveScale = 1f;

        /*----------Functions----------*/
        //PRIVATE

        /// <summary>
        /// Initialise this objects internal information
        /// <summary>
        private void Awake() { trans = transform; }

        /// <summary>
        /// Setup references that are required when this this object is enabled
        /// </summary>
        private void OnEnable() { targetRot = trans.rotation; timer = 0f; RandomiseTarget(); }

        /// <summary>
        /// Update the transform of the attached to match the desired point
        /// </summary>
        private void Update() {
            //Update the timer
            timer += Time.deltaTime;
            if (timer >= updateFrequency) {
                timer = 0f;
                RandomiseTarget();
            }

            //Rotate the transform towards the target
            trans.rotation = Quaternion.SlerpUnclamped(trans.rotation, targetRot, Time.deltaTime * moveScale);
        }

        /// <summary>
        /// Generate a new randomised rotation target for the current object
        /// </summary>
        private void RandomiseTarget() {
            //Get a random rotation 
            Quaternion ran = Random.rotationUniform;

            //Adjust the target by the strength
            targetRot = Quaternion.SlerpUnclamped(targetRot, ran, generatedStrength);
        }
    }
}