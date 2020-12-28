#if UNITY_EDITOR
namespace MitchCroft.SerializedData.EditorInspector.StorageObjects {
    /// <summary>
    /// Store values of the <see cref="bool"/> type so that they can be displayed with <see cref="Data"/> objects in the inspector
    /// </summary>
    [CustomSerialStorage(typeof(bool))]
    public sealed class BoolSerialStorage : SerialStorage<bool> {}
}
#endif