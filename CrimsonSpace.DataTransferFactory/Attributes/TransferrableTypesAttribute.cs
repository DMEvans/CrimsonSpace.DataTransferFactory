namespace CrimsonSpace.DataTransferFactory
{
    using System;

    /// <summary>
    /// Indicates the types that are permitted/compatible for transfers with this class
    /// </summary>
    /// <seealso cref="System.Attribute" />
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class TransferrableTypesAttribute : Attribute
    {
        /// <summary>
        /// The types
        /// </summary>
        private Type[] types = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="TransferrableTypesAttribute"/> class.
        /// </summary>
        /// <param name="types">The types.</param>
        public TransferrableTypesAttribute(params Type[] types)
        {
            if (types == null || types.Length == 0)
            {
                this.types = new Type[] { typeof(object) };
            }
            else
            {
                this.types = types;
            }
        }

        /// <summary>
        /// Gets the types.
        /// </summary>
        /// <value>
        /// The types.
        /// </value>
        public Type[] Types => types;
    }
}