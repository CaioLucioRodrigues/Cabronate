using Cabronate.DAO.Attributes;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace Cabronate.DAO.Types
{
    public class CBoolean : ICTypecs
    {

        public object getValue(object obj, FieldTypeDetail[] type, ValidationAttribute[] validationAttrib)
        {
            if (obj != null)
            {
                if (type.Contains(FieldTypeDetail.BoolChar))
                    return ((bool)obj ? "T" : "F");
                else if (type.Contains(FieldTypeDetail.BoolInt))
                    return ((bool)obj ? 1 : 0);
            }

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
            else if (value.GetType() == typeof(String))
                property.SetValue(obj, ((string)value == "T"), null);
            else
                property.SetValue(obj, (Convert.ToInt32(value) == 1), null);

        }
    }
}
