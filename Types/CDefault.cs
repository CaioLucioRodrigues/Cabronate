using Cabronate.DAO.Attributes;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace Cabronate.DAO.Types
{
    public class CDefault : ICTypecs
    {
        public object getValue(object obj, FieldTypeDetail[] type, ValidationAttribute[] validationAttrib)
        {
            if ((obj != null) && (obj.GetType().BaseType != null) && (obj.GetType().BaseType.Name == "Enum"))
            {
                if (type.Contains(FieldTypeDetail.ENumString))
                    return Convert.ToString(obj);
                else
                    return Convert.ToInt32(obj);
            }
            else
                return obj;
        }

        public void setValue(PropertyInfo property, object obj, object value)
        {
            if (value == DBNull.Value || (value.Equals("") && (property.PropertyType).BaseType.Name == "Enum"))
            {
                object[] defaultValue = property.GetCustomAttributes(typeof(DefaultAttribute), false);

                if (defaultValue.Count() > 0)
                    property.SetValue(obj, ((DefaultAttribute)defaultValue[0]).defaultValue, null);
            }
            else if (((property.PropertyType).BaseType != null) && ((property.PropertyType).BaseType.Name == "Enum"))
            {
                object[] types = property.GetCustomAttributes(typeof(TypeDetailAttribute), false);
                if ((types.Count() > 0) && (((TypeDetailAttribute)types[0]).type.Contains(FieldTypeDetail.ENumString)))
                    value = Enum.Parse(property.PropertyType, value.ToString());

                property.SetValue(obj, Convert.ToInt32(value), null);
            }
            else if ((property.PropertyType.Name == "Nullable`1") &&
                (Nullable.GetUnderlyingType(property.PropertyType).IsEnum))
            {
                if (value != null)
                    property.SetValue(obj, Enum.ToObject(Nullable.GetUnderlyingType(property.PropertyType), value), null);
                else
                    property.SetValue(obj, value, null);
            }
            else
                property.SetValue(obj, value, null);
        }
    }
}
