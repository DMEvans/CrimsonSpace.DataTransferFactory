namespace CrimsonSpace.DataTransferFactory
{
    using System;

    internal class TransferDeconstructor<T> : TransferBase<T> where T : class
    {
        #region Constructors

        internal TransferDeconstructor(object sourceObject) : base()
        {
            dtoType = sourceObject.GetType();
            dtoObject = sourceObject;
            entityType = typeof(T);

            InitialiseMemberCollections();
            SetTransferAllMembers();
            CheckForTransferCompatibility();
        }

        #endregion Constructors

        #region Public Methods

        internal T Deconstruct()
        {
            transferDirection = TransferDirections.DtoToEntity;
            entityObject = (T)Activator.CreateInstance(entityType);
            TransferMembers();
            return entityObject as T;
        }

        #endregion Public Methods
    }
}