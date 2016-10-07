namespace CrimsonSpace.DataTransferFactory
{
    using System;

    internal class TransferDeconstructor<T> : TransferBase<T> where T : class
    {
        #region Constructors

        internal TransferDeconstructor(object sourceObject, int allowedSubLevels) : base()
        {
            dtoType = sourceObject.GetType();
            entityParentType = null;
            dtoObject = sourceObject;
            entityType = typeof(T);

            this.allowedSubLevels = allowedSubLevels;
            currentSubLevel = 0;

            InitialiseMemberCollections();
            SetTransferAllMembers();
            CheckForTransferCompatibility();
        }

        internal TransferDeconstructor(object sourceObject, Type parentType, int allowedSubLevels, int currentSubLevel) : base()
        {
            dtoType = sourceObject.GetType();
            entityParentType = parentType;
            dtoObject = sourceObject;
            entityType = typeof(T);

            this.allowedSubLevels = allowedSubLevels;
            this.currentSubLevel = currentSubLevel;

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