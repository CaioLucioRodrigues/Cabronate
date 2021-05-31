using Cabronate.Base;
using Cabronate.DAO.Attributes;
using Cabronate.DAO.Errors;
using Cabronate.DAO.Types;
using Cabronate.DAO.Value_Objects;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace Cabronate.DAO.Mount
{
    /// <summary>
    /// Classe responsavel por carregar os objetos a partir de uma consulta ja feita no banco.
    /// Essa classe usa Reflection
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class MountObject<T> : IMountObject<T>
        where T : EcalcValueObject
    {
        /// <summary>
        /// Método usado para carregar um objeto a partir de um IDataReader
        /// </summary>
        /// <param name="reader">IDataReader ja preenchido</param>
        /// <param name="obj">Instância da classe que desejamos carregar</param>
        /// <returns>Objeto preenchido</returns>
        public T mountObject(IDataReader reader, T obj, bool lazy = false, DBContexto dbctx = null)
        {
            try
            {
                if (reader.Read())
                {
                    PropertyInfo[] propList = obj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                    foreach (PropertyInfo property in propList)
                    {
                        try
                        {
                            object[] attributes = property.GetCustomAttributes(typeof(FieldNameAttribute), false);
                            if ((attributes != null) && (attributes.Count() > 0))
                                if (property.CanWrite)
                                {
                                    TypeFactory.getCType(property.PropertyType, dbctx.Provider).setValue(property, obj,
                                        dbctx.GetObjectFromReader(reader, ((FieldNameAttribute)attributes[0]).Description));
                                }
                        }
                        catch (Exception e)
                        {
                            throw new LoadPropertyException(
                                string.Format(ErrorMessages.PROPERTY_ERROR, property.Name, property.DeclaringType), e, DateTime.Now);
                        }
                    }

                    if ((obj is ILoadChildren) && (!lazy))
                    {
                        (obj as ILoadChildren).LoadChildren(dbctx);
                    }

                    if (obj is IAfterLoadObject)
                    {
                        (obj as IAfterLoadObject).AfterLoadObject(dbctx);
                    }

                    return obj;
                }
                return default(T);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Método usado para carregar uma lista de objetos a partir de um IDataReader
        /// </summary>
        /// <param name="reader">IDataReader ja preenchido</param>
        /// <param name="obj">Instância da classe que desejamos carregar</param>
        /// <returns>Lista de objetos preenchidos</returns>
        public List<T> mountObjects(System.Data.IDataReader reader, T obj, bool lazy = false, DBContexto dbctx = null)
        {
            PropertyInfo[] propList = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            MountObjectMapper objMapper = new MountObjectMapper();
            objMapper.BuildTypeAndOrginal(typeof(T), reader, obj);

            List<T> list = new List<T>();
            List<MountObjectMapperField> allFieldLocal = objMapper.AllFields;
            while (reader.Read())
            {
                T o = Activator.CreateInstance<T>();

                foreach (MountObjectMapperField fld in allFieldLocal)
                {
                    try
                    {
                        if (fld.FieldOrdinal >= 0)
                        {
                            object fldData = dbctx.GetObjectFromReader(reader, fld.FieldOrdinal);

                            if ((fld.Property.CanWrite) && ((!(fldData == null)) || (fld.TemDefault)))
                            {
                                TypeFactory.getCType(fld.Property.PropertyType, dbctx.tipoBanco).setValue(fld.Property, o, fldData);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        throw new LoadPropertyException(
                            string.Format(ErrorMessages.PROPERTY_ERROR, fld.Property.Name, fld.Property.DeclaringType), e, DateTime.Now);
                    }
                }
                if ((o is ILoadChildren) && (!lazy))
                    (o as ILoadChildren).LoadChildren(dbctx);

                (o as EcalcValueObject).LazyObject = lazy;

                if (o is IAfterLoadObject)
                {
                    (o as IAfterLoadObject).AfterLoadObject(dbctx);
                }

                list.Add(o);
            }

            return list;
        }

        public bool hasEmptyConstructor(Type type)
        {
            ConstructorInfo[] cons = type.GetConstructors();
            foreach (ConstructorInfo c in cons)
            {
                if (c.GetParameters().Count() == 0)
                    return true;
            }
            return false;
        }
    }
}
