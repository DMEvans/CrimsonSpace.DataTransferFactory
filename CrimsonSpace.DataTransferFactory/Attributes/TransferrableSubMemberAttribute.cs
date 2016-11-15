namespace CrimsonSpace.DataTransferFactory
{
    using System;

    /// <summary>
    /// Indicates that the member class that needs to be transferred via the DTO methods
    /// </summary>
    /// <seealso cref="System.Attribute" />
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class TransferrableSubMemberAttribute : Attribute
    {
        /// <summary>
        /// Indicates whether the sub member is a list
        /// </summary>
        private bool isList = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="TransferrableSubMemberAttribute"/> class.
        /// </summary>
        public TransferrableSubMemberAttribute()
        {
            isList = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TransferrableSubMemberAttribute"/> class.
        /// </summary>
        /// <param name="isList">if set to <c>true</c> [is list].</param>
        public TransferrableSubMemberAttribute(bool isList)
        {
            this.isList = isList;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is a list.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is a list; otherwise, <c>false</c>.
        /// </value>
        public bool IsList => isList;
    }
}