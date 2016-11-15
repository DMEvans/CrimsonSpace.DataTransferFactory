namespace CrimsonSpace.DataTransferFactory
{
    using System;

    /// <summary>
    /// Indicates that this property should be excluded when the class has the "TransferAllMembers" attribute
    /// </summary>
    /// <seealso cref="System.Attribute" />
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class NonTransferrableMemberAttribute : Attribute
    {
    }
}