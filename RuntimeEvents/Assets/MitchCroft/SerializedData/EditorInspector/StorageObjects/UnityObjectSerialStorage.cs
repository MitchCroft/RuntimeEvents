#if UNITY_EDITOR
namespace MitchCroft.SerializedData.EditorInspector.StorageObjects {
    /// <summary>
    /// Store values of the <see cref="UnityEngine.Object"/> type so that they can be displayed with <see cref="Data"/> objects in the inspector
    /// </summary>
    [CustomSerialStorage(typeof(UnityEngine.Object), true)]
    public sealed class UnityObjectSerialStorage : SerialStorage<UnityEngine.Object> {
        /*----------Functions----------*/
        //PUBLIC

        /// <summary>
        /// Force a Unity fake 'null' value assigned to become a real null value
        /// </summary>
        /// <returns>Returns the internal value that is stored within this container</returns>
        public override object GetValue() {
            return (serialValue ?
                serialValue :
                null
            );
        }
    }
}
#endif