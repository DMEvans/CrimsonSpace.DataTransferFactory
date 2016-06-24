namespace CrimsonSpace.DataTransferFactory
{
    using System;

    /// <summary>
    /// Inidicates that this property should be excluded when the class has the "TransferAllMembers" attribute
    /// </summary>
    /// <seealso cref="System.Attribute" />
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class NonTransferrableMemberAttribute : Attribute
    {
    }
}