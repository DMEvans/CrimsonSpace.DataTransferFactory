namespace CrimsonSpace.DataTransferFactory
{
    using System;

    /// <summary>
    /// Incompatible types exception
    /// </summary>
    /// <seealso cref="System.Exception" />
    public class IncompatibleTypesException : Exception
    {
        /// <summary>
        /// Gets a message that describes the current exception.
        /// </summary>
        public override string Message => "The supplied types are not compatable for data transfer.  Check the usage of the TransferrableTypes attribute.";
    }
}