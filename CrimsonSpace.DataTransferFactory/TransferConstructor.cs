namespace CrimsonSpace.DataTransferFactory
{
    using System;

    internal class TransferConstructor<T> : TransferBase<T> where T : class, ITransferDTO
    {
        #region Constructors

        internal TransferConstructor(object sourceObject, int allowedSubLevels) : base()
        {
            entityType = sourceObject.GetType();
            dtoParentType = null;
            entityObject = sourceObject;
            dtoType = typeof(T);

            this.allowedSubLevels = allowedSubLevels;
            currentSubLevel = 0;

            InitialiseMemberCollections();
            SetTransferAllMembers();
            CheckForTransferCompatibility();
        }

        internal TransferConstructor(object sourceObject, Type parentType, int allowedSubLevels, int currentSubLevel) : base()
        {
            entityType = sourceObject.GetType();
            dtoParentType = parentType;
            entityObject = sourceObject;
            dtoType = typeof(T);

            this.allowedSubLevels = allowedSubLevels;
            this.currentSubLevel = currentSubLevel;

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