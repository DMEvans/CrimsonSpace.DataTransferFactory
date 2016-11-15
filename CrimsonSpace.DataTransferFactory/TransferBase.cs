namespace CrimsonSpace.DataTransferFactory
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// The base transfer class for both directions
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal abstract class TransferBase<T>
    {
        #region Private Members

        /// <summary>
        /// The member information of the data transfer object
        /// </summary>
        protected MemberCollection dtoInfo = new MemberCollection();

        /// <summary>
        /// The data transfer object
        /// </summary>
        protected object dtoObject;

        /// <summary>
        /// The parent types from which the data transfer object has been extracted - this is used to prevent deconstruction of circular references
        /// </summary>
        protected List<Type> dtoParentTypes;

        /// <summary>
        /// The type of data transfer object
        /// </summary>
        protected Type dtoType;

        /// <summary>
        /// The member information of the entity object
        /// </summary>
        protected MemberCollection entityInfo = new MemberCollection();

        /// <summary>
        /// The entity object
        /// </summary>
        protected object entityObject;

        /// <summary>
        /// The parent types from which the entity object has been extracted - this is used to prevent construction of circular references
        /// </summary>
        protected List<Type> entityParentTypes;

        /// <summary>
        /// The type of the entity object
        /// </summary>
        protected Type entityType;

        /// <summary>
        /// Value indicating whether untransferred members should be ignored - when false an exception will be raised
        /// </summary>
        protected bool ignoreUntransferredMembers;

        /// <summary>
        /// Value indicating whether all members should be transferred unless specifically marked with the NonTransferrableMember attribue
        /// </summary>
        protected bool transferAllMembers;

        /// <summary>
        /// The transfer direction
        /// </summary>
        protected TransferDirections transferDirection;

        /// <summary>
        /// The transfer report
        /// </summary>
        protected List<TransferMemberReport> transferReport = new List<TransferMemberReport>();

        #endregion Private Members

        #region Private Methods

        /// <summary>
        /// Adds to transfer report.
        /// </summary>
        /// <param name="memberName">Name of the member.</param>
        /// <param name="transferName">Name of the transfer.</param>
        /// <param name="message">The message.</param>
        /// <param name="success">if set to <c>true</c> [success].</param>
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

        /// <summary>
        /// Checks for transfer compatibility.
        /// </summary>
        /// <exception cref="IncompatibleTypesException"></exception>
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

        /// <summary>
        /// Provides bi-directional transfer between a field in the data transfer object and a property in the entity object
        /// </summary>
        /// <param name="dtoField">The data transfer object field field.</param>
        /// <returns>Success</returns>
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
                        var methodName = subMemberAttribute.IsList ? "ConstructDTOCollection" : "ConstructDTO";
                        var parameterType = subMemberAttribute.IsList ? dtoFieldType.GetGenericArguments()[0] : dtoFieldType;

                        if (!IsParentType(parameterType))
                        {
                            MethodInfo constructMethod = typeof(Extensions).GetExtensionMethod(methodName).MakeGenericMethod(parameterType);
                            entityValue = constructMethod.Invoke(null, new[] { entityProperty.GetValue(entityObject), dtoParentTypes });
                        }
                        else
                        {
                            entityValue = null;
                            AddToTransferReport(entityProperty.Name, transferName, Constants.TransferSkipped, true);
                        }
                    }
                    else
                    {
                        entityValue = entityProperty.GetValue(entityObject);
                    }

                    if (entityValue != null)
                    {
                        dtoField.SetValue(dtoObject, entityValue);
                        AddToTransferReport(entityProperty.Name, transferName, Constants.TransferCompleted, true);
                    }
                    else
                    {
                        AddToTransferReport(entityProperty.Name, transferName, Constants.ValueWasNull, true);
                    }
                }
                else
                {
                    object dtoValue = null;

                    if (subMemberAttribute != null)
                    {
                        var methodName = subMemberAttribute.IsList ? "DeconstructDTOCollection" : "DeconstructDTO";
                        var parameterType = subMemberAttribute.IsList ? entityProperty.PropertyType.GetGenericArguments()[0] : entityProperty.PropertyType;

                        if (!IsParentType(parameterType))
                        {
                            MethodInfo deconstructMethod = typeof(Extensions).GetExtensionMethod(methodName).MakeGenericMethod(parameterType);
                            dtoValue = deconstructMethod.Invoke(null, new[] { dtoField.GetValue(dtoObject), entityParentTypes });
                        }
                        else
                        {
                            dtoValue = null;
                            AddToTransferReport(dtoField.Name, transferName, Constants.TransferSkipped, true);
                        }
                    }
                    else
                    {
                        dtoValue = dtoField.GetValue(dtoObject);
                    }

                    if (dtoValue != null)
                    {
                        entityProperty.SetValue(entityObject, dtoValue);
                        AddToTransferReport(dtoField.Name, transferName, Constants.TransferCompleted, true);
                    }
                    else
                    {
                        AddToTransferReport(dtoField.Name, transferName, Constants.ValueWasNull, true);
                    }
                }
            }
            catch (Exception ex)
            {
                AddToTransferReport(dtoField.Name, transferName, ex.Message, false);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Provides bi-directional transfer between a field in the data transfer object and a field in the entity object
        /// </summary>
        /// <param name="dtoField">The data transfer object field.</param>
        /// <returns>Success</returns>
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
                        var methodName = subMemberAttribute.IsList ? "ConstructDTOCollection" : "ConstructDTO";
                        var parameterType = subMemberAttribute.IsList ? dtoFieldType.GetGenericArguments()[0] : dtoFieldType;

                        if (!IsParentType(parameterType))
                        {
                            MethodInfo constructMethod = typeof(Extensions).GetExtensionMethod(methodName).MakeGenericMethod(parameterType);
                            entityValue = constructMethod.Invoke(null, new[] { entityField.GetValue(entityObject), dtoParentTypes });
                        }
                        else
                        {
                            entityValue = null;
                            AddToTransferReport(entityField.Name, transferName, Constants.TransferSkipped, true);
                        }
                    }
                    else
                    {
                        entityValue = entityField.GetValue(entityObject);
                    }

                    if (entityValue != null)
                    {
                        dtoField.SetValue(dtoObject, entityValue);
                        AddToTransferReport(entityField.Name, transferName, Constants.TransferCompleted, true);
                    }
                    else
                    {
                        AddToTransferReport(entityField.Name, transferName, Constants.ValueWasNull, true);
                    }
                }
                else
                {
                    object dtoValue = null;

                    if (subMemberAttribute != null)
                    {
                        var methodName = subMemberAttribute.IsList ? "DeconstructDTOCollection" : "DeconstructDTO";
                        var parameterType = subMemberAttribute.IsList ? entityField.FieldType.GetGenericArguments()[0] : entityField.FieldType;

                        if (!IsParentType(parameterType))
                        {
                            MethodInfo deconstructMethod = typeof(Extensions).GetExtensionMethod(methodName).MakeGenericMethod(parameterType); ;
                            dtoValue = deconstructMethod.Invoke(null, new[] { dtoField.GetValue(dtoObject), entityParentTypes });
                        }
                        else
                        {
                            dtoValue = null;
                            AddToTransferReport(dtoField.Name, transferName, Constants.TransferSkipped, true);
                        }
                    }
                    else
                    {
                        dtoValue = dtoField.GetValue(dtoObject);
                    }

                    if (dtoValue != null)
                    {
                        entityField.SetValue(entityObject, dtoValue);
                        AddToTransferReport(dtoField.Name, transferName, Constants.TransferCompleted, true);
                    }
                    else
                    {
                        AddToTransferReport(dtoField.Name, transferName, Constants.ValueWasNull, true);
                    }
                }
            }
            catch (Exception ex)
            {
                AddToTransferReport(dtoField.Name, transferName, ex.Message, false);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Provides bi-directional transfer between a property in the data transfer object and a field in the entity object
        /// </summary>
        /// <param name="dtoProperty">The data transfer object property.</param>
        /// <returns>Success</returns>
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
                        var methodName = subMemberAttribute.IsList ? "ConstructDTOCollection" : "ConstructDTO";
                        var parameterType = subMemberAttribute.IsList ? dtoPropertyType.GetGenericArguments()[0] : dtoPropertyType;

                        if (!IsParentType(parameterType))
                        {
                            MethodInfo constructMethod = typeof(Extensions).GetExtensionMethod(methodName).MakeGenericMethod(parameterType);
                            entityValue = constructMethod.Invoke(null, new[] { entityField.GetValue(entityObject), dtoParentTypes });
                        }
                        else
                        {
                            entityValue = null;
                            AddToTransferReport(dtoProperty.Name, transferName, Constants.TransferSkipped, true);
                        }
                    }
                    else
                    {
                        entityValue = entityField.GetValue(entityObject);
                    }

                    if (entityValue != null)
                    {
                        dtoProperty.SetValue(dtoObject, entityValue);
                        AddToTransferReport(entityField.Name, transferName, Constants.TransferCompleted, true);
                    }
                    else
                    {
                        AddToTransferReport(entityField.Name, transferName, Constants.ValueWasNull, true);
                    }
                }
                else
                {
                    object dtoValue = null;

                    if (subMemberAttribute != null)
                    {
                        var methodName = subMemberAttribute.IsList ? "DeconstructDTOCollection" : "DeconstructDTO";
                        var parameterType = subMemberAttribute.IsList ? entityField.FieldType.GetGenericArguments()[0] : entityField.FieldType;

                        if (!IsParentType(parameterType))
                        {
                            MethodInfo deconstructMethod = typeof(Extensions).GetExtensionMethod(methodName).MakeGenericMethod(parameterType);
                            dtoValue = deconstructMethod.Invoke(null, new[] { dtoProperty.GetValue(dtoObject), entityParentTypes });
                        }
                        else
                        {
                            dtoValue = null;
                            AddToTransferReport(dtoProperty.Name, transferName, Constants.TransferSkipped, true);
                        }
                    }
                    else
                    {
                        dtoValue = dtoProperty.GetValue(dtoObject);
                    }

                    if (dtoValue != null)
                    {
                        entityField.SetValue(entityObject, dtoValue);
                        AddToTransferReport(dtoProperty.Name, transferName, Constants.TransferCompleted, true);
                    }
                    else
                    {
                        AddToTransferReport(dtoProperty.Name, transferName, Constants.ValueWasNull, true);
                    }
                }
            }
            catch (Exception ex)
            {
                AddToTransferReport(dtoProperty.Name, transferName, ex.Message, false);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Provides bi-directional transfer between a property in the data transfer object and a property in the entity object
        /// </summary>
        /// <param name="dtoProperty">The data transfer object property.</param>
        /// <returns>Success</returns>
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
                        var methodName = subMemberAttribute.IsList ? "ConstructDTOCollection" : "ConstructDTO";
                        var parameterType = subMemberAttribute.IsList ? dtoPropertyType.GetGenericArguments()[0] : dtoPropertyType;

                        if (!IsParentType(parameterType))
                        {
                            MethodInfo constructMethod = typeof(Extensions).GetExtensionMethod(methodName).MakeGenericMethod(parameterType);
                            entityValue = constructMethod.Invoke(null, new[] { entityProperty.GetValue(entityObject), dtoParentTypes });
                        }
                        else
                        {
                            entityValue = null;
                            AddToTransferReport(entityProperty.Name, transferName, Constants.TransferSkipped, true);
                        }
                    }
                    else
                    {
                        entityValue = entityProperty.GetValue(entityObject);
                    }

                    if (entityValue != null)
                    {
                        dtoProperty.SetValue(dtoObject, entityValue);
                        AddToTransferReport(entityProperty.Name, transferName, Constants.TransferCompleted, true);
                    }
                    else
                    {
                        AddToTransferReport(entityProperty.Name, transferName, Constants.ValueWasNull, true);
                    }
                }
                else
                {
                    object dtoValue = null;

                    if (subMemberAttribute != null)
                    {
                        var methodName = subMemberAttribute.IsList ? "DeconstructDTOCollection" : "DeconstructDTO";
                        var parameterType = subMemberAttribute.IsList ? entityProperty.PropertyType.GetGenericArguments()[0] : entityProperty.PropertyType;

                        if (!IsParentType(parameterType))
                        {
                            MethodInfo deconstructMethod = typeof(Extensions).GetExtensionMethod(methodName).MakeGenericMethod(parameterType);
                            dtoValue = deconstructMethod.Invoke(null, new[] { dtoProperty.GetValue(dtoObject), entityParentTypes });
                        }
                        else
                        {
                            dtoValue = null;
                            AddToTransferReport(dtoProperty.Name, transferName, Constants.TransferSkipped, true);
                        }
                    }
                    else
                    {
                        dtoValue = dtoProperty.GetValue(dtoObject);
                    }

                    if (dtoValue != null)
                    {
                        entityProperty.SetValue(entityObject, dtoValue);
                        AddToTransferReport(dtoProperty.Name, transferName, Constants.TransferCompleted, true);
                    }
                    else
                    {
                        AddToTransferReport(dtoProperty.Name, transferName, Constants.ValueWasNull, true);
                    }
                }
            }
            catch (Exception ex)
            {
                AddToTransferReport(dtoProperty.Name, transferName, ex.Message, false);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Gets the name of the member being transferred.
        /// </summary>
        /// <param name="memberInfo">The member information.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Initialises the member collections.
        /// </summary>
        protected void InitialiseMemberCollections()
        {
            entityInfo.Properties = entityType.GetProperties();
            entityInfo.Fields = entityType.GetFields();
            dtoInfo.Properties = dtoType.GetProperties();
            dtoInfo.Fields = dtoType.GetFields();
        }

        /// <summary>
        /// Determines whether the specified member is transferrable
        /// </summary>
        /// <param name="memberInfo">The member information.</param>
        /// <returns>
        ///   <c>true</c> if the member is tranferrable; otherwise, <c>false</c>.
        /// </returns>
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

        /// <summary>
        /// Sets a value indicating whether untransferred members should be ignored.
        /// </summary>
        protected void SetIgnoreUntranferredMembers()
        {
            ignoreUntransferredMembers = dtoType.GetCustomAttribute(typeof(IgnoreUntransferredMembersAttribute)) != null;
        }

        /// <summary>
        /// Sets a value indicating whether all members should be transferred except this marked with the NonTransferrableMember attribute
        /// </summary>
        protected void SetTransferAllMembers()
        {
            transferAllMembers = dtoType.GetCustomAttribute(typeof(TransferAllMembersAttribute)) != null;
        }

        /// <summary>
        /// Transfers the field.
        /// </summary>
        /// <param name="dtoField">The dto field.</param>
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
                AddToTransferReport(dtoField.Name, transferName, Constants.NoMatchingMemberFound, false);
            }
        }

        /// <summary>
        /// Transfers the fields.
        /// </summary>
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

        /// <summary>
        /// Transfers the members.
        /// </summary>
        /// <exception cref="Exception"></exception>
        protected void TransferMembers()
        {
            TransferProperties();
            TransferFields();

            if (transferReport.Any(r => r.Success == false))
            {
                //throw new Exception();
            }
        }

        /// <summary>
        /// Transfers the properties.
        /// </summary>
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

        /// <summary>
        /// Transfers the property.
        /// </summary>
        /// <param name="dtoProperty">The dto property.</param>
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
                AddToTransferReport(dtoProperty.Name, transferName, Constants.NoMatchingMemberFound, false);
            }
        }

        /// <summary>
        /// Determines whether the current type is already a parent of the source type.
        /// </summary>
        /// <param name="parameterType">Type of the parameter.</param>
        /// <returns>
        ///   <c>true</c> if [is parent type] [the specified parameter type]; otherwise, <c>false</c>.
        /// </returns>
        protected bool IsParentType(Type parameterType)
        {
            if (transferDirection == TransferDirections.EntityToDto)
            {
                return dtoParentTypes.Contains(parameterType);
            }
            else
            {
                return entityParentTypes.Contains(parameterType);
            }
        }

        #endregion Private Methods
    }
}