#if UNITY_EDITOR
namespace MitchCroft.SerializedData.EditorInspector.StorageObjects {
    /// <summary>
    /// Store values of the <see cref="UnityEngine.Vector4"/> type so that they can be displayed with <see cref="Data"/> objects in the inspector
    /// </summary>
    [CustomSerialStorage(typeof(UnityEngine.Vector4))]
    public sealed class Vector4SerialStorage : SerialStorage<UnityEngine.Vector4> {}
}
#endif