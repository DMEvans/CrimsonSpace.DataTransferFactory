namespace CrimsonSpace.DataTransferFactory
{
    using System.Reflection;

    internal class MemberCollection
    {
        internal PropertyInfo[] Properties { get; set; }
        internal FieldInfo[] Fields { get; set; }
    }
}