#if UNITY_EDITOR
using UnityEngine;

namespace MitchCroft.SerializedData.EditorInspector.StorageObjects {
    /// <summary>
    /// Store values of the <see cref="UnityEngine.Quaternion"/> type so that they can be displayed with <see cref="Data"/> objects in the inspector
    /// </summary>
    [CustomSerialStorage(typeof(UnityEngine.Quaternion))]
    public sealed class QuaternionSerialStorage : SerialStorage<UnityEngine.Vector3> {
        /*----------Functions----------*/
        //PUBLIC

        /// <summary>
        /// Convert the quaternion values to and from euler values for display within the inspector
        /// </summary>
        /// <param name="value">The quaternion value that is to be assigned to the storage</param>
        /// <returns>Returns true if the value could be assigned properly</returns>
        public override bool SetValue(object value) {
            // If the value is null then problem
            if (object.Equals(value, null))
                return false;
            else if (value.GetType() != typeof(Quaternion))
                return false;

            // Save the value for display
            serialValue = ((Quaternion)value).eulerAngles;
            return true;
        }

        /// <summary>
        /// Convert the stored euler values into a quaternion object
        /// </summary>
        /// <returns>Returns the quaternion representation of the euler values</returns>
        public override object GetValue() { return Quaternion.Euler(serialValue); }
    }
}
#endif