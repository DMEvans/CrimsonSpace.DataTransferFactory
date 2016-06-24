namespace CrimsonSpace.DataTransferFactory
{
    using System;

    /// <summary>
    /// Specifies an alternative member name in the corresponding transfer type from which to obtain the value
    /// If multiple names are specified, the first matching name will be used and the remainder ignored
    /// </summary>
    /// <seealso cref="System.Attribute" />
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
    public class TransferMemberNameAttribute : Attribute
    {
        /// <summary>
        /// The property name
        /// </summary>
        private string propertyName = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="TransferMemberNameAttribute"/> class.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        public TransferMemberNameAttribute(string propertyName)
        {
            this.propertyName = propertyName;
        }

        /// <summary>
        /// Gets the name of the property.
        /// </summary>
        /// <value>
        /// The name of the property.
        /// </value>
        public string PropertyName => propertyName;
    }
}