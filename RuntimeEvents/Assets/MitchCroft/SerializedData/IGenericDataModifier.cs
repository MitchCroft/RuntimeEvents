namespace MitchCroft.SerializedData {
    /// <summary>
    /// An interface that can be used to apply custom actions to the way that generic data is handled
    /// </summary>
    /// <remarks>
    /// These references *aren't* serialised and will need to be re-assigned each time
    /// Primarily intended to allow for the modification of inspector display behaviour
    /// </remarks>
    public interface IGenericDataModifier {}
}