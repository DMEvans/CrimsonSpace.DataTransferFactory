namespace CrimsonSpace.DataTransferFactory
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public static class Extensions
    {
        public static T DeconstructDTO<T>(this ITransferDTO source) where T : class
        {
            var transferWorker = new TransferDeconstructor<T>(source);
            return transferWorker.Deconstruct();
        }

        public static T ConstructDTO<T>(this object source) where T : class, ITransferDTO
        {
            var transferWorker = new TransferConstructor<T>(source);
            return transferWorker.Construct();
        }

        public static IEnumerable<T> ConstructDTOCollection<T>(this IEnumerable<object> sourceCollection) where T : class, ITransferDTO
        {
            var returnList = new List<T>();

            foreach (var sourceObject in sourceCollection)
            {
                var transferWorker = new TransferConstructor<T>(sourceObject);
                returnList.Add(transferWorker.Construct());
            }

            return returnList;
        }

        public static IEnumerable<T> DeconstructDTOCollection<T>(this IEnumerable<ITransferDTO> sourceCollection) where T : class
        {
            var returnList = new List<T>();

            foreach (var sourceObject in sourceCollection)
            {
                var transferWorker = new TransferDeconstructor<T>(sourceObject);
                returnList.Add(transferWorker.Deconstruct());
            }

            return returnList;
        }

        internal static MethodInfo GetExtensionMethod(this Type type, string methodName)
        {
            return (from x in type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                    where x.IsDefined(typeof(System.Runtime.CompilerServices.ExtensionAttribute))
                    where x.Name == methodName
                    select x).FirstOrDefault();
        }
    }
}