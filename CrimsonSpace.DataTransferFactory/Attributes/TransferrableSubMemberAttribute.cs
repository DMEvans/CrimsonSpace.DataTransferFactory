namespace CrimsonSpace.DataTransferFactory
{
    using System;

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class TransferrableSubMemberAttribute : Attribute
    {
        private bool isList = false;

        public TransferrableSubMemberAttribute()
        {
            isList = false;
        }

        public TransferrableSubMemberAttribute(bool isList)
        {
            this.isList = isList;
        }

        public bool IsList => isList;
    }
}