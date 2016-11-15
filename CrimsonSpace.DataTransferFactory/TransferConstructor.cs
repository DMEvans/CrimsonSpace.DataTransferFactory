namespace CrimsonSpace.DataTransferFactory
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Class providing functionality for transferring from entity objects to data transfer objects
    /// </summary>
    /// <typeparam name="T">The data transfer object type</typeparam>
    /// <seealso cref="CrimsonSpace.DataTransferFactory.TransferBase{T}" />
    internal class TransferConstructor<T> : TransferBase<T> where T : class, ITransferDTO
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TransferConstructor{T}"/> class.
        /// </summary>
        /// <param name="sourceObject">The source object.</param>
        /// <param name="parentTypes">The parent types.</param>
        internal TransferConstructor(object sourceObject, List<Type> parentTypes = null) : base()
        {
            entityObject = sourceObject;
            dtoType = typeof(T);

            if (sourceObject != null)
            {
                entityType = sourceObject.GetType();

                if (parentTypes == null)
                {
                    dtoParentTypes = new List<Type>();
                }
                else
                {
                    dtoParentTypes = parentTypes.ToList();
                }

                dtoParentTypes.Add(dtoType);

                InitialiseMemberCollections();
                SetTransferAllMembers();
                CheckForTransferCompatibility();
            }
        }

        #endregion Constructors

        #region Public Methods

        /// <summary>
        /// Constructs the data transfer object from the entity object.
        /// </summary>
        /// <returns>Data transfer object</returns>
        internal T Construct()
        {
            transferDirection = TransferDirections.EntityToDto;
            dtoObject = (T)Activator.CreateInstance(dtoType);

            if (entityObject != null)
            {
                TransferMembers();
            }

            return dtoObject as T;
        }

        #endregion Public Methods
    }
}