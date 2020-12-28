#if UNITY_EDITOR
namespace MitchCroft.SerializedData.EditorInspector.StorageObjects {
    /// <summary>
    /// Store values of the <see cref="string"/> type so that they can be displayed with <see cref="Data"/> objects in the inspector
    /// </summary>
    [CustomSerialStorage(typeof(string))]
    public sealed class StringSerialStorage : SerialStorage<string> {}
}
#endif