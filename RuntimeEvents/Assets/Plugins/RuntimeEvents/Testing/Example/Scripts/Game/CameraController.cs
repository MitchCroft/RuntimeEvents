using UnityEngine;

namespace RuntimeEvents.Testing.Example {
    /// <summary>
    /// Manage the position of the camera so that it follows after the target object
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class CameraController : MonoBehaviour {
        /*----------Variables----------*/
        //INVISIBLE

        /// <summary>
        /// Store a reference to the transform of the attached object
        /// </summary>
        private Transform trans;

        /// <summary>
        /// Store the initial offset from the target that is to be maintained
        /// </summary>
        private Vector3 offset;

        //VISIBLE

        [Header("Object References")]

        [SerializeField, Tooltip("Store a reference to the target object that is to be followed after")]
        private Transform target;

        [SerializeField, Tooltip("Store the movement scale that will be used by the camera to follow the target")]
        private float movementSpeed = 5f;

        /*----------Functions----------*/
        //PRIVATE

        /// <summary>
        /// Initialise this objects internal information
        /// <summary>
        private void Awake() {
            trans = transform;
            offset = trans.position - target.position;
        }

        /// <summary>
        /// Move this object to follow the target object
        /// </summary>
        private void LateUpdate() {
            //Get the desired position of this controller
            Vector3 desiredPos = target.position + offset;

            //Set the new position of this object
            trans.position = Vector3.Lerp(trans.position, desiredPos, Time.deltaTime * movementSpeed);
        }
    }
}