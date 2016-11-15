namespace CrimsonSpace.DataTransferFactory
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Class providing functionality for transferring from data transfer objects to entity objects
    /// </summary>
    /// <typeparam name="T">The entity object type</typeparam>
    /// <seealso cref="CrimsonSpace.DataTransferFactory.TransferBase{T}" />
    internal class TransferDeconstructor<T> : TransferBase<T> where T : class
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TransferDeconstructor{T}"/> class.
        /// </summary>
        /// <param name="sourceObject">The source object.</param>
        /// <param name="parentTypes">The parent types.</param>
        internal TransferDeconstructor(object sourceObject, List<Type> parentTypes = null) : base()
        {
            dtoObject = sourceObject;
            entityType = typeof(T);

            if (sourceObject != null)
            {
                dtoType = sourceObject.GetType();

                if (parentTypes == null)
                {
                    entityParentTypes = new List<Type>();
                }
                else
                {
                    entityParentTypes = parentTypes.ToList();
                }

                entityParentTypes.Add(entityType);

                InitialiseMemberCollections();
                SetTransferAllMembers();
                CheckForTransferCompatibility();
            }
        }

        #endregion Constructors

        #region Public Methods

        /// <summary>
        /// Deconstructs the data transfer object into an entity object
        /// </summary>
        /// <returns>Entity object</returns>
        internal T Deconstruct()
        {
            transferDirection = TransferDirections.DtoToEntity;
            entityObject = (T)Activator.CreateInstance(entityType);

            if (dtoObject != null)
            {
                TransferMembers();
            }

            return entityObject as T;
        }

        #endregion Public Methods
    }
}