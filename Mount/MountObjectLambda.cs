using Cabronate.Base;
using Cabronate.DAO.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Cabronate.DAO.Mount
{
    [Obsolete("CLASSE 10% MAIS DEVAGAR QUE O MOUNTOBJECT")]
    public sealed class MountObjectLambda<T> : IMountObject<T>
    {
        public T mountObject(System.Data.IDataReader reader, T obj, bool lazy = false, DBContexto dbctx = null)
        {
            try
            {
                if (reader.Read())
                {
                    PropertyInfo[] propList = obj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                    foreach (PropertyInfo property in propList)
                    {
                        object[] attributes = property.GetCustomAttributes(typeof(FieldNameAttribute), false);
                        if ((attributes != null) && (attributes.Count() > 0))
                            if ((property.CanWrite) && (!reader.IsDBNull(reader.GetOrdinal(((FieldNameAttribute)attributes[0]).Description))))
                            {
                                var propertyChange = typeof(T).GetProperty(property.Name);
                                var TType = Expression.Parameter(typeof(T));
                                var value = Expression.Parameter(property.PropertyType);
                                var assing = Expression.Assign(Expression.Property(TType, propertyChange), value);
                                var lambda = Expression.Lambda<Action<T, int>>(assing, TType, value).Compile();
                                var instance = Activator.CreateInstance<T>();
                                lambda(instance, int.Parse(reader[((FieldNameAttribute)attributes[0]).Description].ToString()));
                                property.SetValue(obj, reader[((FieldNameAttribute)attributes[0]).Description], null);
                            }
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

        public List<T> mountObjects(System.Data.IDataReader reader, T obj, bool lazy = false, DBContexto dbctx = null)
        {
            throw new NotImplementedException();
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
