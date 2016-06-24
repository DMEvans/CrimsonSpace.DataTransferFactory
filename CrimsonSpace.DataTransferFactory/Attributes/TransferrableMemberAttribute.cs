namespace CrimsonSpace.DataTransferFactory
{
    using System;

    /// <summary>
    /// Indicates that this property will be included when transferring to or from a DTO.
    /// This is redundant if the class has the "TransferAllMembers" attribute
    /// </summary>
    /// <seealso cref="System.Attribute" />
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class TransferrableMemberAttribute : Attribute
    {
    }
}