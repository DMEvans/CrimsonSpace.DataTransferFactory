namespace CrimsonSpace.DataTransferFactory
{
    internal class TransferMemberReport
    {
        /// <summary>
        /// The message
        /// </summary>
        private string message = string.Empty;

        /// <summary>
        /// Gets or sets the name of the member.
        /// </summary>
        /// <value>
        /// The name of the member.
        /// </value>
        public string MemberName { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public string Message
        {
            get { return string.Format(message, MemberName, TransferName); }
            set { message = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="TransferMemberReport"/> is success.
        /// </summary>
        /// <value>
        ///   <c>true</c> if success; otherwise, <c>false</c>.
        /// </value>
        public bool Success { get; set; }

        /// <summary>
        /// Gets or sets the name of the transfer.
        /// </summary>
        /// <value>
        /// The name of the transfer.
        /// </value>
        public string TransferName { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format("{0} - {1}", Success ? "Success" : "Failure", Message);
        }
    }
}