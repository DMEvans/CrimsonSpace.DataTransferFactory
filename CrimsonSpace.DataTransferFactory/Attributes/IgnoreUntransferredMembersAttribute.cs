namespace CrimsonSpace.DataTransferFactory
{
    using System;

    /// <summary>
    /// Inidcates whether an exception will be thrown for transferrable members that fail to be transferred
    /// </summary>
    /// <seealso cref="System.Attribute" />
    public class IgnoreUntransferredMembersAttribute : Attribute
    {
    }
}