using Cabronate.Base;
using Cabronate.DAO.Attributes;
using Cabronate.DAO.Errors;
using Cabronate.DAO.Mount;
using Cabronate.DAO.SQL;
using Cabronate.DAO.Types;
using Cabronate.DAO.UniversalDAO;
using System;
using System.Collections.Generic;
using System.Data;

namespace Cabronate.DAO.Utils
{
    public class ServiceDAO<T>
        where T : EcalcValueObject
    {
        private readonly DBContexto _dbctx;

        public ServiceDAO(DBContexto dbctx)
        {
            _dbctx = dbctx;
        }

        public int GetKeyFieldValue(T obj)
        {
            IKeyFieldEvaluator<T> keyEvaluator = KeyFieldFactory<T>.getKeyFieldClass(obj);
            return keyEvaluator.getKeyFieldValue();
        }

        public void SetKeyFieldValue(T obj, int value)
        {
            IKeyFieldEvaluator<T> keyEvaluator = KeyFieldFactory<T>.getKeyFieldClass(obj);
            keyEvaluator.setKeyFieldValue(value);
        }

        public int GetNewID(T obj)
        {
            if (obj.GetType().GetProperty(AttributeReaderSingleton.AttributeReader.getPropertyKeyField(obj)).PropertyType == typeof(int))
            {
                using (IDbCommand command = _dbctx.CreateCommand(""))
                {
                    command.Connection = _dbctx.GetConnection;
                    command.CommandText = AutoIncrementBehaviorFactory.getAutoIncrementBehavior(_dbctx.Provider).getAutoIncrementQuery(
                        AttributeReaderSingleton.AttributeReader.getTableName(obj),
                        AttributeReaderSingleton.AttributeReader.getKeyField(obj));
                    object resultScalar = command.ExecuteScalar();
                    if (resultScalar is System.DBNull)
                        resultScalar = 0;
                    return Convert.ToInt32(resultScalar);
                }
            }
            else
                throw new MinimumSpecificationsException(String.Format(ErrorMessages.INVALID_KEY_TYPE, obj.GetType().ToString()),
                    DateTime.Now);
        }

        public int GetID(T obj)
        {
            if (obj.GetType().GetProperty(AttributeReaderSingleton.AttributeReader.getPropertyKeyField(obj)).PropertyType == typeof(int))
            {   
                using (IDbCommand command = _dbctx.CreateCommand(""))
                {
                    command.Connection = _dbctx.GetConnection;
                    command.CommandText = AutoIncrementBehaviorFactory.getAutoIncrementBehavior(_dbctx.Provider).getKeyValueQuery(
                        AttributeReaderSingleton.AttributeReader.getTableName(obj),
                        AttributeReaderSingleton.AttributeReader.getKeyField(obj));
                    object resultScalar = command.ExecuteScalar();
                    if (resultScalar is System.DBNull)
                        resultScalar = 0;
                    return Convert.ToInt32(resultScalar);
                }
            }
            else
                throw new MinimumSpecificationsException(String.Format(ErrorMessages.INVALID_KEY_TYPE, obj.GetType().ToString()),
                    DateTime.Now);
        }

        public void SetParams(T obj, IDbCommand cmd, MountObjectMapper objMapper, bool isnew, List<string> fields = null)
        {
            // Incluído para verificar se é update, se for o parâmetro da propriedade chave da tabela 
            // tem que ser setado sempre, mesmo que for identity (SQL Server)
            List<MountObjectMapperField> fieldsMapper;
            if (isnew)
                fieldsMapper = objMapper.FieldsInsert;
            else
                fieldsMapper = objMapper.FieldsUpdate;
            string providerName = _dbctx.Provider;
            foreach (MountObjectMapperField field in fieldsMapper)
            {
                try
                {
                    if (isnew)
                    {
                        //EcalcLog.Log(typeof(UniversalDAO<T>), Cabronate.Base.EcalcLog.LogType.Info, "Setando parametros para um novo registro");
                        if (field.paramIns != null)
                        {
                            field.paramIns.Value = TypeFactory.getCType(field.Property.PropertyType, providerName).getValue(
                                field.Property.GetValue(obj, null), field.types, field.attributesValidation);
                            if (field.paramIns.Value == null)
                                field.paramIns.Value = DBNull.Value;
                        }
                    }
                    else
                    {
                        //EcalcLog.Log(typeof(UniversalDAO<T>), Cabronate.Base.EcalcLog.LogType.Info, "Setando parametros para um registro exitente");
                        field.paramUpd.Value = TypeFactory.getCType(field.Property.PropertyType, providerName).getValue(
                            field.Property.GetValue(obj, null), field.types, field.attributesValidation);

                        if (field.paramUpd.Value == null)
                            field.paramUpd.Value = DBNull.Value;
                    }
                }
                catch (Exception e)
                {
                    throw new LoadPropertyException(
                        string.Format(ErrorMessages.PROPERTY_ERROR, field.Property.Name, field.Property.DeclaringType), e, DateTime.Now);
                }
            }
        }
    }
}
