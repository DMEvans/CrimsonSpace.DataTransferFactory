namespace CrimsonSpace.DataTransferFactory
{
    using System;

    internal class TransferConstructor<T> : TransferBase<T> where T : class, ITransferDTO
    {
        #region Constructors

        internal TransferConstructor(object sourceObject) : base()
        {
            entityType = sourceObject.GetType();
            this.entityObject = sourceObject;
            this.dtoType = typeof(T);

            InitialiseMemberCollections();
            SetTransferAllMembers();
            CheckForTransferCompatibility();
        }

        #endregion Constructors

        #region Public Methods

        internal T Construct()
        {
            transferDirection = TransferDirections.EntityToDto;
            dtoObject = (T)Activator.CreateInstance(dtoType);
            TransferMembers();
            return dtoObject as T;
        }

        #endregion Public Methods
    }
}