namespace CrimsonSpace.DataTransferFactory
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    internal abstract class TransferBase<T>
    {
        #region Private Members

        protected MemberCollection dtoInfo = new MemberCollection();

        protected object dtoObject;

        protected Type dtoParentType;

        protected Type dtoType;

        protected MemberCollection entityInfo = new MemberCollection();

        protected object entityObject;

        protected Type entityParentType;

        protected Type entityType;

        protected bool ignoreUntransferredMembers;

        protected int allowedSubLevels;

        protected int currentSubLevel;

        protected bool transferAllMembers;

        protected TransferDirections transferDirection;

        protected List<TransferMemberReport> transferReport = new List<TransferMemberReport>();

        #endregion Private Members

        #region Private Methods

        protected void AddToTransferReport(string memberName, string transferName, string message, bool success)
        {
            var report = new TransferMemberReport()
            {
                MemberName = memberName,
                TransferName = transferName,
                Message = message,
                Success = success
            };

            transferReport.Add(report);
        }

        protected void CheckForTransferCompatibility()
        {
            var transferrableTypesAttribute = dtoType.GetCustomAttribute(typeof(TransferrableTypesAttribute)) as TransferrableTypesAttribute;

            if (transferrableTypesAttribute == null)
            {
                return;
            }

            var isCompatible = transferrableTypesAttribute.Types.ToList().Contains(entityType);

            if (!isCompatible)
            {
                throw new IncompatibleTypesException();
            }
        }

        protected bool DtoFieldFromEntityProperty(FieldInfo dtoField)
        {
            string transferName = GetTransferMemberName(dtoField);
            var subMemberAttribute = dtoField.GetCustomAttribute(typeof(TransferrableSubMemberAttribute)) as TransferrableSubMemberAttribute;

            try
            {
                var dtoFieldType = dtoField.FieldType;

                var entityProperty = (from p in entityInfo.Properties
                                      where p.Name.Equals(transferName, StringComparison.CurrentCulture)
                                      && (dtoFieldType.IsAssignableFrom(p.PropertyType) || subMemberAttribute != null)
                                      select p).FirstOrDefault();

                if (entityProperty == null)
                {
                    return false;
                }

                if (transferDirection == TransferDirections.EntityToDto)
                {
                    object entityValue = null;

                    if (subMemberAttribute != null)
                    {
                        var methodName = subMemberAttribute.IsList ? "ConstructSubDTOCollection" : "ConstructSubDTO";
                        var parameterType = subMemberAttribute.IsList ? dtoFieldType.GetGenericArguments()[0] : dtoFieldType;

                        if (IsSubLevelAllowed() && !IsParentType(parameterType))
                        {
                            MethodInfo constructMethod = typeof(Extensions).GetExtensionMethod(methodName).MakeGenericMethod(parameterType);
                            entityValue = constructMethod.Invoke(null, new[] { entityProperty.GetValue(entityObject), dtoType, allowedSubLevels, currentSubLevel + 1 });
                        }
                        else
                        {
                            entityValue = null;
                        }
                    }
                    else
                    {
                        entityValue = entityProperty.GetValue(entityObject);
                    }

                    dtoField.SetValue(dtoObject, entityValue);
                }
                else
                {
                    object dtoValue = null;

                    if (subMemberAttribute != null)
                    {
                        var methodName = subMemberAttribute.IsList ? "DeconstructSubDTOCollection" : "DeconstructSubDTO";
                        var parameterType = subMemberAttribute.IsList ? entityProperty.PropertyType.GetGenericArguments()[0] : entityProperty.PropertyType;

                        if (IsSubLevelAllowed() && !IsParentType(parameterType))
                        {
                            MethodInfo deconstructMethod = typeof(Extensions).GetExtensionMethod(methodName).MakeGenericMethod(parameterType);
                            dtoValue = deconstructMethod.Invoke(null, new[] { dtoField.GetValue(dtoObject), entityType, allowedSubLevels, currentSubLevel + 1 });
                        }
                        else
                        {
                            dtoValue = null;
                        }
                    }
                    else
                    {
                        dtoValue = dtoField.GetValue(dtoObject);
                    }

                    entityProperty.SetValue(entityObject, dtoValue);
                }

                AddToTransferReport(dtoField.Name, transferName, Constants.TRANSFER_COMPLETED, true);
            }
            catch (Exception ex)
            {
                AddToTransferReport(dtoField.Name, transferName, ex.Message, false);
                return false;
            }

            return true;
        }

        protected bool DtoFieldToEntityField(FieldInfo dtoField)
        {
            string transferName = GetTransferMemberName(dtoField);
            var subMemberAttribute = dtoField.GetCustomAttribute(typeof(TransferrableSubMemberAttribute)) as TransferrableSubMemberAttribute;

            try
            {
                var dtoFieldType = dtoField.FieldType;

                var entityField = (from p in entityInfo.Fields
                                   where p.Name.Equals(transferName, StringComparison.CurrentCulture)
                                   && (dtoFieldType.IsAssignableFrom(p.FieldType) || subMemberAttribute != null)
                                   select p).FirstOrDefault();

                if (entityField == null)
                {
                    return false;
                }

                if (transferDirection == TransferDirections.EntityToDto)
                {
                    object entityValue = null;

                    if (subMemberAttribute != null)
                    {
                        var methodName = subMemberAttribute.IsList ? "ConstructSubDTOCollection" : "ConstructSubDTO";
                        var parameterType = subMemberAttribute.IsList ? dtoFieldType.GetGenericArguments()[0] : dtoFieldType;

                        if (IsSubLevelAllowed() && !IsParentType(parameterType))
                        {
                            MethodInfo constructMethod = typeof(Extensions).GetExtensionMethod(methodName).MakeGenericMethod(parameterType);
                            entityValue = constructMethod.Invoke(null, new[] { entityField.GetValue(entityObject), dtoType, allowedSubLevels, currentSubLevel + 1 });
                        }
                        else
                        {
                            entityValue = null;
                        }
                    }
                    else
                    {
                        entityValue = entityField.GetValue(entityObject);
                    }

                    dtoField.SetValue(dtoObject, entityValue);
                }
                else
                {
                    object dtoValue = null;

                    if (subMemberAttribute != null)
                    {
                        var methodName = subMemberAttribute.IsList ? "DeconstructSubDTOCollection" : "DeconstructSubDTO";
                        var parameterType = subMemberAttribute.IsList ? entityField.FieldType.GetGenericArguments()[0] : entityField.FieldType;

                        if (IsSubLevelAllowed() && !IsParentType(parameterType))
                        {
                            MethodInfo deconstructMethod = typeof(Extensions).GetExtensionMethod(methodName).MakeGenericMethod(parameterType); ;
                            dtoValue = deconstructMethod.Invoke(null, new[] { dtoField.GetValue(dtoObject), entityType, allowedSubLevels, currentSubLevel + 1 });
                        }
                        else
                        {
                            dtoValue = null;
                        }
                    }
                    else
                    {
                        dtoValue = dtoField.GetValue(dtoObject);
                    }

                    entityField.SetValue(entityObject, dtoValue);
                }

                AddToTransferReport(dtoField.Name, transferName, Constants.TRANSFER_COMPLETED, true);
            }
            catch (Exception ex)
            {
                AddToTransferReport(dtoField.Name, transferName, ex.Message, false);
                return false;
            }

            return true;
        }

        protected bool DtoPropertyToEntityField(PropertyInfo dtoProperty)
        {
            string transferName = GetTransferMemberName(dtoProperty);
            var subMemberAttribute = dtoProperty.GetCustomAttribute(typeof(TransferrableSubMemberAttribute)) as TransferrableSubMemberAttribute;

            try
            {
                var dtoPropertyType = dtoProperty.PropertyType;

                var entityField = (from p in entityInfo.Fields
                                   where p.Name.Equals(transferName, StringComparison.CurrentCulture)
                                   && (dtoPropertyType.IsAssignableFrom(p.FieldType) || subMemberAttribute != null)
                                   select p).FirstOrDefault();

                if (entityField == null)
                {
                    return false;
                }

                if (transferDirection == TransferDirections.EntityToDto)
                {
                    object entityValue = null;

                    if (subMemberAttribute != null)
                    {
                        var methodName = subMemberAttribute.IsList ? "ConstructSubDTOCollection" : "ConstructSubDTO";
                        var parameterType = subMemberAttribute.IsList ? dtoPropertyType.GetGenericArguments()[0] : dtoPropertyType;

                        if (IsSubLevelAllowed() && !IsParentType(parameterType))
                        {
                            MethodInfo constructMethod = typeof(Extensions).GetExtensionMethod(methodName).MakeGenericMethod(parameterType);
                            entityValue = constructMethod.Invoke(null, new[] { entityField.GetValue(entityObject), dtoType, allowedSubLevels, currentSubLevel + 1 });
                        }
                        else
                        {
                            entityValue = null;
                        }
                    }
                    else
                    {
                        entityValue = entityField.GetValue(entityObject);
                    }

                    dtoProperty.SetValue(dtoObject, entityValue);
                }
                else
                {
                    object dtoValue = null;

                    if (subMemberAttribute != null)
                    {
                        var methodName = subMemberAttribute.IsList ? "DeconstructSubDTOCollection" : "DeconstructSubDTO";
                        var parameterType = subMemberAttribute.IsList ? entityField.FieldType.GetGenericArguments()[0] : entityField.FieldType;

                        if (IsSubLevelAllowed() && !IsParentType(parameterType))
                        {
                            MethodInfo deconstructMethod = typeof(Extensions).GetExtensionMethod(methodName).MakeGenericMethod(parameterType);
                            dtoValue = deconstructMethod.Invoke(null, new[] { dtoProperty.GetValue(dtoObject), entityType, allowedSubLevels, currentSubLevel + 1 });
                        }
                        else
                        {
                            dtoValue = null;
                        }
                    }
                    else
                    {
                        dtoValue = dtoProperty.GetValue(dtoObject);
                    }

                    entityField.SetValue(entityObject, dtoValue);
                }

                AddToTransferReport(dtoProperty.Name, transferName, Constants.TRANSFER_COMPLETED, true);
            }
            catch (Exception ex)
            {
                AddToTransferReport(dtoProperty.Name, transferName, ex.Message, false);
                return false;
            }

            return true;
        }

        private bool IsParentType(Type parameterType)
        {
            if (transferDirection == TransferDirections.EntityToDto)
            {
                return dtoParentType == parameterType;
            }
            else
            {
                return entityParentType == parameterType;
            }
        }

        protected bool DtoPropertyToEntityProperty(PropertyInfo dtoProperty)
        {
            string transferName = GetTransferMemberName(dtoProperty);
            var subMemberAttribute = dtoProperty.GetCustomAttribute(typeof(TransferrableSubMemberAttribute)) as TransferrableSubMemberAttribute;

            try
            {
                var dtoPropertyType = dtoProperty.PropertyType;

                var entityProperty = (from p in entityInfo.Properties
                                      where p.Name.Equals(transferName, StringComparison.CurrentCulture)
                                      && (dtoPropertyType.IsAssignableFrom(p.PropertyType) || subMemberAttribute != null)
                                      select p).FirstOrDefault();

                if (entityProperty == null)
                {
                    return false;
                }

                if (transferDirection == TransferDirections.EntityToDto)
                {
                    object entityValue = null;

                    if (subMemberAttribute != null)
                    {
                        var methodName = subMemberAttribute.IsList ? "ConstructSubDTOCollection" : "ConstructSubDTO";
                        var parameterType = subMemberAttribute.IsList ? dtoPropertyType.GetGenericArguments()[0] : dtoPropertyType;

                        if (IsSubLevelAllowed() && !IsParentType(parameterType))
                        {
                            MethodInfo constructMethod = typeof(Extensions).GetExtensionMethod(methodName).MakeGenericMethod(parameterType);
                            entityValue = constructMethod.Invoke(null, new[] { entityProperty.GetValue(entityObject), dtoType, allowedSubLevels, currentSubLevel + 1 });
                        }
                        else
                        {
                            entityValue = null;
                        }
                    }
                    else
                    {
                        entityValue = entityProperty.GetValue(entityObject);
                    }

                    dtoProperty.SetValue(dtoObject, entityValue);
                }
                else
                {
                    object dtoValue = null;

                    if (subMemberAttribute != null)
                    {
                        var methodName = subMemberAttribute.IsList ? "DeconstructSubDTOCollection" : "DeconstructSubDTO";
                        var parameterType = subMemberAttribute.IsList ? entityProperty.PropertyType.GetGenericArguments()[0] : entityProperty.PropertyType;

                        if (IsSubLevelAllowed() && !IsParentType(parameterType))
                        {
                            MethodInfo deconstructMethod = typeof(Extensions).GetExtensionMethod(methodName).MakeGenericMethod(parameterType);
                            dtoValue = deconstructMethod.Invoke(null, new[] { dtoProperty.GetValue(dtoObject), entityType, allowedSubLevels, currentSubLevel + 1 });
                        }
                        else
                        {
                            dtoValue = null;
                        }
                    }
                    else
                    {
                        dtoValue = dtoProperty.GetValue(dtoObject);
                    }

                    entityProperty.SetValue(entityObject, dtoValue);
                }

                AddToTransferReport(dtoProperty.Name, transferName, Constants.TRANSFER_COMPLETED, true);
            }
            catch (Exception ex)
            {
                AddToTransferReport(dtoProperty.Name, transferName, ex.Message, false);
                return false;
            }

            return true;
        }

        private bool IsSubLevelAllowed()
        {
            return currentSubLevel < allowedSubLevels;
        }

        protected string GetTransferMemberName(MemberInfo memberInfo)
        {
            var transferNameAttribute = memberInfo.GetCustomAttribute(typeof(TransferMemberNameAttribute));

            if (transferNameAttribute != null)
            {
                return (transferNameAttribute as TransferMemberNameAttribute).PropertyName;
            }
            else
            {
                return memberInfo.Name;
            }
        }

        protected void InitialiseMemberCollections()
        {
            entityInfo.Properties = entityType.GetProperties();
            entityInfo.Fields = entityType.GetFields();
            dtoInfo.Properties = dtoType.GetProperties();
            dtoInfo.Fields = dtoType.GetFields();
        }

        protected bool IsTransferrableMember(MemberInfo memberInfo)
        {
            bool isNonTransferrable = memberInfo.GetCustomAttribute(typeof(NonTransferrableMemberAttribute)) != null;
            bool isTransferrable = memberInfo.GetCustomAttribute(typeof(TransferrableMemberAttribute)) != null;
            bool isSubMember = memberInfo.GetCustomAttribute(typeof(TransferrableSubMemberAttribute)) != null;
            bool isValidForDirection = false;

            if (transferDirection == TransferDirections.DtoToEntity)
            {
                isValidForDirection = memberInfo.GetCustomAttribute(typeof(TransferToDtoOnlyAttribute)) == null;
            }
            else if (transferDirection == TransferDirections.EntityToDto)
            {
                isValidForDirection = memberInfo.GetCustomAttribute(typeof(TransferToEntityOnlyAttribute)) == null;
            }

            if (!isValidForDirection)
            {
                return false;
            }
            else if (transferAllMembers && isNonTransferrable)
            {
                return false;
            }
            else if (isSubMember || isTransferrable || transferAllMembers)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool DtoToEntityTransferCheck(MemberInfo memberInfo)
        {
            bool nonTransferrable = memberInfo.GetCustomAttribute(typeof(NonTransferrableMemberAttribute)) != null;
            bool transferrable = memberInfo.GetCustomAttribute(typeof(TransferrableMemberAttribute)) != null;
            bool subMemberAttribute = memberInfo.GetCustomAttribute(typeof(TransferrableSubMemberAttribute)) != null;

            if (transferAllMembers && nonTransferrable)
            {
                return false;
            }
            else if (subMemberAttribute || transferrable || transferAllMembers)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        protected void SetIgnoreUntranferredMembers()
        {
            ignoreUntransferredMembers = dtoType.GetCustomAttribute(typeof(IgnoreUntransferredMembersAttribute)) != null;
        }

        protected void SetTransferAllMembers()
        {
            transferAllMembers = dtoType.GetCustomAttribute(typeof(TransferAllMembersAttribute)) != null;
        }

        protected void TransferField(FieldInfo dtoField)
        {
            bool transferred;

            transferred = DtoFieldToEntityField(dtoField);

            if (transferred)
            {
                return;
            }

            transferred = DtoFieldFromEntityProperty(dtoField);

            if (!transferred)
            {
                string transferName = GetTransferMemberName(dtoField);
                AddToTransferReport(dtoField.Name, transferName, Constants.NO_MATCHING_MEMBER_FOUND, false);
            }
        }

        protected void TransferFields()
        {
            foreach (var dtoField in dtoInfo.Fields)
            {
                if (IsTransferrableMember(dtoField))
                {
                    TransferField(dtoField);
                }
            }
        }

        protected void TransferMembers()
        {
            TransferProperties();
            TransferFields();

            if (transferReport.Any(r => r.Success == false))
            {
                throw new Exception();
            }
        }

        protected void TransferProperties()
        {
            foreach (var dtoProperty in dtoInfo.Properties)
            {
                if (IsTransferrableMember(dtoProperty))
                {
                    TransferProperty(dtoProperty);
                }
            }
        }

        protected void TransferProperty(PropertyInfo dtoProperty)
        {
            bool transferred = DtoPropertyToEntityProperty(dtoProperty);

            if (transferred)
            {
                return;
            }

            transferred = DtoPropertyToEntityField(dtoProperty);

            if (!transferred)
            {
                string transferName = GetTransferMemberName(dtoProperty);
                AddToTransferReport(dtoProperty.Name, transferName, Constants.NO_MATCHING_MEMBER_FOUND, false);
            }
        }

        #endregion Private Methods
    }
}