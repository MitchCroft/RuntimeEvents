using System;
using System.Collections;

using UnityEngine;

namespace RuntimeEvents.Testing.Example {
    /// <summary>
    /// Manage the elements of the game to allow for the game to progress
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class GameController : MonoBehaviour {
        /*----------Types----------*/
        //PRIVATE

        /// <summary>
        /// Event used to relay the current playback time (in seconds)
        /// </summary>
        [Serializable] private sealed class TimeUpdatedEvt : RuntimeEvent<float> {}

        /*----------Variables----------*/
        //VISIBLE

        [Header("Object References")]

        [SerializeField, Tooltip("Store a reference to the player that is in the scene")]
        private PlayerController player;

        [Header("Game Settings")]

        [SerializeField, Tooltip("Flags if the game should automatically be started")]
        private bool autoStart;

        [SerializeField, Tooltip("The amount of time in between the collectibles respawning after being collected by the player")]
        private float collectibleRespawnTime = 5f;

        [SerializeField, Tooltip("Indicates the amount of time (in seconds) that the game should go for before the final score is calculated")]
        private float gameLength = 60;

        [Header("Events")]

        [SerializeField, Tooltip("Functions that are to be raised whenever the game time is updated")]
        private TimeUpdatedEvt onTimeUpdated;

        [SerializeField, Tooltip("Functions that are to be raised when the game is completed")]
        private RuntimeEvent onComplete;
        
        /*----------Functions----------*/
        //PRIVATE

        /// <summary>
        /// Initialise this object with additional information found in the scene
        /// </summary>
        private void Start() { player.CanMove = false; if (autoStart) StartGame(); }

        /// <summary>
        /// Co-routine that is used to track the elapse of time for gameplay purposes
        /// </summary>
        private IEnumerator ProcessGamePlay_CR() {
            //Store the amount of time elapsed
            float timer = 0f;

            //Loop while the game is progressing
            while (timer < gameLength) {
                yield return null;

                //Increment the game time
                timer += Time.deltaTime;

                //Raise the updated event
                onTimeUpdated.Invoke(Mathf.Min(timer, gameLength));
            }

            //Game is over
            EndGame();
        }

        /// <summary>
        /// Manage the toggling of collectible states in response to the player collecting them
        /// </summary>
        /// <param name="obj">The object that was collected by the player</param>
        private IEnumerator DelayCollectibleRespawn_CR(CollectibleObject obj) {
            obj.gameObject.SetActive(false);
            yield return new WaitForSeconds(collectibleRespawnTime);
            obj.gameObject.SetActive(true);
        }

        //PUBLIC

        /// <summary>
        /// Start the playback of the game 
        /// </summary>
        public void StartGame() {
            //Allow the player to start moving
            player.CanMove = true;

            //Start the playback tracking coroutine
            StartCoroutine(ProcessGamePlay_CR());
        }

        /// <summary>
        /// End the playback of the game
        /// </summary>
        public void EndGame() {
            //Stop all running coroutines that are in operation
            StopAllCoroutines();

            //Prevent the player from moving
            player.CanMove = false;

            //Raise the completed events
            onComplete.Invoke();
        }

        /// <summary>
        /// Start the process of 'respawning' a collectible object
        /// </summary>
        /// <param name="obj">The object that was collected by the player</param>
        /// <param name="score">The score of the collectible when it was collected</param>
        public void CollectedCollectible(CollectibleObject obj, int score) { StartCoroutine(DelayCollectibleRespawn_CR(obj)); }
    }
}