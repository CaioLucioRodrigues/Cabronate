using Cabronate.DAO.Attributes;
using Cabronate.DAO.Errors;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace Cabronate.DAO.Types
{
    public class CDouble : ICTypecs
    {
        public object getValue(object obj, FieldTypeDetail[] type, ValidationAttribute[] validationAttrib)
        {
            if ((type.Contains(FieldTypeDetail.CanNull)) && (Convert.ToDouble(obj) == 0))
                return DBNull.Value;

            object[] greaterThenAttribute = validationAttrib.Where(v => v.GetType() == typeof(GreaterThanAttribute)).ToArray();
            if ((greaterThenAttribute.Count() > 0) && (Convert.ToDouble(obj) < ((GreaterThanAttribute)greaterThenAttribute[0]).StartNumber))
                throw new GreaterThanException(string.Format(ErrorMessages.INVALID_RANGE_GREATER_ERROR, Convert.ToDouble(obj), ((GreaterThanAttribute)greaterThenAttribute[0]).StartNumber));

            return obj;
        }

        public void setValue(PropertyInfo property, object obj, object value)
        {
            if (value == DBNull.Value)
            {
                object[] defaultValue = property.GetCustomAttributes(typeof(DefaultAttribute), false);

                if (defaultValue.Count() > 0)
                    property.SetValue(obj, ((DefaultAttribute)defaultValue[0]).defaultValue, null);
            }
            else
                property.SetValue(obj, Convert.ToDouble(value), null);
        }
    }
}

