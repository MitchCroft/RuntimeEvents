#if UNITY_EDITOR
namespace MitchCroft.SerializedData.EditorInspector.StorageObjects {
    /// <summary>
    /// Store values of the <see cref="UnityEngine.Vector3"/> type so that they can be displayed with <see cref="Data"/> objects in the inspector
    /// </summary>
    [CustomSerialStorage(typeof(UnityEngine.Vector3))]
    public sealed class Vector3SerialStorage : SerialStorage<UnityEngine.Vector3> {}
}
#endif