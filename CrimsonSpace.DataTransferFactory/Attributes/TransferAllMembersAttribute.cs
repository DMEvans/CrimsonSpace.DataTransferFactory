namespace CrimsonSpace.DataTransferFactory
{
    using System;

    /// <summary>
    /// Indicates that all accessible members in the class will be transferred unless marked with the NonTransferrableMember attribute
    /// </summary>
    /// <seealso cref="System.Attribute" />
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class TransferAllMembersAttribute : Attribute
    {
    }
}