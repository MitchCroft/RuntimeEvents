using System;
using System.IO;
using System.Text;
using System.Diagnostics;

using UnityEngine;
using UnityEngine.Events;

using Debug = UnityEngine.Debug;

namespace RuntimeEvents.Testing.Benchmarking {
    /// <summary>
    /// Run comparison tests, comparing the operational speed of a Runtime Event and a Unity Event
    /// </summary>
    public sealed class TestRunner : MonoBehaviour {
        /*----------Types----------*/
        //PRIVATE

        /// <summary>
        /// Store a collection of the two event types that can be executed to test operation performance
        /// </summary>
        [Serializable] private sealed class CombinationEvent {
            [Tooltip("Flags if this event should be run")]
            public bool runEvents = true;

            [Tooltip("Standard Unity Event")]
            public UnityEvent unityEvent;

            [Tooltip("Runtime Event")]
            public RuntimeEvent runtimeEvent;
        }

        /// <summary>
        /// Create serialisable formats for the events that can be raised
        /// </summary>
        [Serializable] private sealed class BoolUnityEvent : UnityEvent<bool> {}
        [Serializable] private sealed class BoolRuntimeEvent : RuntimeEvent<bool> {}

        /// <summary>
        /// Store a collection of the two event types using a boolean parameter as the parameter
        /// </summary>
        [Serializable] private sealed class CombinationBoolEvent {
            [Tooltip("Flags if this event should be run")]
            public bool runEvents = true;

            [Tooltip("Standard Unity Event")]
            public BoolUnityEvent unityEvent;

            [Tooltip("Runtime Event")]
            public BoolRuntimeEvent runtimeEvent;
        }

        /// <summary>
        /// Store a pair of values of the same type
        /// </summary>
        /// <typeparam name="T">The type of value to be stored within this type</typeparam>
        private struct Pair<T> {
            /// <summary>
            /// Store the component values of the pair object
            /// </summary>
            public T first,
                     second;

            /// <summary>
            /// Initialise this pair with the same value
            /// </summary>
            /// <param name="val">The value to be assigned to both components</param>
            public Pair(T val) {
                this.first = 
                this.second = val;
            }

            /// <summary>
            /// Initialise this pair with separate values
            /// </summary>
            /// <param name="first">The value to be assigned to the first component</param>
            /// <param name="second">The value to be assigned to the second component</param>
            public Pair(T first, T second) {
                this.first = first;
                this.second = second;
            }
        }

        /*----------Variables----------*/
        //VISIBLE

        [Header("Test Settings")]

        [SerializeField, Tooltip("The number of times each test will be run")]
        private int testRunCount = 10000;

        [SerializeField, Tooltip("The location that the generated CSV result files should be deposited. This is combined with the persistent data path")]
        private string outputLocation;

        [Header("Test 1")]

        [SerializeField, Tooltip("Single, identical function for both")]
        private CombinationEvent test1a;    

        [SerializeField, Tooltip("Single, same functions but runtime has a non-void return type")]
        private CombinationEvent test1b;    

        [SerializeField, Tooltip("Equal amounts of the same function for both event types")]
        private CombinationEvent test1c;

        [SerializeField, Tooltip("Single empty functions for both, Runtime Event has multiple parameters supplied")]
        private CombinationEvent test1d;

        [SerializeField, Tooltip("Single, identical function for both with an added listener delegate attached to both")]
        private CombinationEvent test1e;

        [SerializeField, Tooltip("Single, identical persistent callback function that does not match event delegate signature")]
        private CombinationEvent test1f;

        [SerializeField, Tooltip("Single empty function to compare static vs non-static")]
        private CombinationEvent test1g;

        [Header("Test 2")]

        [SerializeField, Tooltip("Single, identical, dynamic function for both")]
        private CombinationBoolEvent test2a;

        [SerializeField, Tooltip("Single, same dynamic functions but runtime has a non-void return type")]
        private CombinationBoolEvent test2b;

        [SerializeField, Tooltip("Equal amounts of the same dynamic function for both event types")]
        private CombinationBoolEvent test2c;

        [SerializeField, Tooltip("Single, identical dynamic function for both with an added listener attached to both")]
        private CombinationBoolEvent test2d;

        [Header("Events")]

        [SerializeField, Tooltip("Functions that will be raised once the tests have been completed")]
        private RuntimeEvent onComplete;

        /*----------Functions----------*/
        //PRIVATE

        /// <summary>
        /// Initialise this objects internal information
        /// <summary>
        private void Start() {
            if (test1a.runEvents) BenchmarkCombinationEvent("Test 1A", test1a);
            if (test1b.runEvents) BenchmarkCombinationEvent("Test 1B", test1b);
            if (test1c.runEvents) BenchmarkCombinationEvent("Test 1C", test1c);
            if (test1d.runEvents) BenchmarkCombinationEvent("Test 1D", test1d);
            if (test1e.runEvents) {
                test1e.unityEvent.AddListener(() => { });
                test1e.runtimeEvent.AddListener(() => { });
                BenchmarkCombinationEvent("Test 1E", test1e);
            }
            if (test1f.runEvents) BenchmarkCombinationEvent("Test 1F", test1f);
            if (test1g.runEvents) BenchmarkCombinationEvent("Test 1G", test1g);

            if (test2a.runEvents) BenchmarkBoolCombinationEvent("Test 2A", test2a);
            if (test2b.runEvents) BenchmarkBoolCombinationEvent("Test 2B", test2b);
            if (test2c.runEvents) BenchmarkBoolCombinationEvent("Test 2C", test2c);
            if (test2d.runEvents) {
                test2d.unityEvent.AddListener(x => { });
                test2d.runtimeEvent.AddListener(x => { });
                BenchmarkBoolCombinationEvent("Test 2D", test2d);
            }
            Debug.Log("Tests Complete", this);
            onComplete.Invoke();
        }

        /// <summary>
        /// Ensure that the inspector settings are valid
        /// </summary>
        private void OnValidate() { testRunCount = Mathf.Max(1, testRunCount); }

        /// <summary>
        /// Run and record the execution times required for the assigned events
        /// </summary>
        /// <param name="label">The label to be reported for this benchmark</param>
        /// <param name="evt">The event object that is to be raised</param>
        private void BenchmarkCombinationEvent(string label, CombinationEvent evt) {
            //Create a stopwatch to record the time
            Stopwatch stopwatch = new Stopwatch();

            //Create a collection of times for the two events
            Pair<TimeSpan>[] results = new Pair<TimeSpan>[testRunCount];

            //Run the unity event first
            for (int i = 0; i < testRunCount; i++) {
                //Start counting
                stopwatch.Reset();
                stopwatch.Start();

                //Raise the event(s)
                evt.unityEvent.Invoke();

                //Record the time taken
                stopwatch.Stop();
                results[i].first = stopwatch.Elapsed;
            }

            //Run the runtime events
            for (int i = 0; i < testRunCount; i++) {
                //Start counting
                stopwatch.Reset();
                stopwatch.Start();

                //Raise the event(s)
                evt.runtimeEvent.Invoke();

                //Record the time taken
                stopwatch.Stop();
                results[i].second = stopwatch.Elapsed;
            }

            //Output the results of the test
            ExportResultsCSV(label, results);
        }

        /// <summary>
        /// Run and record the execution times required for the assigned events
        /// </summary>
        /// <param name="label">The label to be reported for this benchmark</param>
        /// <param name="evt">The event object that is to be raised</param>
        private void BenchmarkBoolCombinationEvent(string label, CombinationBoolEvent evt) {
            //Create a stopwatch to record the time
            Stopwatch stopwatch = new Stopwatch();

            //Create a collection of times for the two events
            Pair<TimeSpan>[] results = new Pair<TimeSpan>[testRunCount];

            //Store a bool that can be flipped for the testing
            bool state = true;

            //Run the unity event first
            for (int i = 0; i < testRunCount; i++) {
                //Start counting
                stopwatch.Reset();
                stopwatch.Start();

                //Raise the event(s)
                evt.unityEvent.Invoke(state);

                //Record the time taken
                stopwatch.Stop();
                results[i].first = stopwatch.Elapsed;

                //Invert the state flag
                state = !state;
            }

            //Reset the state
            state = true;

            //Run the runtime events
            for (int i = 0; i < testRunCount; i++) {
                //Start counting
                stopwatch.Reset();
                stopwatch.Start();

                //Raise the event(s)
                evt.runtimeEvent.Invoke(state);

                //Record the time taken
                stopwatch.Stop();
                results[i].second = stopwatch.Elapsed;

                //Invert the state flag
                state = !state;
            }

            //Output the results of the test
            ExportResultsCSV(label, results);
        }

        /// <summary>
        /// Export the received results to a .csv file that can be processed in an external file
        /// </summary>
        /// <param name="label">The name that is to be assigned to the exported file</param>
        /// <param name="results">A pair of the time execution results with the first being Unity Event and the second Runtime Event</param>
        private void ExportResultsCSV(string label, Pair<TimeSpan>[] results) {
            //Construct a string of the results
            StringBuilder csv = new StringBuilder();

            //Calculate the average time across all tests
            Pair<long> average = new Pair<long>();

            //Loop through and append all results to the string
            for (int i = 0; i < results.Length; i++) {
                //Append this entry
                csv.AppendLine(string.Format("{0};{1}", results[i].first, results[i].second));

                //Calculate the average time for both events
                average.first += results[i].first.Ticks;
                average.second += results[i].second.Ticks;
            }

            //Append on the header information
            csv.Insert(0, string.Format("Unity Event;Runtime Event;;{0};{1}\n", new TimeSpan((long)((double)average.first / results.Length)), new TimeSpan((long)((double)average.second / results.Length))));

            //Output the results
            try {
                string path = Path.Combine(Application.persistentDataPath, outputLocation);
                if (!string.IsNullOrEmpty(path)) Directory.CreateDirectory(path);
                File.WriteAllText(Path.Combine(path, label + ".csv"), csv.ToString());
                Debug.LogFormat("Saved Test Results for '{0}' to '{1}'", label, Path.Combine(path, label + ".csv"));
            } catch (Exception exec) {
                Debug.LogError("Failed to export CSV for test " + label + ". ERROR: " + exec.Message, this);
            }
        }

        //PUBLIC

        /// <summary>
        /// An empty function to test the base raising cost of a function
        /// </summary>
        public void EmptyFunction() {}

        /// <summary>
        /// An empty function to test the base raising cost of a function with a non-void return type
        /// </summary>
        public bool EmptyBoolFunction() { return false; }

        /// <summary>
        /// An empty function with multiple parameters to test the cost of raising with multiple parameters
        /// </summary>
        public void MultiParamEmptyFunction(int one, float two, bool three, string four) {}

        /// <summary>
        /// Empty function with a single string value to compare the cost of persistent callback functions
        /// </summary>
        public void PersistentStringCallback(string text) {}

        /// <summary>
        /// Empty static function to compare the cost of raising a static function compared to an instanced one
        /// </summary>
        public static void StaticEmptyFunction() {}

        /// <summary>
        /// An empty function to cost the base raising cost of a dynamic function
        /// </summary>
        public void EmptyDynamicFunction(bool state) {}

        /// <summary>
        /// An empty function to test the base raising cost of a function with a non-void return type
        /// </summary>
        public bool EmptyBoolDynamicFunction(bool state) { return state; }
    }
}