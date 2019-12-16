using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace RuntimeEvents.Testing.Example {
    /// <summary>
    /// A basic UI button object that uses an RuntimeEvent object instead of a UnityEvent
    /// </summary>
    public class RuntimeButton : Selectable, IPointerClickHandler, ISubmitHandler, IEventSystemHandler {
        /*----------Variables----------*/
        //VISIBLE

        [SerializeField, Tooltip("Functions that are raised when this button is raised")]
        private RuntimeEvent onClick;

        /*----------Properties----------*/
        //PUBLIC

        /// <summary>
        /// Manage subscriptions to the OnClick callback
        /// </summary>
        public event RuntimeAction OnClick {
            add { onClick.Listeners += value; }
            remove { onClick.Listeners -= value; }
        }

        /*----------Functions----------*/
        //PUBLIC

        /// <summary>
        /// Manage click callback functionality based on pointer interaction
        /// </summary>
        /// <param name="eventData">Event data about the pointer interaction</param>
        public virtual void OnPointerClick(PointerEventData eventData) { OnSubmit(eventData); }

        /// <summary>
        /// Manage click callback functionality based on submission events
        /// </summary>
        /// <param name="eventData">Event data about the interaction that occurred</param>
        public virtual void OnSubmit(BaseEventData eventData) { onClick.Invoke(); }
    }
}