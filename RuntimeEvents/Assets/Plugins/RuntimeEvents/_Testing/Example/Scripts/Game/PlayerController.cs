using System;

using UnityEngine;

namespace RuntimeEvents.Testing.Example {
    /// <summary>
    /// Handle the movement of the player object within the scene through physics forces
    /// </summary>
    [DisallowMultipleComponent, RequireComponent(typeof(Rigidbody))]
    public sealed class PlayerController : MonoBehaviour {
        /*----------Variables----------*/
        //INVISIBLE

        /// <summary>
        /// Store a reference to the attached rigid body to apply movement
        /// </summary>
        private new Rigidbody rigidbody;

        /// <summary>
        /// Store a reference to the gyroscope to use instead of keyboard input
        /// </summary>
        private Gyroscope gyro;

        //VISIBLE

        [Header("Movement Settings")]

        [SerializeField, Tooltip("The strength that will be used when applying force in the movement directions")]
        private float movementSpeed = 100f;

        /*----------Properties----------*/
        //PUBLIC

        /// <summary>
        /// Toggle the ability for the player to move
        /// </summary>
        public bool CanMove {
            get { return !rigidbody.isKinematic; }
            set { rigidbody.isKinematic = !value; }
        }

        /*----------Functions----------*/
        //PRIVATE

        /// <summary>
        /// Initialise this objects internal information
        /// <summary>
        private void Awake() {
            //Grab the rigidbody reference
            rigidbody = GetComponent<Rigidbody>();

            //Check if a gyroscope is usable
            if (SystemInfo.supportsGyroscope)
                gyro = Input.gyro;
        }

        /// <summary>
        /// Handle input values to allow for the player to move
        /// </summary>
        private void FixedUpdate() {
            //Check that movement is allowed
            if (!CanMove) return;

            //Get the strength values to use as force appliers
            float horizontalForce, verticalForce;

            //If there is no gyroscope, use the default keyboard inputs
            if (gyro == null) {
                horizontalForce = Input.GetAxis("Horizontal");
                verticalForce = Input.GetAxis("Vertical");
            } 
            
            //Otherwise, grab the gyro input values
            else {
                horizontalForce = gyro.userAcceleration.x;
                verticalForce = gyro.userAcceleration.y;
            }

            //Apply the force the rigidbody
            rigidbody.AddForce(new Vector3(horizontalForce, 0f, verticalForce) * movementSpeed * Time.fixedDeltaTime);
        }
    }
}