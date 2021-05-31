using Cabronate.Base;
using Cabronate.DAO.Attributes;
using Cabronate.DAO.SQL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Reflection;

namespace Cabronate.DAO.Mount
{
    public class MountObjectMapper
    {
        public PropertyInfo[] Properties { get; set; }

        public List<MountObjectMapperField> FieldsInsert { get; set; }

        public List<MountObjectMapperField> FieldsUpdate { get; set; }

        public List<MountObjectMapperField> AllFields
        {
            get
            {
                List<MountObjectMapperField> allFields = new List<MountObjectMapperField>();
                allFields.AddRange(FieldsInsert);
                allFields.AddRange(FieldsUpdate);
                allFields = allFields.Distinct().ToList();
                return allFields;
            }
        }

        public void BuildType(Type t)
        {
            FieldsInsert = new List<MountObjectMapperField>();
            FieldsUpdate = new List<MountObjectMapperField>();

            Properties = t.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            foreach (PropertyInfo property in Properties)
            {
                MountObjectMapperField field = new MountObjectMapperField();
                field.FieldOrdinal = -1;
                field.Property = property;

                object[] attributes = property.GetCustomAttributes(typeof(FieldNameAttribute), false);
                if (attributes.Count() > 0)
                    field.FieldName = ((FieldNameAttribute)attributes[0]).Description;

                field.TemDefault = (property.GetCustomAttributes(typeof(DefaultAttribute), false).Count() > 0);

                FieldsInsert.Add(field);
            }
            FieldsUpdate.AddRange(FieldsInsert);
        }

        public void BuildTypeAndOrginal(Type t, IDataReader reader, object concreteType)
        {
            FieldsInsert = new List<MountObjectMapperField>();
            FieldsUpdate = new List<MountObjectMapperField>();

            EcalcValueObjectTableInfo tableInfo = EcalcValueObjectMapper.BuildEcalcValueObjectMapper(((EcalcValueObject)concreteType));
            List<string> fieldNames = Enumerable.Range(0, reader.FieldCount).Select(i => reader.GetName(i)).ToList();

            foreach (EcalcValueObjectFieldInfo field in tableInfo.Fields)
            {
                MountObjectMapperField fieldMapper = new MountObjectMapperField();
                fieldMapper.FieldName = field.FieldName;
                if (fieldNames.Find(f => f.Equals(field.FieldName, StringComparison.InvariantCultureIgnoreCase)) != null)
                    fieldMapper.FieldOrdinal = reader.GetOrdinal(field.FieldName);
                else
                    fieldMapper.FieldOrdinal = -1;
                fieldMapper.Property = field.Property;
                fieldMapper.TemDefault = field.TemDefault;
                FieldsInsert.Add(fieldMapper);

            }
            FieldsUpdate.AddRange(FieldsInsert);
        }

        private bool primaryKey(object[] attributesKeyField)
        {
            return ((attributesKeyField != null) && (attributesKeyField.Count() > 0));
        }

        public void BuildTypeForSave(Type t, List<string> fields, bool EInsert)
        {
            List<MountObjectMapperField> Fields;
            Fields = new List<MountObjectMapperField>();

            Properties = t.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            foreach (PropertyInfo property in Properties)
            {
                MountObjectMapperField field = new MountObjectMapperField();
                field.FieldOrdinal = -1;
                field.Property = property;

                object[] attributes = property.GetCustomAttributes(typeof(FieldNameAttribute), false);
                if (attributes.Count() > 0)
                    field.FieldName = ((FieldNameAttribute)attributes[0]).Description;
                else
                    continue;

                field.TemDefault = (property.GetCustomAttributes(typeof(DefaultAttribute), false).Count() > 0);

                object[] attributesType = property.GetCustomAttributes(typeof(TypeDetailAttribute), false);
                field.types =
                    (((attributesType != null) && (attributesType.Count() > 0))
                    ? ((TypeDetailAttribute)attributesType[0]).type
                    : new FieldTypeDetail[] { FieldTypeDetail.None });

                object[] attributesValidationCustom = property.GetCustomAttributes(typeof(ValidationAttribute), false);
                field.attributesValidation =
                    (((attributesValidationCustom != null) && (attributesValidationCustom.Count() > 0))
                    ? ((ValidationAttribute[])attributesValidationCustom)
                    : new ValidationAttribute[] { });

                field.attributesKeyField = property.GetCustomAttributes(typeof(KeyFieldAttribute), false);

                if (fields != null)
                {
                    if ((!fields.Contains(((FieldNameAttribute)attributes[0]).Description, StringComparer.InvariantCultureIgnoreCase)) && (!primaryKey(field.attributesKeyField)))
                        continue;
                }

                Fields.Add(field);
            }

            if (EInsert)
                FieldsInsert = Fields;
            else
                FieldsUpdate = Fields;
        }

        internal void BuildParamsInsert(DBContexto dbctx, IDbCommand dbCommand, bool forcarID = false)
        {
            foreach (MountObjectMapperField field in this.FieldsInsert)
            {
                if ((field.attributesKeyField != null) && (field.attributesKeyField.Count() > 0))
                {
                    if ((forcarID) || (AutoIncrementBehaviorFactory.getAutoIncrementBehavior(dbctx.Provider).keyFieldInInsert()))
                        field.paramIns = dbctx.CreateParameter(dbCommand, field.FieldName);
                }
                else
                {
                    field.paramIns = dbctx.CreateParameter(dbCommand, field.FieldName);
                }
            }
        }

        internal void BuildParamsUpdate(DBContexto dbctx, IDbCommand dbCommand)
        {
            foreach (MountObjectMapperField field in this.FieldsUpdate)
            {
                field.paramUpd = dbctx.CreateParameter(dbCommand, field.FieldName);
            }
        }
    }
}
