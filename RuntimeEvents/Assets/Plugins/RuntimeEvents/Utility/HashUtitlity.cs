namespace RuntimeEvents {
    /// <summary>
    /// Provide additional functionality for generating Hash IDs for groups of values
    /// </summary>
    public static class HashUtitlity {
        /*----------Functions----------*/
        //PUBLIC

        /// <summary>
        /// Perform a simple combination operation to generate a Hash Code for a group of values
        /// </summary>
        /// <param name="values">The array of objects to be used for the hash generation</param>
        /// <returns>Returns a hash ID to represent the combined values</returns>
        /// <remarks>The combine operation is taken from https://stackoverflow.com/a/1646913 </remarks>
        public static int GetCombinedHash(params object[] values) {
            unchecked {
                int hash = 17;
                for (int i = 0; i < values.Length; i++)
                    hash = hash * 31 + (values[i] != null ? values[i].GetHashCode() : 0);
                return hash;
            }
        }
    }
}