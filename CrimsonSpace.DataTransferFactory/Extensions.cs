namespace CrimsonSpace.DataTransferFactory
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Extension methods
    /// </summary>
    public static class Extensions
    {
        ///// <summary>
        ///// Deconstructs a single DTO object.
        ///// </summary>
        ///// <typeparam name="T">The output type from the deconstruction</typeparam>
        ///// <param name="source">The source.</param>
        ///// <param name="allowedSubLevels">The number of allowed sub levels.</param>
        ///// <returns>The deconstructed object</returns>
        //public static T DeconstructDTO<T>(this ITransferDTO source) where T : class
        //{
        //    var transferWorker = new TransferDeconstructor<T>(source);
        //    return transferWorker.Deconstruct();
        //}

        /// <summary>
        /// Deconstructs a single sub DTO object.
        /// </summary>
        /// <typeparam name="T">The output type from the deconstruction</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="parentType">Type of the parent.</param>
        /// <param name="allowedSubLevels">The number of allowed sub levels.</param>
        /// <param name="currentSubLevel">The current sub level.</param>
        /// <returns>The deconstructed object</returns>
        public static T DeconstructDTO<T>(this ITransferDTO source, List<Type> parentTypes = null) where T : class
        {
            var transferWorker = new TransferDeconstructor<T>(source, parentTypes);
            return transferWorker.Deconstruct();
        }

        ///// <summary>
        ///// Constructs the DTO from a source object.
        ///// </summary>
        ///// <typeparam name="T">The DTO object type</typeparam>
        ///// <param name="source">The source.</param>
        ///// <param name="allowedSubLevels">The number of allowed sub levels.</param>
        ///// <returns>The constructed DTO object</returns>
        //public static T ConstructDTO<T>(this object source) where T : class, ITransferDTO
        //{
        //    var transferWorker = new TransferConstructor<T>(source);
        //    return transferWorker.Construct();
        //}

        /// <summary>
        /// Constructs the sub DTO from a source object.
        /// </summary>
        /// <typeparam name="T">The DTO object type</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="parentType">Type of the parent.</param>
        /// <param name="allowedSubLevels">The number of allowed sub levels.</param>
        /// <param name="currentSubLevel">The current sub level.</param>
        /// <returns></returns>
        public static T ConstructDTO<T>(this object source, List<Type> parentTypes = null) where T : class, ITransferDTO
        {
            var transferWorker = new TransferConstructor<T>(source, parentTypes);
            return transferWorker.Construct();
        }

        public static IEnumerable<T> ConstructDTOCollection<T>(this IEnumerable<object> sourceCollection, List<Type> parentTypes = null) where T : class, ITransferDTO
        {
            var returnList = new List<T>();

            foreach (var sourceObject in sourceCollection)
            {
                var transferWorker = new TransferConstructor<T>(sourceObject, parentTypes);
                returnList.Add(transferWorker.Construct());
            }

            return returnList;
        }

        public static IEnumerable<T> DeconstructDTOCollection<T>(this IEnumerable<ITransferDTO> sourceCollection, List<Type> parentTypes = null) where T : class
        {
            var returnList = new List<T>();

            foreach (var sourceObject in sourceCollection)
            {
                var transferWorker = new TransferDeconstructor<T>(sourceObject, parentTypes);
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