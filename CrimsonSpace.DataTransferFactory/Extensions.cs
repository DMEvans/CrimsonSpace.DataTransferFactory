namespace CrimsonSpace.DataTransferFactory
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public static class Extensions
    {
        public static T DeconstructDTO<T>(this ITransferDTO source, int allowedSubLevels = 1) where T : class
        {
            var transferWorker = new TransferDeconstructor<T>(source, allowedSubLevels);
            return transferWorker.Deconstruct();
        }

        internal static T DeconstructSubDTO<T>(this ITransferDTO source, Type parentType, int allowedSubLevels, int currentSubLevel) where T : class
        {
            var transferWorker = new TransferDeconstructor<T>(source, parentType, allowedSubLevels, currentSubLevel);
            return transferWorker.Deconstruct();
        }

        public static T ConstructDTO<T>(this object source, int allowedSubLevels = 1) where T : class, ITransferDTO
        {
            var transferWorker = new TransferConstructor<T>(source, allowedSubLevels);
            return transferWorker.Construct();
        }

        internal static T ConstructSubDTO<T>(this object source, Type parentType, int allowedSubLevels, int currentSubLevel) where T : class, ITransferDTO
        {
            var transferWorker = new TransferConstructor<T>(source, parentType, allowedSubLevels, currentSubLevel);
            return transferWorker.Construct();
        }

        public static IEnumerable<T> ConstructDTOCollection<T>(this IEnumerable<object> sourceCollection, int allowedSubLevels = 1) where T : class, ITransferDTO
        {
            var returnList = new List<T>();

            foreach (var sourceObject in sourceCollection)
            {
                var transferWorker = new TransferConstructor<T>(sourceObject, allowedSubLevels);
                returnList.Add(transferWorker.Construct());
            }

            return returnList;
        }

        internal static IEnumerable<T> ConstructSubDTOCollection<T>(this IEnumerable<object> sourceCollection, Type parentType, int allowedSubLevels, int currentSubLevel) where T : class, ITransferDTO
        {
            var returnList = new List<T>();

            foreach (var sourceObject in sourceCollection)
            {
                var transferWorker = new TransferConstructor<T>(sourceObject, parentType, allowedSubLevels, currentSubLevel);
                returnList.Add(transferWorker.Construct());
            }

            return returnList;
        }

        public static IEnumerable<T> DeconstructDTOCollection<T>(this IEnumerable<ITransferDTO> sourceCollection, int allowedSubLevels = 1) where T : class
        {
            var returnList = new List<T>();

            foreach (var sourceObject in sourceCollection)
            {
                var transferWorker = new TransferDeconstructor<T>(sourceObject, allowedSubLevels);
                returnList.Add(transferWorker.Deconstruct());
            }

            return returnList;
        }

        internal static IEnumerable<T> DeconstructSubDTOCollection<T>(this IEnumerable<ITransferDTO> sourceCollection, Type parentType, int allowedSubLevels, int subLevel) where T : class
        {
            var returnList = new List<T>();

            foreach (var sourceObject in sourceCollection)
            {
                var transferWorker = new TransferDeconstructor<T>(sourceObject, parentType, allowedSubLevels, subLevel);
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