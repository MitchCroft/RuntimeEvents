using UnityEngine;
using UnityEngine.UI;

namespace RuntimeEvents.Testing.Example {
    /// <summary>
    /// Manage the UI elements that are used to display information to the user
    /// </summary>
    public sealed class UIManager : MonoBehaviour {
        /*----------Variables----------*/
        //INVISIBLE

        /// <summary>
        /// Store a total score achieved so that the the label can be updated easily
        /// </summary>
        private int score;

        //VISIBLE

        [Header("Object References")]

        [SerializeField, Tooltip("Reference to the UI object that is used to display the current time value")]
        private Text timeDisplay;

        [SerializeField, Tooltip("Reference to the UI object that is used to display the current score value")]
        private Text scoreDisplay;

        [Header("Formatting Options")]

        [SerializeField, Tooltip("The formatting that will be used when displaying the time elapsed time. Minutes is passed into {0} and seconds to {0}")]
        private string timeFormatting = "{0:00}:{1:00}";

        [SerializeField, Tooltip("The formatting that will be used when displaying the current score. Score is passed into {0}")]
        private string scoreFormatting = "Score: {0}";

        /*----------Functions----------*/
        //PRIVATE

        /// <summary>
        /// Hide the starting UI elements 
        /// </summary>
        private void Awake() {
            timeDisplay.enabled =
            scoreDisplay.enabled = false;
        }

        //PUBLIC

        /// <summary>
        /// Ready the display elements to show required information
        /// </summary>
        public void ReadyForPlay() {
            //Enable the display components
            timeDisplay.enabled =
            scoreDisplay.enabled = true;

            //Reset the text stored within them
            timeDisplay.text = string.Format(timeFormatting, 0, 0);
            scoreDisplay.text = string.Format(scoreFormatting, 0);
        }

        /// <summary>
        /// Update the display element to show the current elapsed time
        /// </summary>
        /// <param name="elapsedSeconds">The total time (in seconds) that has elapsed to be displayed</param>
        public void UpdateTimeDisplay(float elapsed) {
            //Retrieve the total minutes from the counter
            float minutes = Mathf.Floor(elapsed / 60f);
            float seconds = Mathf.Round(elapsed % 60f);

            //Display the time on the display label
            timeDisplay.text = string.Format(timeFormatting, minutes, seconds);
        }

        /// <summary>
        /// Handle the updating of the displayed score based on the collected objects
        /// </summary>
        /// <param name="obj">The object that was collected by the player</param>
        /// <param name="score">The value of the collectible at the time of collection</param>
        public void UpdateDisplayScore(CollectibleObject obj, int score) {
            //Increment the score value
            this.score += score;

            //Update the display label
            scoreDisplay.text = string.Format(scoreFormatting, this.score);
        }
    }
}