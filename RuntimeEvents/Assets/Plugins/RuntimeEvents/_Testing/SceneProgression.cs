using System.Collections;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace RuntimeEvents.Testing {
    /// <summary>
    /// Provide an interface for progressing the scene
    /// </summary>
    public sealed class SceneProgression : MonoBehaviour {
        /*----------Variables----------*/
        //INVISIBLE

        /// <summary>
        /// Flag if a loading operation is underway to prevent multiple
        /// </summary>
        private bool isLoading;

        /*----------Functions----------*/
        //PRIVATE

        /// <summary>
        /// Wait for the supplied period of time before allowing the scene to load
        /// </summary>
        /// <param name="oper">The operation to control</param>
        /// <param name="time">The time (in seconds) to be waited before the operation is allowed to finish</param>
        private IEnumerator WaitToLoad_CR(AsyncOperation oper, float time) {
            //Lock the operation from finishing
            oper.allowSceneActivation = false;

            //Wait for the time period
            yield return new WaitForSeconds(time);

            //Allow the scene to activate
            oper.allowSceneActivation = true;
        }

        //PUBLIC

        /// <summary>
        /// Start the loading of the next scene
        /// </summary>
        /// <param name="loadDelay">The time (in seconds) to be waited before the operation is allowed to finish</param>
        public void LoadNextScene(float loadDelay = 0f) {
            //Prevent loading of multiple scenes
            if (isLoading) return;

            //Start the next load
            AsyncOperation oper = SceneManager.LoadSceneAsync((SceneManager.GetActiveScene().buildIndex + 1) % SceneManager.sceneCountInBuildSettings);

            //Check if the scene should be delayed
            if (loadDelay > 0f) StartCoroutine(WaitToLoad_CR(oper, loadDelay));

            //Flag the operation as started
            isLoading = true;
        }

        /// <summary>
        /// Start a reload of the current scene
        /// </summary>
        /// <param name="loadDelay">The time (in seconds) to be waited before the operation is allowed to finish</param>
        public void ReloadCurrentScene(float loadDelay = 0f) {
            //Prevent loading of multiple scenes
            if (isLoading) return;

            //Start the next load
            AsyncOperation oper = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);

            //Check if the scene should be delayed
            if (loadDelay > 0f) StartCoroutine(WaitToLoad_CR(oper, loadDelay));

            //Flag the operation as started
            isLoading = true;
        }
    }
}