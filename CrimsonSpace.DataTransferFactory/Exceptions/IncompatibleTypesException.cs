namespace CrimsonSpace.DataTransferFactory
{
    using System;

    public class IncompatibleTypesException : Exception
    {
        public override string Message => "The supplied types are not compatable for data transfer.  Check the usage of the TransferrableTypes attribute.";
    }
}