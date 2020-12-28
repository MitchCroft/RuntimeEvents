#if UNITY_EDITOR
namespace MitchCroft.SerializedData.EditorInspector.StorageObjects {
    /// <summary>
    /// Store values of the <see cref="int"/> type so that they can be displayed with <see cref="Data"/> objects in the inspector
    /// </summary>
    [CustomSerialStorage(typeof(int))]
    public sealed class IntSerialStorage : SerialStorage<int> {}
}
#endif