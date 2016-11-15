namespace CrimsonSpace.DataTransferFactory
{
    using System.Reflection;

    /// <summary>
    /// Container object for the properties and fields belonging to a class
    /// </summary>
    internal class MemberCollection
    {
        /// <summary>
        /// Gets or sets the properties.
        /// </summary>
        /// <value>
        /// The properties.
        /// </value>
        internal PropertyInfo[] Properties { get; set; }

        /// <summary>
        /// Gets or sets the fields.
        /// </summary>
        /// <value>
        /// The fields.
        /// </value>
        internal FieldInfo[] Fields { get; set; }
    }
}