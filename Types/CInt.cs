using Cabronate.DAO.Attributes;
using Cabronate.DAO.Errors;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace Cabronate.DAO.Types
{
    public class CInt : ICTypecs
    {
        public object getValue(object obj, FieldTypeDetail[] type, ValidationAttribute[] validationAttrib)
        {
            if ((type.Contains(FieldTypeDetail.CanNull)) && (Convert.ToInt32(obj) == 0))
                return DBNull.Value;

            object[] greaterThenAttribute = validationAttrib.Where(v => v.GetType() == typeof(GreaterThanAttribute)).ToArray();                           
            if ((greaterThenAttribute.Count() > 0) && (Convert.ToInt32(obj) < ((GreaterThanAttribute)greaterThenAttribute[0]).StartNumber))
                throw new GreaterThanException(string.Format(ErrorMessages.INVALID_RANGE_GREATER_ERROR, Convert.ToInt32(obj), ((GreaterThanAttribute)greaterThenAttribute[0]).StartNumber));

            if (type.Contains(FieldTypeDetail.IntDouble))
                return Convert.ToInt32(obj);
            else
                return obj;
        }

        public void setValue(PropertyInfo property, object obj, object value)
        {
            if (value == DBNull.Value)
            {
                object[] defaultAttribute = property.GetCustomAttributes(typeof(DefaultAttribute), false);

                if (defaultAttribute.Count() > 0)
                    property.SetValue(obj, ((DefaultAttribute)defaultAttribute[0]).defaultValue, null);
            }
            else
                property.SetValue(obj, Convert.ToInt32(value), null);
        }
    }
}
