using Cabronate.DAO.Attributes;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Cabronate.DAO.Types
{
    public class CDecimal : ICTypecs
    {
        public object getValue(object obj, Attributes.FieldTypeDetail[] type, ValidationAttribute[] validationAttrib)
        {
            if ((type.Contains(FieldTypeDetail.CanNull)) && (Convert.ToDecimal(obj) == 0))
                return DBNull.Value;
            else if (type.Contains(FieldTypeDetail.DecimalDouble))
                return Convert.ToDouble(obj);
            else
                return obj;
        }

        public void setValue(System.Reflection.PropertyInfo property, object obj, object value)
        {
            if (value == DBNull.Value)
            {
                object[] defaultValue = property.GetCustomAttributes(typeof(DefaultAttribute), false);

                if (defaultValue.Count() > 0)
                    property.SetValue(obj, ((DefaultAttribute)defaultValue[0]).defaultValue, null);
            }
            else
                property.SetValue(obj, Convert.ToDecimal(value), null);
        }
    }
}
