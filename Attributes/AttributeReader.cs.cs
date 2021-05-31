using Cabronate.DAO.Business_Objects;
using Cabronate.DAO.Errors;
using Cabronate.DAO.Value_Objects;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace Cabronate.DAO.Attributes
{
    /// <summary>
    /// Classe responsavel por ler os atributos dos VO's previamente preparados
    /// </summary>
    public class AttributeReader : IAttributeReader
    {
        /// <summary>
        /// Método usado para ler o nome da tabela referente a um VO
        /// </summary>
        /// <param name="o">Objeto que desejamos saber a tabela referente </param>
        /// <returns>Nome da tabela responsável no banco</returns>
        public string getTableName(Object o)
        {
            Type t = o.GetType();
            object[] attList = t.GetCustomAttributes(typeof(TableNameAttribute), false);
            foreach (TableNameAttribute att in attList)
            {
                return att.Description;
            }
            throw new MinimumSpecificationsException(
                string.Format(ErrorMessages.INVALID_VO_TABLE_NAME, o.GetType().ToString()),
                DateTime.Now);
        }

        public string getDescriptionTableName(Object o)
        {
            Type t = o.GetType();
            object[] attList = t.GetCustomAttributes(typeof(TableNameAttribute), false);
            object[] attListDescription = t.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attListDescription.Count() > 0)
                return ((DescriptionAttribute)attListDescription[0]).Description;
            
            else if (attList.Count() > 0)
                return ((TableNameAttribute)attList[0]).Description;

            else            
                throw new MinimumSpecificationsException(
                    string.Format(ErrorMessages.INVALID_VO_TABLE_NAME, o.GetType().ToString()),
                    DateTime.Now);
        }

        public bool getTableLogable(Object o)
        {
            Type t = o.GetType();
            object[] attList = t.GetCustomAttributes(typeof(NotLogableAttribute), false);
            return (attList.Length == 0);
        }

        public static string getKeyField(Type tipo)
        {
            PropertyInfo[] propList = tipo.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            foreach (PropertyInfo prop in propList)
            {
                object[] attList = prop.GetCustomAttributes(typeof(KeyFieldAttribute), false);
                if ((attList != null) && (attList.Count() > 0))
                {
                    attList = prop.GetCustomAttributes(typeof(FieldNameAttribute), false);
                    if ((attList != null) && (attList.Count() > 0))
                        return ((FieldNameAttribute)attList.First()).Description;
                    else
                        throw new MinimumSpecificationsException(string.Format(ErrorMessages.INVALID_VO_FIELD_NAMEPK,
                            tipo.ToString()), DateTime.Now);
                }
            }
            throw new MinimumSpecificationsException(string.Format(ErrorMessages.INVALID_VO_PRIMARY_KEY,
                tipo.ToString()), DateTime.Now);
        }

        /// <summary>
        /// Método usado para ler a primary key da tabela referente a um VO
        /// </summary>
        /// <param name="o">Objeto que desejamos saber a primary key referente </param>
        /// <returns>Nome do campo que é primary key responsável na tabela</returns>
        public string getKeyField(Object o)
        {
            return getKeyField(o.GetType());
        }

        public string GetFirstNotKeyField(Object o)
        {
            Type tipo = o.GetType();
            PropertyInfo[] propList = tipo.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            if (!(propList is null) && (propList.Count() > 0))
            {
                object[] attList = propList[1].GetCustomAttributes(typeof(FieldNameAttribute), false);
                if ((attList != null) && (attList.Count() > 0))
                    return ((FieldNameAttribute)attList.First()).Description;
                else
                    throw new MinimumSpecificationsException(string.Format(ErrorMessages.INVALID_VO_SECOND_FIELD_NAME,
                        tipo.ToString()), DateTime.Now);
            }
            throw new MinimumSpecificationsException(string.Format(ErrorMessages.INVALID_VO_SECOND_FIELD,
                tipo.ToString()), DateTime.Now);
        }

        public string getFieldName(PropertyInfo prop)
        {
            object[] attList = prop.GetCustomAttributes(typeof(FieldNameAttribute), false);
            if ((attList != null) && (attList.Count() > 0))
                return ((FieldNameAttribute)attList.First()).Description;
            return "";
        }

        public string getDisplayFieldName(PropertyInfo prop)
        {
            object[] attList = prop.GetCustomAttributes(typeof(FieldNameAttribute), false);
            object[] attListDisplay = prop.GetCustomAttributes(typeof(DisplayAttribute), false);

            if (attListDisplay.Count() > 0)
                return ((DisplayAttribute)attListDisplay[0]).Name;

            else if (attList.Count() > 0)
                return ((FieldNameAttribute)attList[0]).Description;

            else
                return "";
        }

        public static int getKeyValue(Object o)
        {
            PropertyInfo[] propList = o.GetType().GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).Where(prop => prop.IsDefined(typeof(KeyFieldAttribute), false)).ToArray();
            foreach (PropertyInfo prop in propList)
            {
                return Convert.ToInt32(prop.GetValue(o, null));
            }
            throw new Exception(string.Format("Atributo [KeyFieldAttribute] não localizado no objeto {0}!", o.GetType().ToString()));
        }

        public static void setKeyValue(Object o, int keyValue)
        {
            PropertyInfo[] propList = o.GetType().GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).Where(prop => prop.IsDefined(typeof(KeyFieldAttribute), false)).ToArray();
            foreach (PropertyInfo prop in propList)
            {
                prop.SetValue(o, keyValue);
            }
        }

        /// <summary>
        /// Método usado para ler os campos da tabela referente a um VO
        /// </summary>
        /// <param name="o">Objeto que desejamos saber os campos referentes </param>
        /// <returns>Nome os campos que representam as propriedades da nossa classe</returns>
        public List<string> getFieldsName(Object o, List<string> fields = null)
        {
            List<string> fieldNames = new List<string>();
            PropertyInfo[] propInfo = o.GetType().GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            foreach (PropertyInfo prop in propInfo)
            {
                var attList = prop.GetCustomAttributes(typeof(FieldNameAttribute), false);
                if ((attList != null) && (attList.Count() > 0))
                {
                    if (fields != null)
                    {
                        if (fields.Contains(((FieldNameAttribute)attList.First()).Description))
                            fieldNames.Add(((FieldNameAttribute)attList.First()).Description);
                    }
                    else
                        fieldNames.Add(((FieldNameAttribute)attList.First()).Description);
                }
            }
            if (fieldNames.Count() > 0)
                return fieldNames;
            else
                throw new MinimumSpecificationsException(string.Format(ErrorMessages.INVALID_VO_FIELD_NAME, o.GetType()
                    .ToString()), DateTime.Now);
        }

        public List<string> getFieldsNameWithNick(Object o)
        {
            List<string> fieldNames = new List<string>();
            PropertyInfo[] propInfo = o.GetType().GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            foreach (PropertyInfo prop in propInfo)
            {
                var attList = prop.GetCustomAttributes(typeof(FieldNameAttribute), false);
                if ((attList != null) && (attList.Count() > 0))
                    fieldNames.Add(string.Format("{0}.{1}", getTableName(o), ((FieldNameAttribute)attList.First()).Description));
            }
            if (fieldNames.Count() > 0)
                return fieldNames;
            else
                throw new MinimumSpecificationsException(string.Format(ErrorMessages.INVALID_VO_FIELD_NAME, o.GetType()
                    .ToString()), DateTime.Now);
        }

        /// <summary>
        /// Método usado para retornar a property respectiva da primery key
        /// </summary>
        /// <param name="o">Objeto que desejamos saber a propriedade referente a  primary key</param>
        /// <returns>Nome da propriedade referente a primary key responsável na tabela</returns>
        public string getPropertyKeyField(Object o)
        {
            PropertyInfo[] propList = o.GetType().GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            foreach (PropertyInfo prop in propList)
            {
                object[] attList = prop.GetCustomAttributes(typeof(KeyFieldAttribute), false);
                if ((attList != null) && (attList.Count() > 0))
                    return prop.Name;
            }
            throw new MinimumSpecificationsException(string.Format(ErrorMessages.INVALID_VO_PRIMARY_KEY, o.GetType()
                .ToString()), DateTime.Now);
        }

        /// <summary>
        /// Método usado para retornar a property respectiva da primery key
        /// </summary>
        /// <param name="o">Objeto que desejamos saber a propriedade referente a  primary key</param>
        /// <returns>Nome da propriedade referente a primary key responsável na tabela</returns>
        public List<string> getPropertiesKeyField(Object o)
        {
            List<string> properties = new List<string>();
            PropertyInfo[] propList = o.GetType().GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            foreach (PropertyInfo prop in propList)
            {
                object[] attList = prop.GetCustomAttributes(typeof(KeyFieldAttribute), false);
                if ((attList != null) && (attList.Count() > 0))
                    properties.Add(prop.Name);
            }

            if (properties.Count == 0)
                throw new MinimumSpecificationsException(string.Format(ErrorMessages.INVALID_VO_PRIMARY_KEY, o.GetType()
                    .ToString()), DateTime.Now);

            return properties;
        }

        public List<ChildCascadeVO> getPropertiesFKField(Object o)
        {
            List<ChildCascadeVO> childs = new List<ChildCascadeVO>();

            Type t = o.GetType();
            object[] attList = t.GetCustomAttributes(typeof(FKAttribute), false);
            foreach (FKAttribute att in attList)
            {
                childs.Add(new ChildCascadeVO()
                {
                    Caption = att.Caption,
                    FieldCaption = att.FieldCaption,
                    FieldTarget = att.FieldTarget,
                    TableTarget = att.TableTarget
                });
            }
            return childs;
        }

        public TipoLogTabela GetLogTable(Object o)
        {
            Type t = o.GetType();
            object[] attList = t.GetCustomAttributes(typeof(LogTableAttribute), false);
            if (attList.Length > 0)
                return ((LogTableAttribute)attList[0]).Type;
            else
                return TipoLogTabela.logTabIndefinido;
        }

        public string GetOrigin(Object o)
        {
            Type t = o.GetType();
            object[] attList = t.GetCustomAttributes(typeof(OriginAttribute), false);
            if (attList.Length > 0)
                return ((OriginAttribute)attList[0]).Origin;
            else
                return "";
        }
    }

    /// <summary>
    /// Instancia singleton e estática de um Attribute Reader
    /// </summary>
    public static class AttributeReaderSingleton
    {
        /// <summary>
        /// Field interno do Attribute Reader
        /// </summary>
        static AttributeReader attributeReader = null;

        /// <summary>
        /// Objeto que enfileira as requisições
        /// </summary>
        static readonly object padlock = new object();

        /// <summary>
        /// Propriedade em singleton e estática de um Atributte Reader
        /// </summary>
        public static AttributeReader AttributeReader
        {
            get
            {
                lock (padlock)
                {
                    if (attributeReader == null)
                    {
                        attributeReader = new AttributeReader();
                    }
                    return attributeReader;
                }
            }
        }
    }
}
