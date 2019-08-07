using System;

using UnityEngine;

namespace RuntimeEvents.Testing.Example {
    /// <summary>
    /// Raise callback events in response to a collision event
    /// </summary>
    [DisallowMultipleComponent, RequireComponent(typeof(Collider), typeof(Renderer))]
    public sealed class CollectibleObject : MonoBehaviour {
        /*----------Types----------*/
        //PRIVATE

        /// <summary>
        /// Simple event that can be used to relay this collectible and its score to the desired listeners
        /// </summary>
        [Serializable] private sealed class CollectibleEvent : RuntimeEvent<CollectibleObject, int> {}

        /*----------Variables----------*/
        //INVISIBLE

        /// <summary>
        /// Store a reference to the material that is attached to this object
        /// </summary>
        private Material material;

        //VISIBLE

        [Header("Game Settings")]

        [SerializeField, Tooltip("The value of this collectible when collected")]
        private int score;

        [SerializeField, Tooltip("The minimum score value that this object can be assigned")]
        private int minScore = 100;

        [SerializeField, Tooltip("The maximum score value that this object can be assigned")]
        private int maxScore = 500;

        [SerializeField, Tooltip("A normalised gradient used to colour this object based on the generated score worth")]
        private Gradient scoreGradient;

        [Header("Collision Restrictions")]

        [SerializeField, Tooltip("If defined, the tag that must be on the collided object to raise the events")]
        private string requiredTag = "Player";

        [Header("Events")]

        [SerializeField, Tooltip("Events that are raised when this object comes in contact with a collision element")]
        private CollectibleEvent onTriggerEnter;

        /*----------Functions----------*/
        //PRIVATE

        /// <summary>
        /// Initialise this object with the default values
        /// </summary>
        private void Awake() { material = GetComponent<Renderer>().material; }

        /// <summary>
        /// When this object is enabled, generate a score that can be used
        /// </summary>
        private void OnEnable() {
            //Generate the score value that this collectible will be worth
            score = UnityEngine.Random.Range(minScore, maxScore);

            //Set the material colour based on the value
            material.color = scoreGradient.Evaluate(Mathf.Clamp01((float)(score - minScore) / (maxScore - minScore)));
        }

        /// <summary>
        /// Raise the trigger enter functionality when colliding with the correct object
        /// </summary>
        /// <param name="other">The object that was collided with in this interaction</param>
        private void OnTriggerEnter(Collider other) {
            //Check if the tag needs checking
            if (!string.IsNullOrEmpty(requiredTag) && requiredTag != other.tag)
                return;

            //Raise the collision events
            onTriggerEnter.Invoke(this, score);
        }

        /// <summary>
        /// Ensure that the inspector values are valid
        /// </summary>
        private void OnValidate() {
            maxScore = Mathf.Max(maxScore, minScore);
            minScore = Mathf.Min(maxScore, minScore);
        }
    }
}