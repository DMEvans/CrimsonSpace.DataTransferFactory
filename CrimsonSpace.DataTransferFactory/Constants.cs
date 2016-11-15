namespace CrimsonSpace.DataTransferFactory
{
    /// <summary>
    /// The constants
    /// </summary>
    internal static class Constants
    {
        /// <summary>
        /// String with message for when no matching member can be found
        /// </summary>
        internal const string NoMatchingMemberFound = "No source member could be found named '{1}' for target member '{0}'";

        /// <summary>
        /// String with message for when the transfer is completed
        /// </summary>
        internal const string TransferCompleted = "Transfer of target member '{0}' from source member '{1}' completed";

        /// <summary>
        /// String with message for when the transfer is completed
        /// </summary>
        internal const string ValueWasNull = "Transfer of target member '{0}' from source member '{1}' not completed because value was null";

        /// <summary>
        /// String with message for when the transfer is completed
        /// </summary>
        internal const string TransferSkipped = "Transfer of target member '{0}' from source member '{1}' not skipped because the transfer type";
    }
}